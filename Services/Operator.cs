using VRPTW.Model;

namespace VRPTW.Services
{
    public static class Operator
    {
        public static void Relocate(this VrptwGraph graph, String c, int r, int pos)
        {
            List<string> dest = graph.Roads[r];
            List<string> origin = graph.Roads.First(r => r.Contains(c));

            origin.Remove(c);
            dest.Insert(pos, c);
        }
    }
}
