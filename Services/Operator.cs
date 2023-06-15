using System.Drawing;
using VRPTW.Model;
using VRPTW.Utils;

namespace VRPTW.Services
{
    public static class Operator
    {
        #region Operators

        public static void Relocate(this VrptwGraph graph, String c, int r, int pos)
        {
            List<string> dest = graph.Roads[r];
            List<string> origin = graph.Roads.First(r => r.Contains(c));

            if (!dest.Contains(c) &&
                graph.GetRoadDistance(dest) + graph.Clients.First(cl => cl.Id == c).Demand > graph.MaxQuantity)
            {
                throw new InvalidOperationException("The destination road is full");
            }

            origin.Remove(c);
            dest.Insert(pos, c);
        }

        public static void Exchange(this VrptwGraph graph, String c1, String c2)
        {
            List<string> road1 = graph.Roads.First(r => r.Contains(c1));
            List<string> road2 = graph.Roads.First(r => r.Contains(c2));

            int demand1 = graph.Clients.First(cl => cl.Id == c1).Demand;
            int demand2 = graph.Clients.First(cl => cl.Id == c2).Demand;

            if (!road1.Contains(c2) &&
               (graph.GetRoadDistance(road1) - demand1 + demand2 > graph.MaxQuantity ||
                graph.GetRoadDistance(road2) - demand2 + demand1 > graph.MaxQuantity))
            {
                throw new InvalidOperationException("The destination road is full");
            }

            int pos1 = road1.IndexOf(c1);
            int pos2 = road2.IndexOf(c2);

            road1.Remove(c1);
            road1.Insert(pos1, c2);
            road2.Remove(c2);
            road2.Insert(pos2, c1);
        }

        public static void Reverse(this VrptwGraph graph, int r)
        {
            graph.Roads[r].Reverse();
        }

        public static void TwoOpt(this VrptwGraph graph, int r, string c)
        {
            List<string> road = graph.Roads[r];

            int start = road.IndexOf(c);
            int end = road.IndexOf(c) + 2;

            if (end >= road.Count)
            {
                throw new InvalidOperationException("Cannot perform Two-Opt on the 2 last clients of a road");
            }

            graph.Exchange(road[start], road[end]);
        }

        public static void CrossExchange(this VrptwGraph graph, string start1, string start2, int length1, int length2)
        {
            List<string> road1 = graph.Roads.First(r => r.Contains(start1));
            if (road1.Contains(start2))
            {
                throw new InvalidOperationException("Cannot perform Cross-Exchange on ranges from the same road");
            }
            List<string> road2 = graph.Roads.First(r => r.Contains(start2));

            List<string> range1 = road1.GetRange(road1.IndexOf(start1), length1);
            List<string> range2 = road2.GetRange(road2.IndexOf(start2), length2);

            int demand1 = range1.Sum(c => graph.Clients.First(cl => cl.Id == c).Demand);
            int demand2 = range2.Sum(c => graph.Clients.First(cl => cl.Id == c).Demand);

            if (graph.GetRoadDistance(road1) - demand1 + demand2 > graph.MaxQuantity ||
                               graph.GetRoadDistance(road2) - demand2 + demand1 > graph.MaxQuantity)
            {
                throw new InvalidOperationException("The destination road is full");
            }

            int pos1 = road1.IndexOf(start1);
            int pos2 = road2.IndexOf(start2);

            road1.RemoveRange(road1.IndexOf(start1), length1);
            road2.RemoveRange(road2.IndexOf(start2), length2);

            road1.InsertRange(pos1, range2);
            road2.InsertRange(pos2, range1);
        }

        #endregion

        #region Neighbours

