namespace WinForm
{
    partial class Form1
    {
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            
            // Main Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Text = "AlwaysDown";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            
            // Menu Strip
            this.menuStrip = new MenuStrip();
            this.fileToolStripMenuItem = new ToolStripMenuItem();
            this.addApplicationToolStripMenuItem = new ToolStripMenuItem();
            this.settingsToolStripMenuItem = new ToolStripMenuItem();
            this.exitToolStripMenuItem = new ToolStripMenuItem();
            this.helpToolStripMenuItem = new ToolStripMenuItem();
            this.aboutToolStripMenuItem = new ToolStripMenuItem();
            
            // Toolbar
            this.toolStrip = new ToolStrip();
            this.addAppButton = new ToolStripButton();
            this.removeAppButton = new ToolStripButton();
            this.startAppButton = new ToolStripButton();
            this.stopAppButton = new ToolStripButton();
            this.restartAppButton = new ToolStripButton();
            this.toolStripSeparator1 = new ToolStripSeparator();
            this.installServiceButton = new ToolStripButton();
            this.uninstallServiceButton = new ToolStripButton();
            this.toolStripSeparator2 = new ToolStripSeparator();
            this.refreshButton = new ToolStripButton();
            
            // Status Strip
            this.statusStrip = new StatusStrip();
            this.statusLabel = new ToolStripStatusLabel();
            this.progressBar = new ToolStripProgressBar();
            
            // Main Tab Control
            this.mainTabControl = new TabControl();
            this.applicationsTab = new TabPage();
            this.logsTab = new TabPage();
            
            // Applications Tab Content
            this.splitContainer1 = new SplitContainer();
            this.applicationsListView = new ListView();
            this.nameColumn = new ColumnHeader();
            this.statusColumn = new ColumnHeader();
            this.pathColumn = new ColumnHeader();
            this.commandColumn = new ColumnHeader();
            this.pidColumn = new ColumnHeader();
            this.lastStartedColumn = new ColumnHeader();
            this.serviceColumn = new ColumnHeader();
            
            // Application Details Panel
            this.applicationDetailsPanel = new GroupBox();
            this.nameLabel = new Label();
            this.nameTextBox = new TextBox();
            this.descriptionLabel = new Label();
            this.descriptionTextBox = new TextBox();
            this.pathLabel = new Label();
            this.pathTextBox = new TextBox();
            this.browseButton = new Button();
            this.commandLabel = new Label();
            this.commandTextBox = new TextBox();
            this.commandHelpButton = new Button();
            this.portLabel = new Label();
            this.portNumericUpDown = new NumericUpDown();
            this.autoStartCheckBox = new CheckBox();
            this.saveButton = new Button();
            this.cancelButton = new Button();
            
            // Logs Tab Content
            this.logsSplitContainer = new SplitContainer();
            this.logAppsListBox = new ListBox();
            this.logTextBox = new RichTextBox();
            this.clearLogsButton = new Button();
            this.autoScrollCheckBox = new CheckBox();
            this.logFilterTextBox = new TextBox();
            this.filterLabel = new Label();
            
            // Context Menu
            this.contextMenuStrip = new ContextMenuStrip(this.components);
            this.startContextMenuItem = new ToolStripMenuItem();
            this.stopContextMenuItem = new ToolStripMenuItem();
            this.restartContextMenuItem = new ToolStripMenuItem();
            this.toolStripSeparator3 = new ToolStripSeparator();
            this.installServiceContextMenuItem = new ToolStripMenuItem();
            this.uninstallServiceContextMenuItem = new ToolStripMenuItem();
            this.toolStripSeparator4 = new ToolStripSeparator();
            this.editContextMenuItem = new ToolStripMenuItem();
            this.deleteContextMenuItem = new ToolStripMenuItem();
            
            // Timer for refresh
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            
            // Notification Tray
            this.notifyIcon = new NotifyIcon(this.components);
            this.trayContextMenuStrip = new ContextMenuStrip(this.components);
            this.showTrayMenuItem = new ToolStripMenuItem();
            this.exitTrayMenuItem = new ToolStripMenuItem();
            
            // Folder Browser Dialog
            this.folderBrowserDialog = new FolderBrowserDialog();
            
