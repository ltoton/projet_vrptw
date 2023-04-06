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
        this.graph = GraphReader.ReadVrptw("../../../Src/data101.vrp");
    }
}