        public static VrptwGraph GetRelocateNeighbours(this VrptwGraph graph, int distance)
        {
            foreach (Client c in graph.Clients)
            {
                for (int r = 0; r < graph.Roads.Count; r++)
                {
                    for (int i = 0; i < graph.Roads[r].Count; i++)
                    {
                        try
                        {
                            VrptwGraph newGraph = graph.DeepClone();
                            newGraph.Relocate(c.Id, r, i);
                            if (newGraph.GetTotalDistance() < distance)
                            {
                                Console.WriteLine($"Nouveau voisin trouvé avec une fitness de {newGraph.GetTotalDistance()}");
                                return newGraph;
                            }
                        }
                        catch (InvalidOperationException e)
                        {
                            //Console.WriteLine(e.Message);
                        }
                    }
                }
            }
            return null;
        }

        public static VrptwGraph GetExchangeNeighbours(this VrptwGraph graph, int distance)
        {
            foreach (Client c1 in graph.Clients)
            {
                foreach (Client c2 in graph.Clients)
                {
                    try
                    {
                        VrptwGraph newGraph = graph.DeepClone();
                        newGraph.Exchange(c1.Id, c2.Id);
                        if (newGraph.GetTotalDistance() < distance)
                        {
                            Console.WriteLine($"Nouveau voisin trouvé avec une fitness de {newGraph.GetTotalDistance()}");
                            return newGraph;
                        }
                    }
                    catch (InvalidOperationException e)
                    {
                        //Console.WriteLine(e.Message);
                    }
                }
            }
            return null;
        }

        public static VrptwGraph GetReverseNeighbours(this VrptwGraph graph, int distance)
        {
            for (int r = 0; r < graph.Roads.Count; r++)
            {
                try
                {
                    VrptwGraph newGraph = graph.DeepClone();
                    newGraph.Reverse(r);
                    if (newGraph.GetTotalDistance() < distance)
                    {
                        Console.WriteLine($"Nouveau voisin trouvé avec une fitness de {newGraph.GetTotalDistance()}");
                        return newGraph;
                    }
                }
                catch (InvalidOperationException e)
                {
                    //Console.WriteLine(e.Message);
                }
            }
            return null;
        }

        public static VrptwGraph GetTwoOptNeighbours(this VrptwGraph graph, int distance)
        {
            foreach (List<string> road in graph.Roads)
            {
                foreach (string c in road)
                {
                    try
                    {
                        VrptwGraph newGraph = graph.DeepClone();
                        newGraph.TwoOpt(graph.Roads.IndexOf(road), c);
                        if (newGraph.GetTotalDistance() < distance)
                        {
                            Console.WriteLine($"Nouveau voisin trouvé avec une fitness de {newGraph.GetTotalDistance()}");
                            return newGraph;
                        }
                    }
                    catch (InvalidOperationException e)
                    {
                        //Console.WriteLine(e.Message);
                    }
                }
            }
            return null;
        }

        public static VrptwGraph GetCrossExchangeNeighbours(this VrptwGraph graph, int distance)
        {
            for (int r1 = 0; r1 < graph.Roads.Count; r1++)
            {
                for (int r2 = 0; r2 < graph.Roads.Count; r2++)
                {
                    if (r1 == r2)
                    {
                        continue;
                    }
                    for (int i = 0; i < graph.Roads[r1].Count; i++)
                    {
                        for (int j = 0; j < graph.Roads[r2].Count; j++)
                        {
                            for (int l1 = 1; l1 < graph.Roads[r1].Count - i; l1++)
                            {
                                for (int l2 = 1; l2 < graph.Roads[r2].Count - j; l2++)
                                {
                                    try
                                    {
                                        VrptwGraph newGraph = graph.DeepClone();
                                        newGraph.CrossExchange(graph.Roads[r1][i], graph.Roads[r2][j], l1, l2);
                                        if (newGraph.GetTotalDistance() < distance)
                                        {
                                            Console.WriteLine($"Nouveau voisin trouvé avec une fitness de {newGraph.GetTotalDistance()}");
                                            return newGraph;
                                        }
                                    }
                                    catch (InvalidOperationException e)
                                    {
                                        //Console.WriteLine(e.Message);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
