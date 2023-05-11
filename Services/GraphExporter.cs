using VRPTW.Model;

namespace VRPTW.Services
{
    public static class GraphExporter
    {
        public static void exportToCSV(VrptwGraph graph)
        {
            string outputPath = "./../../../Output/" + graph.Name + "_export_" + DateTime.Now.ToString("HH-mm-ss") + ".csv";
            string[] lines = new string[graph.NbClients + 1];
            lines[0] = "Client;X;Y;Demand;ReadyTime;";
            for (int i = 0; i < graph.NbClients; i++)
            {
                lines[i + 1] = graph.Clients[i].Id + ";" + graph.Clients[i].X + ";" + graph.Clients[i].Y + ";" + graph.Clients[i].Demand + ";" + graph.Clients[i].ReadyTime + ";";
            }
            File.WriteAllLines(outputPath, lines);
        }
    }
}
