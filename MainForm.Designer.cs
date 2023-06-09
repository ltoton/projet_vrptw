using System.Drawing.Drawing2D;
using VRPTW.Graph;
using VRPTW.Model;

namespace VRPTW;

partial class MainForm
{
    private const int STANDARD_OBJECT_WIDTH = 3;
    private const int STANDARD_LINE_WIDTH = 1;
    private int scaleFactor = 1;
    private List<Color> colorList = new List<Color>();
    private Dictionary<int, Color> colorDictionary = new Dictionary<int, Color>();
    private VrptwGraph graph;
    private Graphics displayWindowGraphics;
    private Graphics truckPanelGraphics;
    private int labelOffsetY = 0;

    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void DrawGraph(VrptwGraph graph)
    {
        // Set the scale factor according to the max value of clients and depots
        int max = 0;
        foreach (Client client in graph.Clients)
        {
            if (client.X > max)
            {
                max = client.X;
            }
            if (client.Y > max)
            {
                max = client.Y;
            }
        }
        foreach (Depot depot in graph.Depots)
        {
            if (depot.X > max)
            {
                max = depot.X;
            }
            if (depot.Y > max)
            {
                max = depot.Y;
            }
        }
        this.scaleFactor = this.displayPanel.Width / max;

        // Draw the depots
        foreach (Depot depot in graph.Depots)
        {
            this.DrawDepot(depot);
        }

        // Draw the roads
        int i = 0;
        foreach (List<string> road in graph.Roads)
        {
            Color color = this.GetNewRandomColor();
            this.DrawRoad(road, color);
            this.AppendTruckCaption(i, color);
            i++;
        }
    }

    private void DrawRoad(List<string> road, Color color)
    {
        foreach(string clientId in road)
        {
            Client client = this.graph.Clients.Find(c => c.Id == clientId);
            this.DrawClient(client, color);
        }
        // Draw first line
        this.DrawRoadSegment(graph.Depots[0], this.graph.Clients.Find(c => c.Id == road[0]), color);
        for (int i = 0; i < road.Count; i ++)
        {
            Client client1 = this.graph.Clients.Find(c => c.Id == road[i]);
            Client client2 = this.graph.Clients.Find(c => c.Id == road[(i + 1) % road.Count]);
            this.DrawRoadSegment(client1, client2, color);
        }
        this.DrawRoadSegment(this.graph.Clients.Find(c => c.Id == road[road.Count-1]), graph.Depots[0], color, true);
    }

    private void DrawRoadSegment(Vertex client1, Vertex client2, Color color, bool endPen = false)
    {
        Pen pen = new Pen(color, STANDARD_LINE_WIDTH);
        pen.CustomEndCap = endPen ? new AdjustableArrowCap(4, 4) : new AdjustableArrowCap(5, 5);
        this.displayWindowGraphics.DrawLine(
            pen,
            client1.X * this.scaleFactor,
            client1.Y * this.scaleFactor,
            client2.X * this.scaleFactor,
            client2.Y * this.scaleFactor);
    }

    private void DrawClient(Client client, Color color)
    {
        Pen pen = new Pen(color, STANDARD_OBJECT_WIDTH);
        Rectangle rectangle = new Rectangle(
            client.X * this.scaleFactor - STANDARD_OBJECT_WIDTH / 2,
            client.Y * this.scaleFactor - STANDARD_OBJECT_WIDTH / 2,
            STANDARD_OBJECT_WIDTH,
            STANDARD_OBJECT_WIDTH);
        this.displayWindowGraphics.DrawEllipse(pen, rectangle);
    }

