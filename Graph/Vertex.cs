using VRPTW.Model;

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

    public int GetDistanceWith(Vertex? client)
    {
        return client != default ? (int)Math.Sqrt(Math.Pow(this.X - client.X, 2) + Math.Pow(this.Y - client.Y, 2)) : 0;
    }
}
