using VRPTW.Services.Operators;
using VRPTW.Utils;

namespace VRPTW.Model;

[Serializable]
public class VrptwGraph
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public List<Depot> Depots { get; set; } = new();
    public List<Client> Clients { get; set; } = new();
    public List<List<string>> Roads { get; set; } = new();
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

    public bool IsBetterThan(VrptwGraph other)
    {
        return this.GetTotalDistance() < other.GetTotalDistance();
    }

    public void GenerateInitialSolution()
    {
        List<Client> clients = this.Clients.OrderBy(c => Guid.NewGuid()).ToList();
        List<string> newRoad = new();
        int currentQuantity = 0;
        for (int i = 0; i < clients.Count; i++)
        {
            Client client = clients[i];
            if (currentQuantity + client.Demand > this.MaxQuantity)
            {
                this.Roads.Add(newRoad);
                newRoad = new();
                currentQuantity = 0;
            }
            newRoad.Add(client.Id);
            currentQuantity += client.Demand;
            if (i == clients.Count - 1)
            {
                this.Roads.Add(newRoad);
            }
        }
    }

    public int GetTotalDistance()
    {
        return this.Roads.Select(t =>
        {
            int s = this.Depots.First().GetDistanceWith(this.Clients.Find(c => c.Id == t.First()));
            for (int i = 0; i < t.Count - 1; i++)
            {
                Client? c = this.Clients.Find(c => c.Id == t[i]);
                Client? c2 = this.Clients.Find(c => c.Id == t[i + 1]);
                s += c.GetDistanceWith(c2);
            }
            s += this.Depots.Last().GetDistanceWith(this.Clients.Find(c => c.Id == t.Last()));
            return s;
        }).Sum();
    }

    private static VrptwGraph CheckAndDeleteEmptyRoads(VrptwGraph graph)
    {
        graph.Roads = graph.Roads.Where(r => r.Count > 0).ToList();
        return graph;
    }

    public VrptwGraph? GetNextNeighbour(List<NeighboursMethods>? methods = default)
    {
        methods = methods ?? this.AllNeighboursMethods();
        VrptwGraph neighbour = ClassUtils.DeepClone(this);
        foreach (NeighboursMethods method in methods)
        {
            switch (method)
            {
                case NeighboursMethods.Relocate:
                    return RelocateOperator.Calculate(this);
                case NeighboursMethods.Exchange:
                    return ExchangeOperator.Calculate(this);
                case NeighboursMethods.Reverse:
                    break;
                case NeighboursMethods.Two_Opt:
                    return TwoOptOperator.Calculate(this);
                case NeighboursMethods.CrossExchange:
                    break;
            }
        }
        return default;
    }

    private List<NeighboursMethods> AllNeighboursMethods()
    {
        return new List<NeighboursMethods>() {
            NeighboursMethods.Relocate,
            NeighboursMethods.Exchange,
            NeighboursMethods.Reverse,
            NeighboursMethods.Two_Opt,
            NeighboursMethods.CrossExchange
        };
    }

    #region Meta-heuristique

    public static VrptwGraph HillClimbing(VrptwGraph graph, List<NeighboursMethods>? neighboursMethods = null, int? nbGeneration = null)
    {
        neighboursMethods ??= new() { NeighboursMethods.Relocate };
        while (nbGeneration != 0)
        {
            nbGeneration--;
            var neighbour = graph.GetNextNeighbour(neighboursMethods);
            if (neighbour != default)
            {
                Console.WriteLine("Nouveau voisin trouvé");
                neighbour = CheckAndDeleteEmptyRoads(neighbour);
                graph = neighbour;
            }
            else
            {
                break;
            }
        }
        return graph;
    }

    #endregion
}