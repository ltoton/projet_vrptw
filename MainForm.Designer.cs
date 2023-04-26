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
        // Draw the lines for the trucks and the clients
        foreach (Truck truck in graph.Trucks)
        {
            Color color = this.GetNewRandomColor();
            this.DrawLineBetweenClient(truck.Stages, truck.Depot, color);
            this.AppendTruckCaption(truck, color);
        }
    }

    private Color GetNewRandomColor()
    {
        // Sets a random color for the truck, this function has been extracted to be more readable
        Random random = new Random();
        Color randomColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        while (this.colors.Contains(randomColor))
        {
            randomColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        }
        this.colors.Add(randomColor);

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

    private void DrawLineBetweenClient(List<Client> stages, Depot depot, Color randomColor)
    {
        Pen linePen = new Pen(randomColor, STANDARD_LINE_WIDTH);
        
        // Ads an arrow to the end of the line
        linePen.CustomEndCap = new AdjustableArrowCap(5, 5);

        // Draw the first line from the depot to the first client
        this.displayWindowGraphics.DrawLine(
            linePen, 
            depot.X * this.scaleFactor, 
            depot.Y * this.scaleFactor,
            stages[0].X * this.scaleFactor, 
            stages[0].Y * this.scaleFactor
            );
        
        // Draw each line between the clients and the clients
        for (int i = 0; i < stages.Count - 1; i++)
        {
            this.DrawClient(stages[i], randomColor);
            this.displayWindowGraphics.DrawLine(
                linePen, 
                stages[i].X * this.scaleFactor, 
                stages[i].Y * this.scaleFactor, 
                stages[i + 1].X * this.scaleFactor, 
                stages[i + 1].Y * this.scaleFactor
                );
        }
        // Draw the last client
        this.DrawClient(stages[stages.Count - 1], randomColor);

        // Draw the last line from the last client to the depot
        this.displayWindowGraphics.DrawLine(
            linePen, 
            depot.X * this.scaleFactor, 
            depot.Y * this.scaleFactor, 
            stages[stages.Count - 1].X * this.scaleFactor, 
            stages[stages.Count - 1].Y * this.scaleFactor
            );
    }

    private void AppendTruckCaption(Truck truck, Color color)
    {
        // Gets the length of the truck
        int truckDistance = this.graph.GetTruckDistance(truck);

        // Create a pen and an elipse to draw the truck information, then add it to the truck panel
        Pen pen = new Pen(color, STANDARD_OBJECT_WIDTH);
        Rectangle rectangle = new Rectangle(10, labelOffsetY + 10, STANDARD_OBJECT_WIDTH * 4, STANDARD_OBJECT_WIDTH * 4);
        
        String truckString = "Truck" + truck.Id + " - Length : " + this.graph.GetTruckDistance(truck);
        Font font = new Font("Arial", 10);
        Brush brush = new SolidBrush(Color.Black);
        Point point = new Point(30, labelOffsetY + 8);
        
        this.truckPanelGraphics.DrawString(truckString, font, brush, point);
        this.truckPanelGraphics.DrawEllipse(pen, rectangle);
        
        labelOffsetY += 20;
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
        flowLayoutPanel1 = new FlowLayoutPanel();
        panel1 = new Panel();
        flowLayoutPanel1.SuspendLayout();
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
        // ValidateButton
        // 
        ValidateButton.Location = new Point(3, 44);
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
        // flowLayoutPanel1
        // 
        flowLayoutPanel1.Controls.Add(dataSelectLabel);
        flowLayoutPanel1.Controls.Add(dataSelect);
        flowLayoutPanel1.Controls.Add(ValidateButton);
        flowLayoutPanel1.Location = new Point(830, 11);
        flowLayoutPanel1.Name = "flowLayoutPanel1";
        flowLayoutPanel1.Size = new Size(194, 71);
        flowLayoutPanel1.TabIndex = 5;
        // 
        // panel1
        // 
        panel1.Location = new Point(830, 107);
        panel1.Name = "panel1";
        panel1.Size = new Size(194, 476);
        panel1.TabIndex = 6;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1034, 831);
        Controls.Add(panel1);
        Controls.Add(flowLayoutPanel1);
        Controls.Add(displayPanel);
        Margin = new Padding(3, 2, 3, 2);
        Name = "MainForm";
        Text = "MainForm";
        flowLayoutPanel1.ResumeLayout(false);
        flowLayoutPanel1.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private ComboBox dataSelect;
    private Label dataSelectLabel;
    private Button ValidateButton;
    private Panel displayPanel;
    private FlowLayoutPanel flowLayoutPanel1;
    private Panel panel1;
}