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
        List<Client> clients = this.Clients.OrderBy(c => c.ReadyTime).ToList();
        Truck currentTruck = new() { Id = 0, Capacity = this.MaxQuantity, Depot = this.Depots[0] };
        while (clients.Any())
        {
            Client client = clients.First();
            if (currentTruck.Content + client.Demand > currentTruck.Capacity)
            {
                this.Trucks.Add(currentTruck);
                currentTruck = new() { Id = currentTruck.Id + 1, Capacity = MaxQuantity, Depot = this.Depots[0] };
                clients = clients.OrderBy(c => c.ReadyTime).ToList();
            }
            if (currentTruck.Stages.Count == 0 || currentTruck.Stages.Last().DueTime + GetDistanceBetweenClients(currentTruck.Stages.Last(), client) >= client.ReadyTime)
            {
                currentTruck.AddStage(client);
                clients.Remove(client);
                continue;
            }
            else if (currentTruck.Stages.Count > 0)
            {
                clients.Remove(client);
                clients.Add(client);
            }
        }
        this.Trucks.Add(currentTruck);
    }

    private int GetDistanceBetweenClients(Client client1, Client client2)
    {
        return (int)Math.Sqrt(Math.Pow(client1.X - client2.X, 2) + Math.Pow(client1.Y - client2.Y, 2));
    }

    public int GetTruckDistance(Truck truck)
    {
        int distance = 0;
        for (int i = 0; i < truck.Stages.Count - 1; i++)
        {
            distance += GetDistanceBetweenClients(truck.Stages[i], truck.Stages[i + 1]);
        }
        return distance;
    }

    public int GetTotalDistance()
    {
        return this.Trucks.Select(t => GetTruckDistance(t)).Sum();
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