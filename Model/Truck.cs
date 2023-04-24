namespace VRPTW.Model;

public class Truck
{
    public int Id { get; set; }

    public int Capacity { get; set; }

    public int Content { get; set; } = 0;

    public List<Client> Stages { get; set; } = new();

    public Depot Depot { get; set; } = new();

    public void AddStage(Client c, int i = -1)
    {
        if (i >= 0 && Stages.Count > i)
        {
            Stages.Insert(i, c);
            var dest = c.Edges.Find((e) => e.Destination.Id == Stages.ElementAt(i + 1).Id);
            if (dest != null)
            {
                dest.Destination = c;
            }
            c.Edges.Find((e) => e.Destination.Id == Stages.ElementAt(i + 1).Id);
        }
        else
        {
            Stages.Add(c);
            Content += c.Demand;
        }
    }

    public void RemoveStage(Client c)
    {
        Stages.Remove(c);
        Content -= c.Demand;
    }
}