    private void AppendTruckCaption(int road, Color color)
    {
        // Gets the length of the truck
        // TODO : Implement length of one road

        // Create a pen and an elipse to draw the truck information, then add it to the truck panel
        Pen pen = new Pen(color, STANDARD_OBJECT_WIDTH);
        Rectangle rectangle = new Rectangle(10, labelOffsetY + 10, STANDARD_OBJECT_WIDTH * 4, STANDARD_OBJECT_WIDTH * 4);

        String truckString = "Truck" + road + " - Length : TODO";
        Font font = new Font("Arial", 10);
        Brush brush = new SolidBrush(Color.Black);
        Point point = new Point(30, labelOffsetY + 8);

        this.truckPanelGraphics.DrawString(truckString, font, brush, point);
        this.truckPanelGraphics.DrawEllipse(pen, rectangle);

        labelOffsetY += 20;
    }

    private Color GetNewRandomColor()
    {
        // Sets a random color for the truck, this function has been extracted to be more readable
        Random random = new Random();
        Color randomColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        while (this.colorList.Contains(randomColor))
        {
            randomColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        }
        this.colorList.Add(randomColor);

        return randomColor;
    }

    private void DrawDepot(Depot depot)
    {
        Pen depotPen = new Pen(Color.Red, STANDARD_OBJECT_WIDTH);
        Rectangle rectangle = new Rectangle(
            depot.X * this.scaleFactor - STANDARD_OBJECT_WIDTH / 2,
            depot.Y * this.scaleFactor - STANDARD_OBJECT_WIDTH / 2,
            STANDARD_OBJECT_WIDTH * 2,
            STANDARD_OBJECT_WIDTH * 2);
        this.displayWindowGraphics.DrawEllipse(depotPen, rectangle);
    }

    private void DrawTotalLength(double totalDistance)
    {
        // Draws the total length of the solution
        String lengthString = "Total length : " + totalDistance;
        Font font = new Font("Arial", 10, FontStyle.Bold);
        Brush brush = new SolidBrush(Color.Black);
        Point point = new Point(30, labelOffsetY + 8);
        this.truckPanelGraphics.DrawString(lengthString, font, brush, point);
        labelOffsetY += 20;
    }

    private List<NeighboursMethods> GetListOfRelocator()
    {
        List<NeighboursMethods> relocators = new List<NeighboursMethods>();
        if (this.checkboxExchange.Checked)
        {
            relocators.Add(NeighboursMethods.Exchange);
        }
        if (this.checkboxRelocate.Checked)
        {
            relocators.Add(NeighboursMethods.Relocate);
        }
        if (this.checkboxReverse.Checked)
        {
            relocators.Add(NeighboursMethods.Reverse);
        }
        if (this.checkboxTwoOpt.Checked)
        {
            relocators.Add(NeighboursMethods.Two_Opt);
        }
        if (this.checkboxCrossExchange.Checked)
        {
            relocators.Add(NeighboursMethods.CrossExchange);
        }
        return relocators;
    }

    private void ClearCanvas()
    {
        this.labelOffsetY = 0;
        displayWindowGraphics.Clear(Color.White);
        truckPanelGraphics.Clear(this.displayPanel.BackColor);
    }

