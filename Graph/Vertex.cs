namespace VRPTW.Graph;
public class Vertex
{
    public string Id { get; set; } = string.Empty;

    public int X { get; set; }

    public int Y { get; set; }

    public List<Edge> Edges { get; set; } = new List<Edge>();

    public void AddEdge(Edge edge)
    {
        Edges.Add(edge);
    }
}
