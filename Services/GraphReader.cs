using VRPTW.Model;

namespace VRPTW.Services;

public static class GraphReader
{
    public static VrptwGraph ReadVrptw(string path)
    {
        VrptwGraph graph = new VrptwGraph();
        string[] lines = File.ReadAllLines(path);
        bool depot = false;
        bool client = false;
        foreach (var line in lines)
        {
            if (line.StartsWith("NAME"))
            {
                graph.Name = line.Split(':')[1].Trim();
                continue;
            }

            if (line.StartsWith("COMMENT"))
            {
                graph.Description = line.Split(':')[1].Trim();
                continue;
            }

            if (line.StartsWith("TYPE"))
            {
                graph.Type = line.Split(':')[1].Trim();
                continue;
            }

            if (line.StartsWith("NB_DEPOTS"))
            {
                graph.NbDepots = int.Parse(line.Split(':')[1].Trim());
                continue;
            }

            if (line.StartsWith("NB_CLIENTS"))
            {
                graph.NbClients = int.Parse(line.Split(':')[1].Trim());
                continue;
            }

            if (line.StartsWith("MAX_QUANTITY"))
            {
                graph.MaxQuantity = int.Parse(line.Split(':')[1].Trim());
                continue;
            }

            if (line.StartsWith("DATA_DEPOTS"))
            {
                depot = true;
                client = false;
                continue;
            }

            if (line.StartsWith("DATA_CLIENTS"))
            {
                depot = false;
                client = true;
                continue;
            }

            if (depot && line != "")
            {
                addDepot(graph, line);
            }

            if (client && line != "")
            {
                addClient(graph, line);
            }
        }
        return graph;
    }

    private static void addDepot(VrptwGraph graph, string line)
    {
        string[] data = line.Split(' ');
        Depot depot = new Depot();
        depot.Id = data[0];
        depot.X = int.Parse(data[1]);
        depot.Y = int.Parse(data[2]);
        depot.ReadyTime = int.Parse(data[3]);
        depot.DueTime = int.Parse(data[4]);
        graph.AddDepot(depot);
    }

    private static void addClient(VrptwGraph graph, string line)
    {
        string[] data = line.Split(' ');
        Client client = new Client();
        client.Id = data[0];
        client.X = int.Parse(data[1]);
        client.Y = int.Parse(data[2]);
        client.ReadyTime = int.Parse(data[3]);
        client.DueTime = int.Parse(data[4]);
        client.Demand = int.Parse(data[5]);
        client.Service = int.Parse(data[6]);
        graph.AddClient(client);
    }
}
