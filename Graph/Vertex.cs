namespace VRPTW.Graph;
public class Vertex
{
    string Id { get; set; } = string.Empty;

    public int X { get; set; }

    public int Y { get; set; }

    List<Edge> Edges { get; set; } = new List<Edge>();

    void AddEdge(Edge edge)
    {
        Edges.Add(edge);
    }
}
