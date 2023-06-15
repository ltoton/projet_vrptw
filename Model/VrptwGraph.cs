using System.Diagnostics;
using VRPTW.Services;
using VRPTW.Utils;

namespace VRPTW.Model;

[Serializable]
public class VrptwGraph
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public List<Depot> Depots { get; set; } = new();
    public List<Client> Clients { get; set; } = new();
    public List<List<string>> Roads { get; set; } = new();
    public int NbDepots { get; set; } = 0;
    public int NbClients { get; set; } = 0;
    public int MaxQuantity { get; set; } = 0;

    public void AddDepot(Depot depot)
    {
        Depots.Add(depot);
    }

    public void AddClient(Client client)
    {
        Clients.Add(client);
    }

    public int GetMinTrucks()
    {
        int totalDemand = Clients.Select(c => c.Demand).Sum();
        return (int)Math.Ceiling((double)totalDemand / MaxQuantity);
    }

    public bool IsBetterThan(VrptwGraph other)
    {
        return this.GetTotalDistance() < other.GetTotalDistance();
    }

    public void GenerateInitialSolution()
    {
        List<Client> clients = this.Clients.OrderBy(c => Guid.NewGuid()).ToList();
        List<string> newRoad = new();
        int currentQuantity = 0;
        for (int i = 0; i < clients.Count; i++)
        {
            Client client = clients[i];
            if (currentQuantity + client.Demand > this.MaxQuantity)
            {
                this.Roads.Add(newRoad);
                newRoad = new();
                currentQuantity = 0;
            }
            newRoad.Add(client.Id);
            currentQuantity += client.Demand;
            if (i == clients.Count - 1)
            {
                this.Roads.Add(newRoad);
            }
        }
    }

    public int GetTotalDistance()
    {
        return this.Roads.Select(GetRoadDistance).Sum();
    }

    public int GetRoadDistance(List<string> road)
    {
        Client? client = this.Clients.Find(c => c.Id == road.First());
        int s = this.Depots.First().GetDistanceWith(client);
        for (int i = 0; i < road.Count - 1; i++)
        {
            Client? next = this.Clients.Find(c => c.Id == road[i + 1]);
            s += client.GetDistanceWith(next);
            client = next;
        }
        s += this.Depots.Last().GetDistanceWith(client);
        return s;
    }

    private static VrptwGraph CheckAndDeleteEmptyRoads(VrptwGraph graph)
    {
        graph.Roads = graph.Roads.Where(r => r.Count > 0).ToList();
        return graph;
    }

    public (VrptwGraph, NeighboursMethods) GetNeighbour(List<NeighboursMethods>? methods = default)
    {
        int currentDistance = this.GetTotalDistance();
        methods = methods ?? this.AllNeighboursMethods();
        VrptwGraph neighbour = null;
        var temp_methods = methods.DeepClone();
        NeighboursMethods method = NeighboursMethods.Exchange;
        while (temp_methods.Count > 0)
        {
            method = temp_methods.OrderBy(m => Guid.NewGuid()).First();
            temp_methods.Remove(method);
            switch (method)
            {
                case NeighboursMethods.Relocate:
                    neighbour = this.GetRelocateNeighbours(currentDistance);
                    break;
                case NeighboursMethods.Exchange:
                    neighbour = this.GetExchangeNeighbours(currentDistance);
                    break;
                case NeighboursMethods.Reverse:
                    neighbour = this.GetReverseNeighbours(currentDistance);
                    break;
                case NeighboursMethods.Two_Opt:
                    neighbour = this.GetTwoOptNeighbours(currentDistance);
                    break;
                case NeighboursMethods.CrossExchange:
                    neighbour = this.GetCrossExchangeNeighbours(currentDistance);
                    break;
            }
            if (neighbour != null)
            {
                return (neighbour, method);
            }
        }
        return (null, method);
    }

    private List<NeighboursMethods> AllNeighboursMethods()
    {
        return new List<NeighboursMethods>() {
            NeighboursMethods.Relocate,
            NeighboursMethods.Exchange,
            NeighboursMethods.Reverse,
            NeighboursMethods.Two_Opt,
            NeighboursMethods.CrossExchange
        };
    }

    public static VrptwGraph GetBest(List<VrptwGraph> graphs, VrptwGraph g)
    {
        graphs.Add(g);
        return graphs.OrderBy(g => g.GetTotalDistance()).First();
    }

    #region Meta-heuristique

    public static VrptwGraph HillClimbing(VrptwGraph graph, List<NeighboursMethods>? neighboursMethods = null, bool export = false)
    {
        neighboursMethods ??= new() { NeighboursMethods.Relocate };
        int k = 0;
        List<int> distances = new();
        List<NeighboursMethods> usedOperators = new();
        List<int> nbVehicules = new();
        List<long> timesToFindNeighbour = new();
        Stopwatch timer = new();
        timer.Start();
        (VrptwGraph neighbour, NeighboursMethods method) neighbour = graph.GetNeighbour(neighboursMethods);
        timesToFindNeighbour.Add(timer.ElapsedMilliseconds);
        if (neighbour.neighbour == null)
        {
            return graph;
        }
        do
        {
            graph = CheckAndDeleteEmptyRoads(neighbour.neighbour);
            k++;
            distances.Add(graph.GetTotalDistance());
            usedOperators.Add(neighbour.method);
            nbVehicules.Add(graph.Roads.Count);
            timer.Restart();
            neighbour = graph.GetNeighbour(neighboursMethods);
            timesToFindNeighbour.Add(timer.ElapsedMilliseconds);
        }
        while (neighbour.neighbour != null);
        if (export)
        {
            // export the number of iteration and the evolution of the distance and the list of used operators
            exportDatas(graph.Name, k, distances, usedOperators, nbVehicules, timesToFindNeighbour);
        }
        return graph;
    }

    private static void exportDatas(string name, int k, List<int> distances, List<NeighboursMethods> usedOperators, List<int> nbVehicules, List<long> times)
    {
        string path = $"./export/hill-climbing/{new string(name.SkipLast(4).ToArray())}_{DateTime.Now:dd-MM-yyyy-HH-mm}.csv";
        // create the directory if it doesn't exist
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        using StreamWriter sw = new(path);
        sw.WriteLine("iteration,distance,operator,nbVehicules,timeToFindNeighbour");
        for (int i = 0; i < k; i++)
        {
            sw.WriteLine($"{i},{distances[i]},{usedOperators[i]},{nbVehicules[i]},{times[i]}");
        }
        Console.WriteLine($"Exported in {path}");
    }

    public static VrptwGraph SimulatedAnealing(VrptwGraph graph, List<NeighboursMethods>? neighboursMethods = null)
    {
        neighboursMethods ??= new() { NeighboursMethods.Relocate };
        VrptwGraph currentSolution = graph.DeepClone();
        VrptwGraph bestSolution = graph.DeepClone();
        double coolingRate = 0.8;
        int maxIteration = 1000;
        double currentTemperature = GetInitialeTemperature(graph, maxIteration);
        Random random = new();
        double currentDistance = currentSolution.GetTotalDistance();

        for (int iteration = 0; iteration < maxIteration; iteration++)
        {
            VrptwGraph neighbour = currentSolution.GetNeighbour(neighboursMethods).Item1;
            if (neighbour == null)
            {
                return currentSolution;
            }
            double neighbourDistance = neighbour.GetTotalDistance();
            if (neighbourDistance < currentDistance || Math.Exp((currentDistance - neighbourDistance) / currentTemperature) > random.NextDouble())
            {
                currentSolution = neighbour;
                currentDistance = neighbourDistance;
            }
            if (neighbourDistance < bestSolution.GetTotalDistance())
            {
                bestSolution = neighbour;
            }
            currentTemperature *= coolingRate;
            Console.WriteLine($"Iteration {iteration} : {currentSolution.GetTotalDistance()}");
        }
        return bestSolution;
    }

    private static double GetInitialeTemperature(VrptwGraph graph, int nbIteration)
    {
        int sumDiffLess = 0;
        int nbLess = 0;
        var s = graph.DeepClone();
        int e = graph.GetTotalDistance();
        int k = 0;
        for (int i = 0; i < nbIteration; i++)
        {
            var sn = s.GetNeighbour().Item1;
            var en = sn.GetTotalDistance();
            if (en > e)
            {
                sumDiffLess += en - e;
                nbLess++;
            }
            k++;
        }
        return (-sumDiffLess / nbLess) / Math.Log(0.8);
    }

    #endregion
}