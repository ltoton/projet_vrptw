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
        this.displayWindowGraphics = this.displayPanel.CreateGraphics();
        displayWindowGraphics.Clear(Color.White);

        this.graph.GenerateInitialSolution();
        this.DrawGraph(this.graph);

        Console.WriteLine(this.graph.Trucks.Count);
    }
}