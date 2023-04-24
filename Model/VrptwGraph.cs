namespace VRPTW.Model;

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

    public void GenerateInitialSolution()
    {
        var clientEnumerator = this.Clients.OrderBy(a => Guid.NewGuid()).ToList().GetEnumerator();
        Truck currentTruck = new() { Id = 0, Capacity = this.MaxQuantity, Depot = this.Depots[0] };
        while (clientEnumerator.MoveNext())
        {
            Client nextClient = clientEnumerator.Current;
            if (currentTruck.Content + nextClient.Demand > currentTruck.Capacity)
            {
                this.Trucks.Add(currentTruck);
                currentTruck = new() { Id = currentTruck.Id + 1, Capacity = MaxQuantity, Depot = this.Depots[0] };
            }
            currentTruck.AddStage(nextClient);
        }
        this.Trucks.Add(currentTruck);
    }

    public void Switch(Client client1, Client client2)
    {
        Truck? truck1 = this.Trucks.Find((t) => t.Stages.Contains(client1));
        Truck? truck2 = this.Trucks.Find((t) => t.Stages.Contains(client2));
        if (truck1 != null && truck2 != null && client1.Id != client2.Id)
        {
            int leftPlace1 = truck1.Capacity - client1.Demand;
            int leftPlace2 = truck2.Capacity - client2.Demand;
            if (leftPlace1 < client2.Demand || leftPlace2 < client1.Demand)
            {
                return;
            }
            int i1 = truck1.Stages.IndexOf(client1);
            int i2 = truck2.Stages.IndexOf(client2);
            truck1.RemoveStage(client1);
            truck1.AddStage(client2, i1);
            truck2.RemoveStage(client2);
            truck2.AddStage(client1, i2);
        }
    }

    public void InsertShift(Client client, Truck destTruck, int pos)
    {
        Truck? truck = this.Trucks.Find((t) => t.Stages.Contains(client));
        if (truck != null)
        {
            int leftPlace = destTruck.Capacity - destTruck.Content;
            if (leftPlace < client.Demand)
            {
                return;
            }
            truck.RemoveStage(client);
            destTruck.AddStage(client, pos);
        }
    }

    public void Inversion(Truck truck, int start, int end)
    {
        for (int i = start; i <= end / 2; i++)
        {
            this.Switch(truck.Stages[i], truck.Stages[end - i]);
        }
    }

    /// <summary>
    ///     Fais un 2-opt entre les 2 arêtes (start1, start1+1) et (start2, start2+1)
    /// </summary>
    public void Opt_2(Client start1, Client start2)
    {
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
        }

    }
}