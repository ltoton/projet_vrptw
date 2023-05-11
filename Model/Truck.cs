using VRPTW.Graph;

namespace VRPTW.Model;

[Serializable]
public class Truck
{
    public int Id { get; set; }

    public int Capacity { get; set; }

    public int Content { get; set; } = 0;

    public List<Client> Stages { get; set; } = new();

    public Depot Depot { get; set; } = new();

    public Vertex LastStage => (Vertex?)Stages.LastOrDefault() ?? Depot;

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
        c = Stages.First((e) => e.Id == c.Id);
        Stages.Remove(c);
        Content -= c.Demand;
    }

    public int GetDistance()
    {
        int distance = this.Depot.GetDistanceWith(this.Stages.FirstOrDefault());
        for (int i = 0; i < this.Stages.Count - 1; i++)
        {
            distance += this.Stages[i].GetDistanceWith(this.Stages[i + 1]);
        }
        return distance;
    }

    public int GetDuration()
    {
        int duration = this.Depot.GetDistanceWith(this.Stages.FirstOrDefault());
        for (int i = 0; i < this.Stages.Count - 1; i++)
        {
            int nextDuration = this.Stages[i].GetDistanceWith(this.Stages[i + 1]);
            if (duration + nextDuration < this.Stages[i].ReadyTime)
            {
                duration = this.Stages[i].ReadyTime;
            }
            duration += nextDuration;
        }
        return duration;
    }
}
