namespace VRPTW.Model;
public class VRPTW
{
    public String name { get; set; } = string.Empty;
    public String description { get; set; } = string.Empty;
    public String type { get; set; } = string.Empty;
    public List<Depot> Depots { get; set; } = new List<Depot> { };
    public List<Client> Clients { get; set; } = new List<Client> { };
    public int NbDepots { get; set; } = Depots.Count;
    public int NbClients { get; set; } = Clients.Count;
    public int MaxQuantity { get; set; } = 0;
}