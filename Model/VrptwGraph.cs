using VRPTW.Graph;
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

    public bool IsBetterThan(VrptwGraph other)
    {
        return this.GetTotalDistance() < other.GetTotalDistance();
    }

    public void GenerateInitialSolution()
    {
        List<Client> clients = this.Clients.OrderBy(c => c.ReadyTime).ToList();
        List<Client> leftClients = new();
        Truck currentTruck = new() { Id = 0, Capacity = this.MaxQuantity, Depot = this.Depots[0] };
        while (clients.Any() || leftClients.Any())
        {
            Client? client = clients.FirstOrDefault();
            if (client == default || currentTruck.Content + client.Demand > currentTruck.Capacity)
            {
                this.Trucks.Add(currentTruck);
                currentTruck = new() { Id = currentTruck.Id + 1, Capacity = MaxQuantity, Depot = this.Depots[0] };
                clients = clients.Concat(leftClients).OrderBy(c => c.ReadyTime).ToList();
                leftClients.Clear();
                continue;
            }
            int arrivalTime = currentTruck.GetDistance() + currentTruck.LastStage.GetDistanceWith(client);
            if (arrivalTime <= client.DueTime)
            {
                currentTruck.AddStage(client);
                clients.Remove(client);
                continue;
            }
            clients.Remove(client);
            leftClients.Add(client);
        }
        if (currentTruck.Stages.Any())
            this.Trucks.Add(currentTruck);
    }

    public int GetTotalDistance()
    {
        return this.Trucks.Select(t => t.GetDistance()).Sum();
    }

    private static VrptwGraph CheckAndDeleteEmptyTrucks(VrptwGraph graph)
    {
        List<Truck> toDelete = new();
        foreach (Truck truck in graph.Trucks)
        {
            if (!truck.Stages.Any())
            {
                toDelete.Add(truck);
            }
        }
        foreach (Truck truck in toDelete)
        {
            graph.Trucks.Remove(truck);
        }
        return graph;
    }

    public void Relocate(Client client, Truck truck, int i)
    {
        Truck? truck1 = this.Trucks.Find((t) => t.Stages.Any(c => c.Id == client.Id));
        if (truck1 != null)
        {
            int leftPlace = truck.Capacity - truck.Content;
            if (leftPlace < client.Demand)
            {
                throw new InvalidOperationException();
            }
            truck1.RemoveStage(client);
            truck.AddStage(client, i);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    public void Exchange(Client client1, Client client2)
    {
        if (client1.Id == client2.Id)
        {
            throw new InvalidOperationException();
        }
        Truck? truck1 = this.Trucks.Find((t) => t.Stages.Any((c) => c.Id == client1.Id));
        Truck? truck2 = this.Trucks.Find((t) => t.Stages.Any((c) => c.Id == client1.Id));
        if (truck1 != null && truck2 != null)
        {
            int leftPlace1 = truck1.Capacity - client1.Demand;
            int leftPlace2 = truck2.Capacity - client2.Demand;
            if (leftPlace1 < client2.Demand || leftPlace2 < client1.Demand)
            {
                throw new InvalidOperationException();
            }
            int i1 = truck1.Stages.IndexOf(client1);
            int i2 = truck2.Stages.IndexOf(client2);
            truck1.RemoveStage(client1);
            truck1.AddStage(client2, i1);
            truck2.RemoveStage(client2);
            truck2.AddStage(client1, i2);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    public void Reverse(Truck truck, int start = 0, int end = -1)
    {
        end = end == -1 ? truck.Stages.Count - 1 : end;
        for (int i = start; i <= end / 2; i++)
        {
            this.Exchange(truck.Stages[i], truck.Stages[end - i]);
        }
    }

    /// <summary>
    ///     Fais un 2-opt entre les 2 arêtes (start1, start1+1) et (start2, start2+1)
    /// </summary>
    public void Opt_2(Client start1, Client start2)
    {
        start1 = this.Clients.First((c) => c.Id == start1.Id);
        start2 = this.Clients.First((c) => c.Id == start2.Id);
        Truck? truck1 = this.Trucks.Find((t) => t.Stages.Contains(start1));
        Truck? truck2 = this.Trucks.Find((t) => t.Stages.Contains(start2));
        if (truck1 != null && truck2 != null)
        {
            int i1 = truck1.Stages.IndexOf(start1);
            int i2 = truck2.Stages.IndexOf(start2);
            IEnumerable<Client> toAdd1 = truck1.Stages.Skip(i1);
            IEnumerable<Client> toAdd2 = truck2.Stages.Skip(i2);

            int leftPlace1 = truck1.Capacity - truck1.Content;
            int leftPlace2 = truck2.Capacity - truck2.Content;

            if (leftPlace1 >= toAdd1.Select(c => c.Demand).Sum() && leftPlace2 >= toAdd2.Select(c => c.Demand).Sum())
            {
                truck1.Stages.RemoveRange(i1, truck1.Stages.Count - i1);
                truck2.Stages.RemoveRange(i2, truck2.Stages.Count - i2);

                truck1.Stages.AddRange(toAdd1);
                truck2.Stages.AddRange(toAdd2);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    public void CrossExchange(Client start1, int n1, Client start2, int n2)
    {
        start1 = this.Clients.First((c) => c.Id == start1.Id);
        start2 = this.Clients.First((c) => c.Id == start2.Id);
        Truck? truck1 = this.Trucks.Find((t) => t.Stages.Contains(start1));
        Truck? truck2 = this.Trucks.Find((t) => t.Stages.Contains(start2));
        if (truck1 != null && truck2 != null)
        {
            int i1 = truck1.Stages.IndexOf(start1);
            int i2 = truck2.Stages.IndexOf(start2);
            IEnumerable<Client> toAdd1 = truck1.Stages.Skip(i1).Take(n1);
            IEnumerable<Client> toAdd2 = truck2.Stages.Skip(i2).Take(n2);
            int leftPlace1 = truck1.Capacity - truck1.Content;
            int leftPlace2 = truck2.Capacity - truck2.Content;
            if (leftPlace1 >= toAdd1.Select(c => c.Demand).Sum() && leftPlace2 >= toAdd2.Select(c => c.Demand).Sum())
            {
                truck1.Stages.RemoveRange(i1, n1);
                truck2.Stages.RemoveRange(i2, n2);
                truck1.Stages.InsertRange(i1, toAdd2);
                truck2.Stages.InsertRange(i2, toAdd1);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        else
        {
            throw new InvalidOperationException();
        }
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
                    neighbour = ClassUtils.DeepClone(this);
                    foreach (Truck truck in neighbour.Trucks)
                    {
                        foreach (Client client in neighbour.Clients)
                        {
                            for (int i = 0; i <= truck.Stages.Count; i++)
                            {
                                try
                                {
                                    neighbour.Relocate(client, truck, i);
                                    if (neighbour.IsBetterThan(this))
                                    {
                                        Console.WriteLine("Relocate");
                                        return neighbour;
                                    }
                                }
                                catch (InvalidOperationException) { /* do nothing */ }
                            }
                        }
                    }
                    break;
                case NeighboursMethods.Exchange:
                    neighbour = ClassUtils.DeepClone(this);
                    foreach (Client client1 in neighbour.Clients)
                    {
                        foreach (Client client2 in neighbour.Clients)
                        {
                            try
                            {
                                neighbour.Exchange(client1, client2);
                                if (neighbour.IsBetterThan(this))
                                {
                                    Console.WriteLine("Exchange");
                                    return neighbour;
                                }
                            }
                            catch (InvalidOperationException) { /* do nothing */ }
                        }
                    }
                    break;
                case NeighboursMethods.Reverse:
                    neighbour = ClassUtils.DeepClone(this);
                    foreach (Truck truck in neighbour.Trucks)
                    {
                        for (int i = 0; i < truck.Stages.Count; i++)
                        {
                            for (int j = i; j < truck.Stages.Count; j++)
                            {
                                try
                                {
                                    neighbour.Reverse(truck, i, j);
                                    if (neighbour.IsBetterThan(this))
                                    {
                                        Console.WriteLine("Reverse");
                                        return neighbour;
                                    }
                                }
                                catch (InvalidOperationException) { /* do nothing */ }
                            }
                        }
                    }
                    break;
                case NeighboursMethods.Two_Opt:
                    neighbour = TwoOptOperator.Calculate(this);
                    break;
                case NeighboursMethods.CrossExchange:
                    neighbour = ClassUtils.DeepClone(this);
                    foreach (Client start1 in neighbour.Clients)
                    {
                        foreach (Client start2 in neighbour.Clients)
                        {
                            for (int i = 0; i < neighbour.Clients.Count; i++)
                            {
                                for (int j = 0; j < neighbour.Clients.Count; j++)
                                {
                                    try
                                    {
                                        neighbour.CrossExchange(start1, i, start2, j);
                                        if (neighbour.IsBetterThan(this))
                                        {
                                            Console.WriteLine("Cross-Exchange");
                                            return neighbour;
                                        }
                                    }
                                    catch (InvalidOperationException) { /* do nothing */ }
                                }
                            }
                        }
                    }
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
                neighbour = CheckAndDeleteEmptyTrucks(neighbour);
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