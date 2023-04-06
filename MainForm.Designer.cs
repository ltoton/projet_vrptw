using VRPTW.Model;

namespace VRPTW;

partial class MainForm
{
    private VrptwGraph graph;
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
        dataSelect.Location = new Point(594, 53);
        dataSelect.Name = "dataSelect";
        dataSelect.Size = new Size(179, 28);
        dataSelect.TabIndex = 0;
        // 
        // dataSelectLabel
        // 
        dataSelectLabel.AutoSize = true;
        dataSelectLabel.Location = new Point(594, 30);
        dataSelectLabel.Name = "dataSelectLabel";
        dataSelectLabel.Size = new Size(179, 20);
        dataSelectLabel.TabIndex = 1;
        dataSelectLabel.Text = "Choix du jeu de données :";
        // 
        // ValidateButton
        // 
        ValidateButton.Location = new Point(594, 87);
        ValidateButton.Name = "ValidateButton";
        ValidateButton.Size = new Size(179, 29);
        ValidateButton.TabIndex = 2;
        ValidateButton.Text = "Valider";
        ValidateButton.UseVisualStyleBackColor = true;
        ValidateButton.Click += ValidateButton_Click;
        // 
        // displayPanel
        // 
        displayPanel.Location = new Point(12, 12);
        displayPanel.Name = "displayPanel";
        displayPanel.Size = new Size(576, 426);
        displayPanel.TabIndex = 4;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(displayPanel);
        Controls.Add(ValidateButton);
        Controls.Add(dataSelectLabel);
        Controls.Add(dataSelect);
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