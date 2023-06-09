using VRPTW.Model;
using VRPTW.Utils;

namespace VRPTW.Services.Operators;

class ExchangeOperator
{
    public static VrptwGraph Calculate(VrptwGraph graph)
    {
        VrptwGraph newGraph = ClassUtils.DeepClone(graph);
        foreach(Client client1 in newGraph.Clients)
        {
            foreach(Client client2 in newGraph.Clients)
            {
                if(client1.Id != client2.Id)
                {
                    Truck? truck1 = newGraph.Trucks.Find(t => t.Stages.Any(c => c.Id == client1.Id));
                    Truck? truck2 = newGraph.Trucks.Find(t => t.Stages.Any(c => c.Id == client2.Id));
                    if (truck1 != null && truck2 != null)
                    {
                        int posClient1 = truck1.Stages.FindIndex(c => c.Id == client1.Id);
                        int posClient2 = truck2.Stages.FindIndex(c => c.Id == client2.Id);

                        if (truck1.Capacity >= truck1.Content - client1.Demand + client2.Demand &&
                                                       truck2.Capacity >= truck2.Content - client2.Demand + client1.Demand)
                        {
                            truck1.RemoveStage(client1);
                            truck2.RemoveStage(client2);
                            truck1.AddStage(client2, posClient1);
                            truck2.AddStage(client1, posClient2);

                            if (newGraph.IsBetterThan(graph))
                            {
                                return newGraph;
                            }
                            else
                            {
                                newGraph = ClassUtils.DeepClone(graph);
                            }
                        }
                    }
                }
            }
        }

        return graph;
    }
}
