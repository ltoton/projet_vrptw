using VRPTW.Model;
using VRPTW.Utils;

namespace VRPTW.Services.Operators;

class TwoOptOperator
{
    public static VrptwGraph Calculate(VrptwGraph graph)
    {
        VrptwGraph newGraph = ClassUtils.DeepClone(graph);
        foreach(Truck truck in newGraph.Trucks)
        {
            for (int start = 0; start < truck.Stages.Count - 2; start++)
            {
                int end = start + 2;
                Client startStage = truck.Stages[start];
                Client endStage = truck.Stages[end];

                truck.RemoveStage(startStage);
                truck.RemoveStage(endStage);

                truck.AddStage(endStage, start);
                truck.AddStage(startStage, end);

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

        return graph;
    }
}