            this.SuspendLayout();
            
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.fileToolStripMenuItem,
                this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1200, 28);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";
            
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.addApplicationToolStripMenuItem,
                this.settingsToolStripMenuItem,
                new ToolStripSeparator(),
                this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "&File";
            
            // 
            // addApplicationToolStripMenuItem
            // 
            this.addApplicationToolStripMenuItem.Name = "addApplicationToolStripMenuItem";
            this.addApplicationToolStripMenuItem.Size = new System.Drawing.Size(200, 26);
            this.addApplicationToolStripMenuItem.Text = "&Add Application";
            this.addApplicationToolStripMenuItem.Click += new System.EventHandler(this.AddApplication_Click);
            
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(200, 26);
            this.settingsToolStripMenuItem.Text = "&Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.Settings_Click);
            
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(200, 26);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.Exit_Click);
            
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this.helpToolStripMenuItem.Text = "&Help";
            
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(133, 26);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.About_Click);
            
            // 
            // toolStrip
            // 
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.addAppButton,
                this.removeAppButton,
                this.toolStripSeparator1,
                this.startAppButton,
                this.stopAppButton,
                this.restartAppButton,
                this.toolStripSeparator2,
                this.installServiceButton,
                this.uninstallServiceButton,
                this.toolStripSeparator3,
                this.refreshButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 28);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1200, 27);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip";
            
            // Setup toolbar buttons
            this.addAppButton.Text = "Add App";
            this.addAppButton.Click += new System.EventHandler(this.AddApplication_Click);
            
            this.removeAppButton.Text = "Remove";
            this.removeAppButton.Click += new System.EventHandler(this.RemoveApplication_Click);
            
            this.startAppButton.Text = "Start";
            this.startAppButton.Click += new System.EventHandler(this.StartApplication_Click);
            
            this.stopAppButton.Text = "Stop";
            this.stopAppButton.Click += new System.EventHandler(this.StopApplication_Click);
            
            this.restartAppButton.Text = "Restart";
            this.restartAppButton.Click += new System.EventHandler(this.RestartApplication_Click);
            
            this.installServiceButton.Text = "Install Service";
            this.installServiceButton.Click += new System.EventHandler(this.InstallService_Click);
            
            this.uninstallServiceButton.Text = "Uninstall Service";
            this.uninstallServiceButton.Click += new System.EventHandler(this.UninstallService_Click);
            
            this.refreshButton.Text = "Refresh";
            this.refreshButton.Click += new System.EventHandler(this.Refresh_Click);
            
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.statusLabel,
                this.progressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 774);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1200, 26);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip";
            
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(49, 20);
            this.statusLabel.Text = "Ready";
            
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 18);
            this.progressBar.Visible = false;
            
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.applicationsTab);
            this.mainTabControl.Controls.Add(this.logsTab);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 55);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(1200, 719);
            this.mainTabControl.TabIndex = 3;
            
            // 
            // applicationsTab
            // 
            this.applicationsTab.Controls.Add(this.splitContainer1);
            this.applicationsTab.Location = new System.Drawing.Point(4, 29);
            this.applicationsTab.Name = "applicationsTab";
            this.applicationsTab.Padding = new System.Windows.Forms.Padding(3);
            this.applicationsTab.Size = new System.Drawing.Size(1192, 686);
            this.applicationsTab.TabIndex = 0;
            this.applicationsTab.Text = "Applications";
            this.applicationsTab.UseVisualStyleBackColor = true;
            
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainer1.Panel1.Controls.Add(this.applicationsListView);
            this.splitContainer1.Panel2.Controls.Add(this.applicationDetailsPanel);
            this.splitContainer1.Size = new System.Drawing.Size(1186, 680);
            this.splitContainer1.SplitterDistance = 400;
            this.splitContainer1.TabIndex = 0;
            
            // Setup Applications ListView
            this.applicationsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.applicationsListView.FullRowSelect = true;
            this.applicationsListView.GridLines = true;
            this.applicationsListView.View = System.Windows.Forms.View.Details;
            this.applicationsListView.SelectedIndexChanged += new System.EventHandler(this.ApplicationsListView_SelectedIndexChanged);
            this.applicationsListView.ContextMenuStrip = this.contextMenuStrip;
            
            // Setup ListView Columns
            this.nameColumn.Text = "Name";
            this.nameColumn.Width = 150;
            this.statusColumn.Text = "Status";
            this.statusColumn.Width = 100;
            this.pathColumn.Text = "Path";
            this.pathColumn.Width = 250;
            this.commandColumn.Text = "Command";
            this.commandColumn.Width = 150;
            this.pidColumn.Text = "PID";
            this.pidColumn.Width = 80;
            this.lastStartedColumn.Text = "Last Started";
            this.lastStartedColumn.Width = 150;
            this.serviceColumn.Text = "Startup";
            this.serviceColumn.Width = 100;
            
            this.applicationsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.nameColumn,
                this.statusColumn,
                this.pathColumn,
                this.commandColumn,
                this.pidColumn,
                this.lastStartedColumn,
                this.serviceColumn});
            
            // Setup Application Details Panel
            this.applicationDetailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.applicationDetailsPanel.Text = "Application Details";
            this.applicationDetailsPanel.Padding = new Padding(10);
            
            // Setup detail controls (simplified layout)
            this.nameLabel.Text = "Name:";
            this.nameLabel.Location = new System.Drawing.Point(20, 30);
            this.nameLabel.Size = new System.Drawing.Size(100, 23);
            
            this.nameTextBox.Location = new System.Drawing.Point(130, 30);
            this.nameTextBox.Size = new System.Drawing.Size(300, 27);
            
            this.descriptionLabel.Text = "Description:";
            this.descriptionLabel.Location = new System.Drawing.Point(20, 70);
            this.descriptionLabel.Size = new System.Drawing.Size(100, 23);
            
            this.descriptionTextBox.Location = new System.Drawing.Point(130, 70);
            this.descriptionTextBox.Size = new System.Drawing.Size(300, 27);
            
            this.pathLabel.Text = "Project Path:";
            this.pathLabel.Location = new System.Drawing.Point(20, 110);
            this.pathLabel.Size = new System.Drawing.Size(100, 23);
            
            this.pathTextBox.Location = new System.Drawing.Point(130, 110);
            this.pathTextBox.Size = new System.Drawing.Size(250, 27);
            
            this.browseButton.Text = "Browse";
            this.browseButton.Location = new System.Drawing.Point(390, 110);
            this.browseButton.Size = new System.Drawing.Size(80, 27);
            this.browseButton.Click += new System.EventHandler(this.Browse_Click);
            
            this.commandLabel.Text = "Start Command:";
            this.commandLabel.Location = new System.Drawing.Point(20, 150);
            this.commandLabel.Size = new System.Drawing.Size(100, 23);
            
            this.commandTextBox.Location = new System.Drawing.Point(130, 150);
            this.commandTextBox.Size = new System.Drawing.Size(220, 27);
            
            this.commandHelpButton.Text = "Examples";
            this.commandHelpButton.Location = new System.Drawing.Point(360, 150);
            this.commandHelpButton.Size = new System.Drawing.Size(80, 27);
            this.commandHelpButton.Click += new System.EventHandler(this.CommandHelp_Click);
            
            this.portLabel.Text = "Port:";
            this.portLabel.Location = new System.Drawing.Point(20, 190);
            this.portLabel.Size = new System.Drawing.Size(100, 23);
            
            this.portNumericUpDown.Location = new System.Drawing.Point(130, 190);
            this.portNumericUpDown.Size = new System.Drawing.Size(120, 27);
            this.portNumericUpDown.Minimum = 1;
            this.portNumericUpDown.Maximum = 65535;
            this.portNumericUpDown.Value = 3000;
            
            this.autoStartCheckBox.Text = "Auto Start with Windows";
            this.autoStartCheckBox.Location = new System.Drawing.Point(130, 230);
            this.autoStartCheckBox.Size = new System.Drawing.Size(200, 24);
            
            this.saveButton.Text = "Save";
            this.saveButton.Location = new System.Drawing.Point(300, 230);
            this.saveButton.Size = new System.Drawing.Size(80, 30);
            this.saveButton.Click += new System.EventHandler(this.SaveApplication_Click);
            
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Location = new System.Drawing.Point(390, 230);
            this.cancelButton.Size = new System.Drawing.Size(80, 30);
            this.cancelButton.Click += new System.EventHandler(this.CancelEdit_Click);
            
            this.applicationDetailsPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.nameLabel, this.nameTextBox,
                this.descriptionLabel, this.descriptionTextBox,
                this.pathLabel, this.pathTextBox, this.browseButton,
                this.commandLabel, this.commandTextBox, this.commandHelpButton,
                this.portLabel, this.portNumericUpDown,
                this.autoStartCheckBox,
                this.saveButton, this.cancelButton});
            
            // 
            // logsTab
            // 
            this.logsTab.Controls.Add(this.logsSplitContainer);
            this.logsTab.Location = new System.Drawing.Point(4, 29);
            this.logsTab.Name = "logsTab";
            this.logsTab.Padding = new System.Windows.Forms.Padding(3);
            this.logsTab.Size = new System.Drawing.Size(1192, 686);
            this.logsTab.TabIndex = 1;
            this.logsTab.Text = "Logs";
            this.logsTab.UseVisualStyleBackColor = true;
            
            // 
            // logsSplitContainer
            // 
            this.logsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logsSplitContainer.Location = new System.Drawing.Point(3, 3);
            this.logsSplitContainer.Name = "logsSplitContainer";
            this.logsSplitContainer.Panel1.Controls.Add(this.logAppsListBox);
            this.logsSplitContainer.Panel2.Controls.Add(this.logTextBox);
            this.logsSplitContainer.Panel2.Controls.Add(this.clearLogsButton);
            this.logsSplitContainer.Panel2.Controls.Add(this.autoScrollCheckBox);
            this.logsSplitContainer.Panel2.Controls.Add(this.filterLabel);
            this.logsSplitContainer.Panel2.Controls.Add(this.logFilterTextBox);
            this.logsSplitContainer.Size = new System.Drawing.Size(1186, 680);
            this.logsSplitContainer.SplitterDistance = 200;
            this.logsSplitContainer.TabIndex = 0;
            
            this.logAppsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logAppsListBox.SelectedIndexChanged += new System.EventHandler(this.LogAppsListBox_SelectedIndexChanged);
            
            this.logTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logTextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.logTextBox.ReadOnly = true;
            this.logTextBox.BackColor = System.Drawing.Color.Black;
            this.logTextBox.ForeColor = System.Drawing.Color.White;
            this.logTextBox.Margin = new Padding(0, 40, 0, 40);
            
            this.filterLabel.Text = "Filter:";
            this.filterLabel.Location = new System.Drawing.Point(10, 10);
            this.filterLabel.Size = new System.Drawing.Size(50, 23);
            
            this.logFilterTextBox.Location = new System.Drawing.Point(65, 10);
            this.logFilterTextBox.Size = new System.Drawing.Size(200, 27);
            this.logFilterTextBox.TextChanged += new System.EventHandler(this.LogFilter_TextChanged);
            
            this.autoScrollCheckBox.Text = "Auto Scroll";
            this.autoScrollCheckBox.Location = new System.Drawing.Point(280, 10);
            this.autoScrollCheckBox.Size = new System.Drawing.Size(100, 24);
            this.autoScrollCheckBox.Checked = true;
            
            this.clearLogsButton.Text = "Clear";
            this.clearLogsButton.Location = new System.Drawing.Point(400, 10);
            this.clearLogsButton.Size = new System.Drawing.Size(80, 27);
            this.clearLogsButton.Click += new System.EventHandler(this.ClearLogs_Click);
            
            // Setup Context Menu
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.startContextMenuItem,
                this.stopContextMenuItem,
                this.restartContextMenuItem,
                this.toolStripSeparator3,
                this.installServiceContextMenuItem,
                this.uninstallServiceContextMenuItem,
                this.toolStripSeparator4,
                this.editContextMenuItem,
                this.deleteContextMenuItem});
            
            this.startContextMenuItem.Text = "Start";
            this.startContextMenuItem.Click += new System.EventHandler(this.StartApplication_Click);
            
            this.stopContextMenuItem.Text = "Stop";
            this.stopContextMenuItem.Click += new System.EventHandler(this.StopApplication_Click);
            
            this.restartContextMenuItem.Text = "Restart";
            this.restartContextMenuItem.Click += new System.EventHandler(this.RestartApplication_Click);
            
            this.installServiceContextMenuItem.Text = "Install as Service";
            this.installServiceContextMenuItem.Click += new System.EventHandler(this.InstallService_Click);
            
            this.uninstallServiceContextMenuItem.Text = "Uninstall Service";
            this.uninstallServiceContextMenuItem.Click += new System.EventHandler(this.UninstallService_Click);
            
            this.editContextMenuItem.Text = "Edit";
            this.editContextMenuItem.Click += new System.EventHandler(this.EditApplication_Click);
            
            this.deleteContextMenuItem.Text = "Delete";
            this.deleteContextMenuItem.Click += new System.EventHandler(this.RemoveApplication_Click);
            
            // Setup Timer
            this.refreshTimer.Interval = 5000;
            this.refreshTimer.Tick += new System.EventHandler(this.RefreshTimer_Tick);
            this.refreshTimer.Enabled = true;
            
            // Setup Tray Icon
            this.notifyIcon.Text = "AlwaysDown";
            this.notifyIcon.Visible = false;
            this.notifyIcon.DoubleClick += new System.EventHandler(this.NotifyIcon_DoubleClick);
            this.notifyIcon.ContextMenuStrip = this.trayContextMenuStrip;
            
            this.trayContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.showTrayMenuItem,
                this.exitTrayMenuItem});
            
            this.showTrayMenuItem.Text = "Show";
            this.showTrayMenuItem.Click += new System.EventHandler(this.ShowTrayMenuItem_Click);
            
            this.exitTrayMenuItem.Text = "Exit";
            this.exitTrayMenuItem.Click += new System.EventHandler(this.Exit_Click);
            
            // 
            // Form1
            // 
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem addApplicationToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        
        private ToolStrip toolStrip;
        private ToolStripButton addAppButton;
        private ToolStripButton removeAppButton;
        private ToolStripButton startAppButton;
        private ToolStripButton stopAppButton;
        private ToolStripButton restartAppButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton installServiceButton;
        private ToolStripButton uninstallServiceButton;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton refreshButton;
        
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private ToolStripProgressBar progressBar;
        
        private TabControl mainTabControl;
        private TabPage applicationsTab;
        private TabPage logsTab;
        
        private SplitContainer splitContainer1;
        private ListView applicationsListView;
        private ColumnHeader nameColumn;
        private ColumnHeader statusColumn;
        private ColumnHeader pathColumn;
        private ColumnHeader commandColumn;
        private ColumnHeader pidColumn;
        private ColumnHeader lastStartedColumn;
        private ColumnHeader serviceColumn;
        
        private GroupBox applicationDetailsPanel;
        private Label nameLabel;
        private TextBox nameTextBox;
        private Label descriptionLabel;
        private TextBox descriptionTextBox;
        private Label pathLabel;
        private TextBox pathTextBox;
        private Button browseButton;
        private Label commandLabel;
        private TextBox commandTextBox;
        private Button commandHelpButton;
        private Label portLabel;
        private NumericUpDown portNumericUpDown;
        private CheckBox autoStartCheckBox;
        private Button saveButton;
        private Button cancelButton;
        
        private SplitContainer logsSplitContainer;
        private ListBox logAppsListBox;
        private RichTextBox logTextBox;
        private Button clearLogsButton;
        private CheckBox autoScrollCheckBox;
        private TextBox logFilterTextBox;
        private Label filterLabel;
        
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem startContextMenuItem;
        private ToolStripMenuItem stopContextMenuItem;
        private ToolStripMenuItem restartContextMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem installServiceContextMenuItem;
        private ToolStripMenuItem uninstallServiceContextMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem editContextMenuItem;
        private ToolStripMenuItem deleteContextMenuItem;
        
        private System.Windows.Forms.Timer refreshTimer;
        
        private NotifyIcon notifyIcon;
        private ContextMenuStrip trayContextMenuStrip;
        private ToolStripMenuItem showTrayMenuItem;
        private ToolStripMenuItem exitTrayMenuItem;
        
        private FolderBrowserDialog folderBrowserDialog;
    }
}