using VRPTW.Graph;

namespace VRPTW.Model;
public class Depot: Vertex
{
    public int DueTime { get; set; } = 0;

    public int Demand { get; set; } = 0;
}