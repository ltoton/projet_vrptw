using VRPTW.Model;
using VRPTW.Services;

namespace VRPTW;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        this.dataSelect.DataSource = this.GetAvailableFiles();
    }

    private void ValidateButton_Click(object sender, EventArgs e)
    {
        this.graph = GraphReader.ReadVrptw($"../../../Src/{this.dataSelect.SelectedValue}");
        var solution = this.graph.GenerateInitialSolution();
        Console.WriteLine(solution.Trucks.Count);
    }
}