    /// <summary>
    ///  Get & set the list of available files
    /// </summary>
    protected string[] GetAvailableFiles()
    {
        string target = "./../../../Src/";
        return Directory.GetFiles(target, "*.vrp").Select(Path.GetFileName).ToArray();
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        dataSelect = new ComboBox();
        dataSelectLabel = new Label();
        importGraphButton = new Button();
        displayPanel = new Panel();
        flowLayoutPanel1 = new FlowLayoutPanel();
        panel1 = new Panel();
        panel2 = new Panel();
        generateOneSolutionButton = new Button();
        checkboxCrossExchange = new CheckBox();
        checkboxExchange = new CheckBox();
        checkboxReverse = new CheckBox();
        checkboxRelocate = new CheckBox();
        checkboxTwoOpt = new CheckBox();
        label1 = new Label();
        generationNbInput = new TextBox();
        generateNbSolutionsButton = new Button();
        flowLayoutPanel1.SuspendLayout();
        panel2.SuspendLayout();
        SuspendLayout();
        // 
        // dataSelect
        // 
        dataSelect.FormattingEnabled = true;
        dataSelect.Location = new Point(3, 17);
        dataSelect.Margin = new Padding(3, 2, 3, 2);
        dataSelect.Name = "dataSelect";
        dataSelect.Size = new Size(157, 23);
        dataSelect.TabIndex = 0;
        // 
        // dataSelectLabel
        // 
        dataSelectLabel.AutoSize = true;
        dataSelectLabel.Location = new Point(3, 0);
        dataSelectLabel.Name = "dataSelectLabel";
        dataSelectLabel.Size = new Size(144, 15);
        dataSelectLabel.TabIndex = 1;
        dataSelectLabel.Text = "Choix du jeu de données :";
        // 
        // importGraphButton
        // 
        importGraphButton.Location = new Point(3, 44);
        importGraphButton.Margin = new Padding(3, 2, 3, 2);
        importGraphButton.Name = "importGraphButton";
        importGraphButton.Size = new Size(157, 22);
        importGraphButton.TabIndex = 2;
        importGraphButton.Text = "Valider";
        importGraphButton.UseVisualStyleBackColor = true;
        importGraphButton.Click += ValidateButton_Click;
        // 
        // displayPanel
        // 
        displayPanel.Location = new Point(10, 9);
        displayPanel.Margin = new Padding(3, 2, 3, 2);
        displayPanel.Name = "displayPanel";
        displayPanel.Size = new Size(812, 812);
        displayPanel.TabIndex = 4;
        // 
        // flowLayoutPanel1
        // 
        flowLayoutPanel1.Controls.Add(dataSelectLabel);
        flowLayoutPanel1.Controls.Add(dataSelect);
        flowLayoutPanel1.Controls.Add(importGraphButton);
        flowLayoutPanel1.Location = new Point(830, 11);
        flowLayoutPanel1.Name = "flowLayoutPanel1";
        flowLayoutPanel1.Size = new Size(194, 71);
        flowLayoutPanel1.TabIndex = 5;
        // 
        // panel1
        // 
        panel1.Location = new Point(830, 213);
        panel1.Name = "panel1";
        panel1.Size = new Size(194, 606);
        panel1.TabIndex = 6;
        // 
        // panel2
        // 
        panel2.Controls.Add(generateOneSolutionButton);
        panel2.Controls.Add(checkboxCrossExchange);
        panel2.Controls.Add(checkboxExchange);
        panel2.Controls.Add(checkboxReverse);
        panel2.Controls.Add(checkboxRelocate);
        panel2.Controls.Add(checkboxTwoOpt);
        panel2.Controls.Add(label1);
        panel2.Controls.Add(generationNbInput);
        panel2.Controls.Add(generateNbSolutionsButton);
        panel2.Location = new Point(830, 88);
        panel2.Name = "panel2";
        panel2.Size = new Size(194, 119);
        panel2.TabIndex = 7;
        // 
        // generateOneSolutionButton
        // 
        generateOneSolutionButton.Location = new Point(3, 93);
        generateOneSolutionButton.Name = "generateOneSolutionButton";
        generateOneSolutionButton.Size = new Size(85, 23);
        generateOneSolutionButton.TabIndex = 8;
        generateOneSolutionButton.Text = "One step";
        generateOneSolutionButton.UseVisualStyleBackColor = true;
        generateOneSolutionButton.Click += generateOneSolution_Click;
        // 
        // checkboxCrossExchange
        // 
        checkboxCrossExchange.AutoSize = true;
        checkboxCrossExchange.Location = new Point(94, 43);
        checkboxCrossExchange.Name = "checkboxCrossExchange";
        checkboxCrossExchange.Size = new Size(106, 19);
        checkboxCrossExchange.TabIndex = 7;
        checkboxCrossExchange.Text = "CrossExchange";
        checkboxCrossExchange.UseVisualStyleBackColor = true;
        // 
        // checkboxExchange
        // 
        checkboxExchange.AutoSize = true;
        checkboxExchange.Location = new Point(94, 18);
        checkboxExchange.Name = "checkboxExchange";
        checkboxExchange.Size = new Size(77, 19);
        checkboxExchange.TabIndex = 6;
        checkboxExchange.Text = "Exchange";
        checkboxExchange.UseVisualStyleBackColor = true;
        // 
        // checkboxReverse
        // 
        checkboxReverse.AutoSize = true;
        checkboxReverse.Location = new Point(5, 68);
        checkboxReverse.Name = "checkboxReverse";
        checkboxReverse.Size = new Size(66, 19);
        checkboxReverse.TabIndex = 5;
        checkboxReverse.Text = "Reverse";
        checkboxReverse.UseVisualStyleBackColor = true;
        // 
        // checkboxRelocate
        // 
        checkboxRelocate.AutoSize = true;
        checkboxRelocate.Location = new Point(5, 43);
        checkboxRelocate.Name = "checkboxRelocate";
        checkboxRelocate.Size = new Size(71, 19);
        checkboxRelocate.TabIndex = 4;
        checkboxRelocate.Text = "Relocate";
        checkboxRelocate.UseVisualStyleBackColor = true;
        // 
        // checkboxTwoOpt
        // 
        checkboxTwoOpt.AutoSize = true;
        checkboxTwoOpt.Checked = true;
        checkboxTwoOpt.CheckState = CheckState.Checked;
        checkboxTwoOpt.Location = new Point(5, 18);
        checkboxTwoOpt.Name = "checkboxTwoOpt";
        checkboxTwoOpt.Size = new Size(67, 19);
        checkboxTwoOpt.TabIndex = 3;
        checkboxTwoOpt.Text = "TwoOpt";
        checkboxTwoOpt.UseVisualStyleBackColor = true;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(3, 0);
        label1.Name = "label1";
        label1.Size = new Size(145, 15);
        label1.TabIndex = 2;
        label1.Text = "Opérateurs de voisinages :";
        // 
        // generationNbInput
        // 
        generationNbInput.Location = new Point(94, 66);
        generationNbInput.Name = "generationNbInput";
        generationNbInput.Size = new Size(97, 23);
        generationNbInput.TabIndex = 1;
        generationNbInput.Text = "10";
        // 
        // generateNbSolutionsButton
        // 
        generateNbSolutionsButton.Location = new Point(94, 93);
        generateNbSolutionsButton.Name = "generateNbSolutionsButton";
        generateNbSolutionsButton.Size = new Size(97, 23);
        generateNbSolutionsButton.TabIndex = 0;
        generateNbSolutionsButton.Text = "Generate";
        generateNbSolutionsButton.UseVisualStyleBackColor = true;
        generateNbSolutionsButton.Click += generateNbSolutions_Click;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1034, 831);
        Controls.Add(panel2);
        Controls.Add(panel1);
        Controls.Add(flowLayoutPanel1);
        Controls.Add(displayPanel);
        Margin = new Padding(3, 2, 3, 2);
        Name = "MainForm";
        Text = "MainForm";
        flowLayoutPanel1.ResumeLayout(false);
        flowLayoutPanel1.PerformLayout();
        panel2.ResumeLayout(false);
        panel2.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private ComboBox dataSelect;
    private Label dataSelectLabel;
    private Button importGraphButton;
    private Panel displayPanel;
    private FlowLayoutPanel flowLayoutPanel1;
    private Panel panel1;
    private Panel panel2;
    private CheckedListBox checkListAgent;
    private Button generateNbSolutionsButton;
    private Label label1;
    private TextBox generationNbInput;
    private CheckBox checkboxCrossExchange;
    private CheckBox checkboxExchange;
    private CheckBox checkboxReverse;
    private CheckBox checkboxRelocate;
    private CheckBox checkboxTwoOpt;
    private Button generateOneSolutionButton;
}