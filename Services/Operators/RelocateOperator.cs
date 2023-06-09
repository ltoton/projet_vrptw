using VRPTW.Model;
using VRPTW.Utils;

namespace VRPTW.Services.Operators;

class RelocateOperator
{
    public static VrptwGraph Calculate(VrptwGraph graph)
    {
        VrptwGraph newGraph = ClassUtils.DeepClone(graph);
        foreach (Client client in newGraph.Clients)
        {
            foreach (Truck truck in newGraph.Trucks)
            {
                for (int i = 0; i < truck.Stages.Count; i++)
                {
                    if (truck.Stages[i].Id != client.Id)
                    {
                        Truck? firstTruck = newGraph.Trucks.Find(t => t.Stages.Any(c => c.Id == client.Id));
                        if (
                            firstTruck != null && (
                            truck.Id == firstTruck.Id ||
                            truck.Capacity >= truck.Content - truck.Stages[i].Demand + client.Demand
                            ))
                        {
                            firstTruck.RemoveStage(client);
                            truck.AddStage(client, i);
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
