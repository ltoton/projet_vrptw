namespace VRPTW.Model;

public class VrptwGraph
{
    public String name { get; set; } = string.Empty;
    public String description { get; set; } = string.Empty;
    public String type { get; set; } = string.Empty;
    public List<Depot> Depots { get; set; } = new List<Depot> { };
    public List<Client> Clients { get; set; } = new List<Client> { };
    public int NbDepots { get; set; } = 0;
    public int NbClients { get; set; } = 0;
    public int MaxQuantity { get; set; } = 0;

    public void addDepot(Depot depot)
    {
        Depots.Add(depot);
    }

    public void addClient(Client client)
    {
        Clients.Add(client);
    }
}