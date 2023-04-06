using VRPTW.Graph;

namespace VRPTW.Model;

public class Client : Vertex
{
    public int ReadyTime { get; set; } = 0;

    public int DueTime { get; set; } = 0;

    public int Demand { get; set; } = 0;

    public int Service { get; set; } = 0;
}
