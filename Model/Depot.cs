using VRPTW.Graph;

namespace VRPTW.Model;

public class Depot : Vertex
{
    public int ReadyTime { get; set; } = 0;

    public int DueTime { get; set; } = 0;
}