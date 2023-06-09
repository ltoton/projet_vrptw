using VRPTW.Model;
using VRPTW.Utils;

namespace VRPTW.Services.Operators;

class ReverseOperator
{
    public static VrptwGraph Calculate(VrptwGraph graph)
    {
        foreach(Truck truck in graph.Trucks)
        {
            truck.Stages.Reverse();
        }
        return graph;
    }
}
