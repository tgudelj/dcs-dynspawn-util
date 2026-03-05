using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace DCSDynamicTemplateHelper;

public partial class frmMain : Form {
    private readonly MissionTemplateService _missionService = new();
    private AppSettings _settings = new();
    private string? _missionFilePath;
    private List<DCSTemplateGroupInfo> GroupsInMission = new();
    private MissionData? _loadedMission;
    internal BindingList<DCSTemplateGroupInfo> _groups = new();
    internal BindingList<DCSTypeInfo> _aircraftTypes = new();
    private DCSTemplateGroupInfo? SelectedTemplateGroup;
    private int _previousSelectedIndex = -1;

    public frmMain() {
        InitializeComponent();
    }

    #region ------------- Event handlers -------------

    private void frmMain_Load(object sender, EventArgs e) {
        string cultureName = Thread.CurrentThread.CurrentCulture.Name;
        CultureInfo ci = new(cultureName);
        if (ci.NumberFormat.NumberDecimalSeparator != ".") {
            ci.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = ci;
        }

        _settings = Program.Configuration.GetSection("Settings").Get<AppSettings>() ?? new AppSettings();
        foreach (DCSTypeInfo item in _settings.Flyable) {
            _aircraftTypes.Add(item);
        }
        lbApplyTo.DataSource = _aircraftTypes;
        lbApplyTo.DisplayMember = "DisplayName";
        lbApplyTo.SelectedIndex = -1;

        lbMizGroups.DataSource = _groups;
        lbMizGroups.DisplayMember = "DisplayName";
        lbMizGroups.SelectedIndex = -1;
    }

    private void btnSelectAll_Click(object sender, EventArgs e) {
        for (int i = 0; i < lbApplyTo.Items.Count; i++) {
            lbApplyTo.SetSelected(i, true);
        }
    }

    private void btnClear_Click(object sender, EventArgs e) {
        lbApplyTo.SelectedIndex = -1;
    }

    private void btnClearGroupsFilter_Click(object sender, EventArgs e) {
        txtGroupsFilter.Text = "";
        _groups.Clear();
        foreach (DCSTemplateGroupInfo g in GroupsInMission) {
            _groups.Add(g);
        }
    }

    private void btnClearApplyToFilter_Click(object sender, EventArgs e) {
        txtApplyToFilter.Text = "";
        _aircraftTypes.Clear();
        foreach (DCSTypeInfo item in _settings.Flyable) {
            _aircraftTypes.Add(item);
        }
    }

    private void txtGroupsFilter_KeyUp(object sender, KeyEventArgs e) {
        string searchString = txtGroupsFilter.Text;
        _groups.Clear();
        if (searchString.Length < 2) {
            foreach (DCSTemplateGroupInfo group in GroupsInMission) {
                _groups.Add(group);
            }
            return;
        }

        foreach (DCSTemplateGroupInfo g in GroupsInMission) {
            if (g.GroupName.ToLower().Contains(searchString.ToLower()) || g.DCSVehicleType.ToLower().Contains(searchString.ToLower())) {
                _groups.Add(g);
            }
        }
    }

    private void txtApplyToFilter_KeyUp(object sender, KeyEventArgs e) {
        string searchString = txtApplyToFilter.Text;
        _aircraftTypes.Clear();
        if (searchString.Length < 2) {
            foreach (DCSTypeInfo item in _settings.Flyable) {
                _aircraftTypes.Add(item);
            }
            return;
        }

        foreach (DCSTypeInfo t in _settings.Flyable) {
            if (t.DisplayName.ToLower().Contains(searchString.ToLower())) {
                _aircraftTypes.Add(t);
            }
        }
    }

    private void lbMizGroups_SelectedIndexChanged(object sender, EventArgs e) {
        if (lbMizGroups.SelectedItems.Count == 0) {
            SelectedTemplateGroup = null;
            return;
        }

        if (_previousSelectedIndex >= 0) {
            var rect = lbMizGroups.GetItemRectangle(_previousSelectedIndex);
            lbMizGroups.Invalidate(rect);
        } else {
            lbMizGroups.Invalidate();
        }

        _previousSelectedIndex = lbMizGroups.SelectedIndex;
        SelectedTemplateGroup = lbMizGroups.SelectedItem as DCSTemplateGroupInfo;
        if (SelectedTemplateGroup != null) {
            lblSelectedGroupInfo.Text = $"Name: [{SelectedTemplateGroup.Coalition.ToUpper()}] {SelectedTemplateGroup.GroupName}\nType: {SelectedTemplateGroup.DCSVehicleType}\nGroupId {SelectedTemplateGroup.GroupId}";
        }
    }

