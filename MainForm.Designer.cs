using System.Drawing.Drawing2D;
using VRPTW.Model;

namespace VRPTW;

partial class MainForm
{
    private const int STANDARD_OBJECT_WIDTH = 3;
    private const int STANDARD_LINE_WIDTH = 1;
    private int scaleFactor = 1;
    private List<Color> colors = new List<Color>();
    private VrptwGraph graph;
    private Graphics displayWindowGraphics;
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
        // Draw the lines for the trucks and the clients
        foreach (Truck truck in graph.Trucks)
        {
            this.DrawLineBetweenClient(truck.Stages, truck.Depot);
        }
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

    private void DrawClient(Client client, Color color)
    {
        Pen clientPen = new Pen(color, STANDARD_OBJECT_WIDTH);
        Rectangle rectangle = new Rectangle(
            client.X * this.scaleFactor - STANDARD_OBJECT_WIDTH / 2,
            client.Y * this.scaleFactor - STANDARD_OBJECT_WIDTH / 2,
            STANDARD_OBJECT_WIDTH,
            STANDARD_OBJECT_WIDTH);
        this.displayWindowGraphics.DrawEllipse(clientPen, rectangle);
    }

    private void DrawLineBetweenClient(List<Client> stages, Depot depot)
    {
        // get a random color and add it to the list, if it's not already in the list
        Random random = new Random();
        Color randomColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        while (this.colors.Contains(randomColor))
        {
            randomColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        }
        this.colors.Add(randomColor);
        Pen linePen = new Pen(randomColor, STANDARD_LINE_WIDTH);
        linePen.CustomEndCap = new AdjustableArrowCap(5, 5);
        this.displayWindowGraphics.DrawLine(linePen, depot.X * this.scaleFactor, depot.Y * this.scaleFactor, stages[0].X * this.scaleFactor, stages[0].Y * this.scaleFactor);
        for (int i = 0; i < stages.Count - 1; i++)
        {
            this.DrawClient(stages[i], randomColor);
            this.displayWindowGraphics.DrawLine(linePen, stages[i].X * this.scaleFactor, stages[i].Y * this.scaleFactor, stages[i + 1].X * this.scaleFactor, stages[i + 1].Y * this.scaleFactor);
        }
        this.DrawClient(stages[stages.Count - 1], randomColor);
        this.displayWindowGraphics.DrawLine(linePen, depot.X * this.scaleFactor, depot.Y * this.scaleFactor, stages[stages.Count - 1].X * this.scaleFactor, stages[stages.Count - 1].Y * this.scaleFactor);

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
        ValidateButton = new Button();
        displayPanel = new Panel();
        SuspendLayout();
        // 
        // dataSelect
        // 
        dataSelect.FormattingEnabled = true;
        dataSelect.Location = new Point(828, 27);
        dataSelect.Margin = new Padding(3, 2, 3, 2);
        dataSelect.Name = "dataSelect";
        dataSelect.Size = new Size(157, 23);
        dataSelect.TabIndex = 0;
        // 
        // dataSelectLabel
        // 
        dataSelectLabel.AutoSize = true;
        dataSelectLabel.Location = new Point(828, 9);
        dataSelectLabel.Name = "dataSelectLabel";
        dataSelectLabel.Size = new Size(144, 15);
        dataSelectLabel.TabIndex = 1;
        dataSelectLabel.Text = "Choix du jeu de données :";
        // 
        // ValidateButton
        // 
        ValidateButton.Location = new Point(828, 52);
        ValidateButton.Margin = new Padding(3, 2, 3, 2);
        ValidateButton.Name = "ValidateButton";
        ValidateButton.Size = new Size(157, 22);
        ValidateButton.TabIndex = 2;
        ValidateButton.Text = "Valider";
        ValidateButton.UseVisualStyleBackColor = true;
        ValidateButton.Click += ValidateButton_Click;
        // 
        // displayPanel
        // 
        displayPanel.Location = new Point(10, 9);
        displayPanel.Margin = new Padding(3, 2, 3, 2);
        displayPanel.Name = "displayPanel";
        displayPanel.Size = new Size(812, 812);
        displayPanel.TabIndex = 4;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(984, 831);
        Controls.Add(displayPanel);
        Controls.Add(ValidateButton);
        Controls.Add(dataSelectLabel);
        Controls.Add(dataSelect);
        Margin = new Padding(3, 2, 3, 2);
        Name = "MainForm";
        Text = "MainForm";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private ComboBox dataSelect;
    private Label dataSelectLabel;
    private Button ValidateButton;
    private Panel displayPanel;
}