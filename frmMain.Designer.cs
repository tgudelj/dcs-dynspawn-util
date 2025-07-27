namespace DCSDynamicTemplateHelper;

partial class frmMain {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
        if (disposing && (components != null)) {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
        menuMain = new MenuStrip();
        fileToolStripMenuItem = new ToolStripMenuItem();
        miOpenMission = new ToolStripMenuItem();
        dlgOpenFile = new OpenFileDialog();
        lbFoundGroups = new ListBox();
        label1 = new Label();
        lbApplyTo = new ListBox();
        lbApplyToTypes = new Label();
        btnCancel = new Button();
        btnApply = new Button();
        btnSelectAll = new Button();
        button1 = new Button();
        menuMain.SuspendLayout();
        SuspendLayout();
        // 
        // menuMain
        // 
        menuMain.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
        menuMain.Location = new Point(0, 0);
        menuMain.Name = "menuMain";
        menuMain.Size = new Size(565, 24);
        menuMain.TabIndex = 0;
        menuMain.Text = "menuStrip1";
        // 
        // fileToolStripMenuItem
        // 
        fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { miOpenMission });
        fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        fileToolStripMenuItem.Size = new Size(37, 20);
        fileToolStripMenuItem.Text = "File";
        // 
        // miOpenMission
        // 
        miOpenMission.Name = "miOpenMission";
        miOpenMission.Size = new Size(172, 22);
        miOpenMission.Text = "Open DCS Mission";
        miOpenMission.Click += miOpenMission_Click;
        // 
        // dlgOpenFile
        // 
        dlgOpenFile.Filter = "DCS World Mission file | *.miz";
        // 
        // lbFoundGroups
        // 
        lbFoundGroups.FormattingEnabled = true;
        lbFoundGroups.ItemHeight = 15;
        lbFoundGroups.Location = new Point(19, 57);
        lbFoundGroups.Name = "lbFoundGroups";
        lbFoundGroups.Size = new Size(245, 409);
        lbFoundGroups.TabIndex = 1;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(20, 32);
        label1.Name = "label1";
        label1.Size = new Size(81, 15);
        label1.TabIndex = 2;
        label1.Text = "Found groups";
        // 
        // lbApplyTo
        // 
        lbApplyTo.FormattingEnabled = true;
        lbApplyTo.ItemHeight = 15;
        lbApplyTo.Location = new Point(305, 87);
        lbApplyTo.Name = "lbApplyTo";
        lbApplyTo.SelectionMode = SelectionMode.MultiExtended;
        lbApplyTo.Size = new Size(242, 379);
        lbApplyTo.Sorted = true;
        lbApplyTo.TabIndex = 2;
        // 
        // lbApplyToTypes
        // 
        lbApplyToTypes.AutoSize = true;
        lbApplyToTypes.Location = new Point(305, 26);
        lbApplyToTypes.Name = "lbApplyToTypes";
        lbApplyToTypes.Size = new Size(133, 15);
        lbApplyToTypes.TabIndex = 4;
        lbApplyToTypes.Text = "Apply template to types";
        // 
        // btnCancel
        // 
        btnCancel.Font = new Font("Segoe UI", 10F);
        btnCancel.Location = new Point(343, 481);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(95, 33);
        btnCancel.TabIndex = 5;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;
        // 
        // btnApply
        // 
        btnApply.Font = new Font("Segoe UI", 10F);
        btnApply.Location = new Point(444, 481);
        btnApply.Name = "btnApply";
        btnApply.Size = new Size(103, 33);
        btnApply.TabIndex = 6;
        btnApply.Text = "Apply";
        btnApply.UseVisualStyleBackColor = true;
        // 
        // btnSelectAll
        // 
        btnSelectAll.Location = new Point(305, 51);
        btnSelectAll.Name = "btnSelectAll";
        btnSelectAll.Size = new Size(113, 23);
        btnSelectAll.TabIndex = 7;
        btnSelectAll.Text = "SELECT ALL";
        btnSelectAll.UseVisualStyleBackColor = true;
        // 
        // button1
        // 
        button1.Location = new Point(472, 51);
        button1.Name = "button1";
        button1.Size = new Size(75, 23);
        button1.TabIndex = 8;
        button1.Text = "CLEAR";
        button1.UseVisualStyleBackColor = true;
        // 
        // frmMain
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(565, 545);
        Controls.Add(button1);
        Controls.Add(btnSelectAll);
        Controls.Add(btnApply);
        Controls.Add(btnCancel);
        Controls.Add(lbApplyToTypes);
        Controls.Add(lbApplyTo);
        Controls.Add(label1);
        Controls.Add(lbFoundGroups);
        Controls.Add(menuMain);
        MainMenuStrip = menuMain;
        Name = "frmMain";
        Text = "frmMain";
        Load += frmMain_Load;
        menuMain.ResumeLayout(false);
        menuMain.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip menuMain;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem miOpenMission;
    private OpenFileDialog dlgOpenFile;
    private ListBox lbFoundGroups;
    private Label label1;
    private ListBox lbApplyTo;
    private Label lbApplyToTypes;
    private Button btnCancel;
    private Button btnApply;
    private Button btnSelectAll;
    private Button button1;
}