namespace VRPTW.Model;

public class VrptwGraph
{
    public String name { get; set; } = string.Empty;
    public String description { get; set; } = string.Empty;
    public String type { get; set; } = string.Empty;
    public List<Depot> Depots { get; set; } = new();
    public List<Client> Clients { get; set; } = new();
    public List<Truck> Trucks { get; set; } = new();
    public int NbDepots { get; set; } = 0;
    public int NbClients { get; set; } = 0;
    public int MaxQuantity { get; set; } = 0;

    public void AddDepot(Depot depot)
    {
        Depots.Add(depot);
    }

    public void AddClient(Client client)
    {
        Clients.Add(client);
    }

    public int GetMinTrucks()
    {
        int totalDemand = Clients.Select(c => c.Demand).Sum();
        return (int)Math.Ceiling((double)totalDemand / MaxQuantity);
    }

    public VrptwGraph GenerateInitialSolution()
    {
        VrptwGraph newGraph = (VrptwGraph)this.MemberwiseClone();
        var clientEnumerator = newGraph.Clients.GetEnumerator();
        Truck currentTruck = new() { Id = 0, Capacity = MaxQuantity };
        while (clientEnumerator.MoveNext())
        {
            Client nextClient = clientEnumerator.Current;
            if (currentTruck.Content + nextClient.Demand > currentTruck.Capacity)
            {
                newGraph.Trucks.Add(currentTruck);
                currentTruck = new() { Id = currentTruck.Id + 1, Capacity = MaxQuantity };
            }
            currentTruck.AddStage(nextClient);
        }
        newGraph.Trucks.Add(currentTruck);
        return newGraph;
    }
}