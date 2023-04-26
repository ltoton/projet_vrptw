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
        this.truckPanelGraphics = this.panel1.CreateGraphics();
        this.labelOffsetY = 0;
        displayWindowGraphics.Clear(Color.White);
        truckPanelGraphics.Clear(this.displayPanel.BackColor);

        this.graph.GenerateInitialSolution();
        this.DrawGraph(this.graph);

        Console.WriteLine(this.graph.Trucks.Count);
    }
}