    private void lbApplyTo_SelectedIndexChanged(object sender, EventArgs e) {
    }

    private void miOpenMission_Click(object sender, EventArgs e) {
        DialogResult result = dlgOpenFile.ShowDialog();
        if (result != DialogResult.OK) {
            return;
        }

        string fileName = dlgOpenFile.FileName;
        FileInfo original = new(fileName);
        string backup = original.FullName + DateTime.Now.ToString("_yyyy-MM-dd-HH-mm-ss") + ".bak";
        File.Copy(original.FullName, backup);

        _groups.Clear();
        GroupsInMission.Clear();
        _missionFilePath = fileName;
        LoadMizFile(fileName);
    }

    private void helpToolStripMenuItem_Click(object sender, EventArgs e) {
    Process.Start(new ProcessStartInfo {
        FileName = "https://github.com/tgudelj/dcs-dynspawn-util",
        UseShellExecute = true
        });
    }

    private DialogResult ShowCenteredMessage(string message, MessageBoxIcon icon) {
        return MessageBox.Show(this, message, "Dynamic Spawn helper", MessageBoxButtons.OK, icon);
    }
    #endregion

    private void LoadMizFile(string filename) {
        SelectedTemplateGroup = null;
        GroupsInMission.Clear();
        _groups.Clear();
        lbApplyTo.SelectedIndex = -1;
        lbMizGroups.SelectedIndex = -1;

        _loadedMission = _missionService.LoadMissionFile(filename);
        GroupsInMission = _loadedMission.GroupsInMission;
        foreach (DCSTemplateGroupInfo group in GroupsInMission) {
            _groups.Add(group);
        }
        if (_groups.Count > 0) { 
            lbMizGroups.SelectedIndex = 0;
            SelectedTemplateGroup = lbMizGroups.SelectedItem as DCSTemplateGroupInfo;
            lblSelectedGroupInfo.Text = $"Name: [{SelectedTemplateGroup.Coalition.ToUpper()}] {SelectedTemplateGroup.GroupName}\nType: {SelectedTemplateGroup.DCSVehicleType}\nGroupId {SelectedTemplateGroup.GroupId}";
        }
    }

    private void btnApply_Click(object sender, EventArgs e) {
        if (SelectedTemplateGroup == null || _loadedMission == null || _missionFilePath == null) {
            ShowCenteredMessage("Please select a group that will be used as waypoint template", MessageBoxIcon.Warning);
            return;
        }

        if (lbApplyTo.SelectedItems.Count == 0) {
            ShowCenteredMessage("Please select at least one aircraft type to which the template will be applied", MessageBoxIcon.Warning);
            return;
        }

        Cursor.Current = Cursors.WaitCursor;
        try {
            List<DCSTypeInfo> selectedTypes = lbApplyTo.SelectedItems.Cast<DCSTypeInfo>().ToList();
            _missionService.ApplyTemplateAndSave(_missionFilePath, _loadedMission, SelectedTemplateGroup, selectedTypes);
            ShowCenteredMessage("Operation completed successfuly!", MessageBoxIcon.Information);
            LoadMizFile(_missionFilePath);
        } finally {
            Cursor.Current = Cursors.Default;
        }
    }

    private void lbMizGroups_DrawItem(object sender, DrawItemEventArgs e) {
        if (e.Index < 0) {
            return;
        }

        DCSTemplateGroupInfo? item = lbMizGroups.Items[e.Index] as DCSTemplateGroupInfo;
        Color itemColor = Color.Black;
        if (item != null) {
            switch (item.Coalition.ToLower()) {
                case "red":
                    itemColor = Color.Red;
                    break;
                case "blue":
                    itemColor = Color.Blue;
                    break;
            }
        }

        if (e.Index == lbMizGroups.SelectedIndex) {
            itemColor = Color.White;
        }

        e.DrawBackground();
        using (Brush brush = new SolidBrush(itemColor)) {
            e.Graphics.DrawString(lbMizGroups.GetItemText(item), e.Font, brush, e.Bounds);
        }

        e.DrawFocusRectangle();
    }
}















