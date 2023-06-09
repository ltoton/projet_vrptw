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
        this.ClearCanvas();
        this.graph.GenerateInitialSolution();
        this.DrawGraph(this.graph);
    }

    private void generateOneSolution_Click(object sender, EventArgs e)
    {
        this.generateOneSolutionButton.Enabled = false;
        this.generateNbSolutionsButton.Enabled = false;
        this.graph = VrptwGraph.HillClimbing(this.graph, this.GetListOfRelocator(), 1);
        this.ClearCanvas();
        this.DrawGraph(this.graph, false);
        this.generateNbSolutionsButton.Enabled = true;
        this.generateOneSolutionButton.Enabled = true;
    }

    private void generateNbSolutions_Click(object sender, EventArgs e)
    {
        this.generateOneSolutionButton.Enabled = false;
        this.generateNbSolutionsButton.Enabled = false;
        this.graph = VrptwGraph.HillClimbing(this.graph, this.GetListOfRelocator(), Int16.Parse(generationNbInput.Text));
        this.ClearCanvas();
        this.DrawGraph(this.graph, false);
        this.generateNbSolutionsButton.Enabled = true;
        this.generateOneSolutionButton.Enabled = true;
    }
}