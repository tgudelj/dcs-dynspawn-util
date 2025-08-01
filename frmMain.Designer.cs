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
        lbMizGroups = new ListBox();
        label1 = new Label();
        lbApplyTo = new ListBox();
        lblApplyToTypes = new Label();
        btnApply = new Button();
        btnSelectAll = new Button();
        btnClear = new Button();
        lblSelectedGroupInfo = new Label();
        txtGroupsFilter = new TextBox();
        btnClearGroupsFilter = new Button();
        txtApplyToFilter = new TextBox();
        btnClearApplyToFilter = new Button();
        lblTemplateGroupDescription = new Label();
        menuMain.SuspendLayout();
        SuspendLayout();
        // 
        // menuMain
        // 
        menuMain.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
        menuMain.Location = new Point(0, 0);
        menuMain.Name = "menuMain";
        menuMain.Size = new Size(911, 24);
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
        // lbMizGroups
        // 
        lbMizGroups.DrawMode = DrawMode.OwnerDrawFixed;
        lbMizGroups.Font = new Font("Segoe UI", 10F);
        lbMizGroups.FormattingEnabled = true;
        lbMizGroups.ItemHeight = 17;
        lbMizGroups.Location = new Point(19, 121);
        lbMizGroups.Name = "lbMizGroups";
        lbMizGroups.Size = new Size(567, 327);
        lbMizGroups.Sorted = true;
        lbMizGroups.TabIndex = 1;
        lbMizGroups.DrawItem += lbMizGroups_DrawItem;
        lbMizGroups.SelectedIndexChanged += lbMizGroups_SelectedIndexChanged;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(19, 59);
        label1.Name = "label1";
        label1.Size = new Size(102, 15);
        label1.TabIndex = 2;
        label1.Text = "Groups in mission";
        // 
        // lbApplyTo
        // 
        lbApplyTo.Font = new Font("Segoe UI", 10F);
        lbApplyTo.FormattingEnabled = true;
        lbApplyTo.ItemHeight = 17;
        lbApplyTo.Location = new Point(618, 121);
        lbApplyTo.Name = "lbApplyTo";
        lbApplyTo.SelectionMode = SelectionMode.MultiExtended;
        lbApplyTo.Size = new Size(282, 327);
        lbApplyTo.Sorted = true;
        lbApplyTo.TabIndex = 4;
        lbApplyTo.SelectedIndexChanged += lbApplyTo_SelectedIndexChanged;
        // 
        // lblApplyToTypes
        // 
        lblApplyToTypes.AutoSize = true;
        lblApplyToTypes.Location = new Point(618, 24);
        lblApplyToTypes.Name = "lblApplyToTypes";
        lblApplyToTypes.Size = new Size(133, 15);
        lblApplyToTypes.TabIndex = 4;
        lblApplyToTypes.Text = "Apply template to types";
        // 
        // btnApply
        // 
        btnApply.Font = new Font("Segoe UI", 10F);
        btnApply.Location = new Point(797, 500);
        btnApply.Name = "btnApply";
        btnApply.Size = new Size(103, 33);
        btnApply.TabIndex = 6;
        btnApply.Text = "Apply";
        btnApply.UseVisualStyleBackColor = true;
        btnApply.Click += btnApply_Click;
        // 
        // btnSelectAll
        // 
        btnSelectAll.Location = new Point(618, 51);
        btnSelectAll.Name = "btnSelectAll";
        btnSelectAll.Size = new Size(113, 23);
        btnSelectAll.TabIndex = 2;
        btnSelectAll.Text = "SELECT ALL";
        btnSelectAll.UseVisualStyleBackColor = true;
        btnSelectAll.Click += btnSelectAll_Click;
        // 
        // btnClear
        // 
        btnClear.Location = new Point(825, 51);
        btnClear.Name = "btnClear";
        btnClear.Size = new Size(75, 23);
        btnClear.TabIndex = 3;
        btnClear.Text = "CLEAR";
        btnClear.UseVisualStyleBackColor = true;
        btnClear.Click += btnClear_Click;
        // 
        // lblSelectedGroupInfo
        // 
        lblSelectedGroupInfo.Font = new Font("Segoe UI", 12F);
        lblSelectedGroupInfo.Location = new Point(19, 471);
        lblSelectedGroupInfo.Name = "lblSelectedGroupInfo";
        lblSelectedGroupInfo.Size = new Size(567, 65);
        lblSelectedGroupInfo.TabIndex = 9;
        // 
        // txtGroupsFilter
        // 
        txtGroupsFilter.Location = new Point(19, 91);
        txtGroupsFilter.Name = "txtGroupsFilter";
        txtGroupsFilter.Size = new Size(528, 23);
        txtGroupsFilter.TabIndex = 10;
        txtGroupsFilter.KeyUp += txtGroupsFilter_KeyUp;
        // 
        // btnClearGroupsFilter
        // 
        btnClearGroupsFilter.Location = new Point(554, 91);
        btnClearGroupsFilter.Name = "btnClearGroupsFilter";
        btnClearGroupsFilter.Size = new Size(32, 23);
        btnClearGroupsFilter.TabIndex = 11;
        btnClearGroupsFilter.Text = "X";
        btnClearGroupsFilter.UseVisualStyleBackColor = true;
        btnClearGroupsFilter.Click += btnClearGroupsFilter_Click;
        // 
        // txtApplyToFilter
        // 
        txtApplyToFilter.Location = new Point(618, 91);
        txtApplyToFilter.Name = "txtApplyToFilter";
        txtApplyToFilter.Size = new Size(243, 23);
        txtApplyToFilter.TabIndex = 12;
        txtApplyToFilter.KeyUp += txtApplyToFilter_KeyUp;
        // 
        // btnClearApplyToFilter
        // 
        btnClearApplyToFilter.Location = new Point(867, 91);
        btnClearApplyToFilter.Name = "btnClearApplyToFilter";
        btnClearApplyToFilter.Size = new Size(32, 23);
        btnClearApplyToFilter.TabIndex = 13;
        btnClearApplyToFilter.Text = "X";
        btnClearApplyToFilter.UseVisualStyleBackColor = true;
        btnClearApplyToFilter.Click += btnClearApplyToFilter_Click;
        // 
        // lblTemplateGroupDescription
        // 
        lblTemplateGroupDescription.AutoSize = true;
        lblTemplateGroupDescription.Location = new Point(19, 29);
        lblTemplateGroupDescription.Name = "lblTemplateGroupDescription";
        lblTemplateGroupDescription.Size = new Size(508, 15);
        lblTemplateGroupDescription.TabIndex = 14;
        lblTemplateGroupDescription.Text = "Waypoints of selected group will be used to create dynamic spawn templates for selected types";
        // 
        // frmMain
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(911, 545);
        Controls.Add(lblTemplateGroupDescription);
        Controls.Add(btnClearApplyToFilter);
        Controls.Add(txtApplyToFilter);
        Controls.Add(btnClearGroupsFilter);
        Controls.Add(txtGroupsFilter);
        Controls.Add(lblSelectedGroupInfo);
        Controls.Add(btnClear);
        Controls.Add(btnSelectAll);
        Controls.Add(btnApply);
        Controls.Add(lblApplyToTypes);
        Controls.Add(lbApplyTo);
        Controls.Add(label1);
        Controls.Add(lbMizGroups);
        Controls.Add(menuMain);
        MainMenuStrip = menuMain;
        Name = "frmMain";
        Text = "Dynamic Spawn Template Helper 0.1.4";
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
    private ListBox lbMizGroups;
    private Label label1;
    private ListBox lbApplyTo;
    private Label lblApplyToTypes;
    private Button btnApply;
    private Button btnSelectAll;
    private Button btnClear;
    private Label lblSelectedGroupInfo;
    private TextBox txtGroupsFilter;
    private Button btnClearGroupsFilter;
    private TextBox txtApplyToFilter;
    private Button btnClearApplyToFilter;
    private Label lblTemplateGroupDescription;
}