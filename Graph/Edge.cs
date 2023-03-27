namespace VRPTW.Graph;
public class Edge
{
    public Vertex Source { get; set; } = new Vertex();

    public Vertex Destination { get; set; } = new Vertex();

    public List<double> Valuations { get; set; } = new List<double>();

    public double Distance => Math.Sqrt(Math.Pow(Destination.X - Source.X, 2) + Math.Pow(Destination.Y - Source.Y, 2));
}
