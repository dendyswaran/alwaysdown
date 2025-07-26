using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinForm.Models;
using WinForm.Services;

namespace WinForm
{
    public partial class Form1 : Form
    {
        private readonly NodeJsProcessManager _processManager;
        private readonly WindowsServiceManager _serviceManager;
        private readonly ConfigurationManager _configManager;
        private List<NodeJsApplication> _applications;
        private NodeJsApplication? _selectedApplication;
        private bool _isEditing = false;
        
        public Form1()
        {
            InitializeComponent();
            
            _processManager = new NodeJsProcessManager();
            _serviceManager = new WindowsServiceManager();
            _configManager = new ConfigurationManager();
            _applications = new List<NodeJsApplication>();
            
            // Subscribe to events
            _processManager.ProcessStatusChanged += OnProcessStatusChanged;
            _processManager.ProcessLogReceived += OnProcessLogReceived;
            
            // Initialize UI
            _ = InitializeUI();
            
            // Load applications
            _ = LoadApplicationsAsync();
        }
        
        private async Task InitializeUI()
        {
            // Set window icon
            this.Icon = SystemIcons.Application;
            
            // Setup tray icon
            notifyIcon.Icon = SystemIcons.Application;
            
            // Load settings
            try
            {
                var settings = await _configManager.LoadAppSettingsAsync();
                refreshTimer.Interval = settings.RefreshInterval;
                
                if (settings.StartMinimized)
                {
                    this.WindowState = FormWindowState.Minimized;
                    if (settings.MinimizeToTray)
                    {
                        this.Hide();
                        notifyIcon.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Failed to load settings: {ex.Message}");
            }
            
            // Clear details panel initially
            ClearApplicationDetails();
            await UpdateUI();
        }
        
        private async Task LoadApplicationsAsync()
        {
            try
            {
                SetStatus("Loading applications...");
                _applications = await _configManager.LoadApplicationsAsync();
                
                // Update application status
                foreach (var app in _applications)
                {
                    app.IsRunning = _processManager.IsApplicationRunning(app.Id);
                    
                    // Check service status if service is installed
                    try
                    {
                        if (await _serviceManager.ServiceExistsAsync(app.ServiceName))
                        {
                            var serviceStatus = await _serviceManager.GetServiceStatusAsync(app.ServiceName);
                            // Service status takes precedence over process status
                            if (serviceStatus == ServiceControllerStatus.Running)
                            {
                                app.IsRunning = true;
                            }
                        }
                    }
                    catch
                    {
                        // Ignore service check errors, fall back to process status
                    }
                }
                
                await RefreshApplicationsList();
                RefreshLogsList();
                SetStatus("Applications loaded successfully.");
            }
            catch (Exception ex)
            {
                ShowError($"Failed to load applications: {ex.Message}");
            }
        }
        
        private async Task RefreshApplicationsList()
        {
            applicationsListView.Items.Clear();
            
            foreach (var app in _applications)
            {
                var item = new ListViewItem(app.GetDisplayName());
                item.SubItems.Add(app.GetStatus());
                item.SubItems.Add(app.ProjectPath);
                item.SubItems.Add(app.StartCommand);
                item.SubItems.Add(app.ProcessId?.ToString() ?? "");
                item.SubItems.Add(app.LastStarted?.ToString("yyyy-MM-dd HH:mm:ss") ?? "");
                item.SubItems.Add(app.AutoStart ? "Auto Start" : "Manual");
                item.Tag = app;
                
                // Color coding
                if (app.IsRunning)
                {
                    item.ForeColor = Color.Green;
                }
                else
                {
                    item.ForeColor = Color.Red;
                }
                
                applicationsListView.Items.Add(item);
            }
            
            await UpdateUI();
        }
        
        private void RefreshLogsList()
        {
            var selectedApp = logAppsListBox.SelectedItem as NodeJsApplication;
            
            logAppsListBox.Items.Clear();
            foreach (var app in _applications)
            {
                logAppsListBox.Items.Add(app);
            }
            
            // Restore selection
            if (selectedApp != null)
            {
                var index = _applications.FindIndex(a => a.Id == selectedApp.Id);
                if (index >= 0)
                {
                    logAppsListBox.SelectedIndex = index;
                }
            }
        }
        
        private async Task UpdateUI()
        {
            var hasSelection = _selectedApplication != null;
            var isRunning = hasSelection && _selectedApplication.IsRunning;
            var hasService = hasSelection && await _serviceManager.ServiceExistsAsync(_selectedApplication.ServiceName);
            
            // Toolbar buttons
            startAppButton.Enabled = hasSelection && !isRunning;
            stopAppButton.Enabled = hasSelection && isRunning;
            restartAppButton.Enabled = hasSelection;
            removeAppButton.Enabled = hasSelection && !_isEditing;
            
            // Enable service buttons
            installServiceButton.Enabled = hasSelection && !hasService && !_isEditing;
            uninstallServiceButton.Enabled = hasSelection && hasService && !_isEditing;
            installServiceButton.ToolTipText = hasService ? "Service already installed" : "Install as Windows Service";
            uninstallServiceButton.ToolTipText = hasService ? "Uninstall Windows Service" : "No service installed";
            
            // Context menu
            startContextMenuItem.Enabled = startAppButton.Enabled;
            stopContextMenuItem.Enabled = stopAppButton.Enabled;
            restartContextMenuItem.Enabled = restartAppButton.Enabled;
            installServiceContextMenuItem.Enabled = installServiceButton.Enabled;
            uninstallServiceContextMenuItem.Enabled = uninstallServiceButton.Enabled;
            editContextMenuItem.Enabled = hasSelection && !_isEditing;
            deleteContextMenuItem.Enabled = removeAppButton.Enabled;
            
            // Details panel
            saveButton.Enabled = _isEditing;
            cancelButton.Enabled = _isEditing;
            
            var detailsEnabled = hasSelection && _isEditing;
            nameTextBox.ReadOnly = !detailsEnabled;
            descriptionTextBox.ReadOnly = !detailsEnabled;
            pathTextBox.ReadOnly = !detailsEnabled;
            commandTextBox.ReadOnly = !detailsEnabled;
            portNumericUpDown.Enabled = detailsEnabled;
            autoStartCheckBox.Enabled = detailsEnabled;
            browseButton.Enabled = detailsEnabled;
        }
        
        private void UpdateUISync()
        {
            var hasSelection = _selectedApplication != null;
            var isRunning = hasSelection && _selectedApplication.IsRunning;
            var hasService = hasSelection && _serviceManager.ServiceExists(_selectedApplication.ServiceName);
            
            // Toolbar buttons
            startAppButton.Enabled = hasSelection && !isRunning;
            stopAppButton.Enabled = hasSelection && isRunning;
            restartAppButton.Enabled = hasSelection;
            removeAppButton.Enabled = hasSelection && !_isEditing;
            
            // Service buttons (conservative approach for sync version)
            installServiceButton.Enabled = hasSelection && !hasService && !_isEditing;
            uninstallServiceButton.Enabled = hasSelection && hasService && !_isEditing;
            installServiceButton.ToolTipText = hasService ? "Service already installed" : "Install as Windows Service";
            uninstallServiceButton.ToolTipText = hasService ? "Uninstall Windows Service" : "No service installed";
            
            // Context menu
            startContextMenuItem.Enabled = startAppButton.Enabled;
            stopContextMenuItem.Enabled = stopAppButton.Enabled;
            restartContextMenuItem.Enabled = restartAppButton.Enabled;
            installServiceContextMenuItem.Enabled = installServiceButton.Enabled;
            uninstallServiceContextMenuItem.Enabled = uninstallServiceButton.Enabled;
            editContextMenuItem.Enabled = hasSelection && !_isEditing;
            deleteContextMenuItem.Enabled = removeAppButton.Enabled;
            
            // Details panel
            saveButton.Enabled = _isEditing;
            cancelButton.Enabled = _isEditing;
            
            var detailsEnabled = hasSelection && _isEditing;
            nameTextBox.ReadOnly = !detailsEnabled;
            descriptionTextBox.ReadOnly = !detailsEnabled;
            pathTextBox.ReadOnly = !detailsEnabled;
            commandTextBox.ReadOnly = !detailsEnabled;
            portNumericUpDown.Enabled = detailsEnabled;
            autoStartCheckBox.Enabled = detailsEnabled;
            browseButton.Enabled = detailsEnabled;
        }
        
        private void ClearApplicationDetails()
        {
            nameTextBox.Text = "";
            descriptionTextBox.Text = "";
            pathTextBox.Text = "";
            commandTextBox.Text = "";
            portNumericUpDown.Value = 3000;
            autoStartCheckBox.Checked = false;
        }
        
        private void LoadApplicationDetails(NodeJsApplication app)
        {
            nameTextBox.Text = app.Name;
            descriptionTextBox.Text = app.Description;
            pathTextBox.Text = app.ProjectPath;
            commandTextBox.Text = app.StartCommand;
            portNumericUpDown.Value = app.Port;
            autoStartCheckBox.Checked = app.AutoStart;
        }
        
        private void SaveApplicationDetails(NodeJsApplication app)
        {
            app.Name = nameTextBox.Text;
            app.Description = descriptionTextBox.Text;
            app.ProjectPath = pathTextBox.Text;
            app.StartCommand = commandTextBox.Text;
            app.Port = (int)portNumericUpDown.Value;
            app.AutoStart = autoStartCheckBox.Checked;
            app.WorkingDirectory = app.ProjectPath; // Default working directory
        }
        
        private async Task SaveApplicationsAsync()
        {
            try
            {
                await _configManager.SaveApplicationsAsync(_applications);
            }
            catch (Exception ex)
            {
                ShowError($"Failed to save applications: {ex.Message}");
            }
        }
        
        private void SetStatus(string message)
        {
            statusLabel.Text = message;
            Application.DoEvents();
        }
        
        private void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
        private void ShowInfo(string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private bool ConfirmAction(string message)
        {
            return MessageBox.Show(message, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
        
        // Event Handlers
        private void OnProcessStatusChanged(object? sender, ProcessStatusChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnProcessStatusChanged(sender, e)));
                return;
            }
            
            var app = _applications.FirstOrDefault(a => a.Id == e.ApplicationId);
            if (app != null)
            {
                app.IsRunning = e.IsRunning;
                app.ProcessId = e.ProcessId;
                
                if (e.IsRunning)
                {
                    app.LastStarted = DateTime.Now;
                }
                else
                {
                    app.LastStopped = DateTime.Now;
                    app.ProcessId = null;
                }
                
                RefreshApplicationsList();
                _ = SaveApplicationsAsync();
            }
        }
        
        private void OnProcessLogReceived(object? sender, ProcessLogEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnProcessLogReceived(sender, e)));
                return;
            }
            
            var selectedApp = logAppsListBox.SelectedItem as NodeJsApplication;
            if (selectedApp?.Id == e.ApplicationId)
            {
                // Apply filter if set
                var filterText = logFilterTextBox.Text.Trim();
                if (string.IsNullOrEmpty(filterText) || e.LogEntry.Contains(filterText, StringComparison.OrdinalIgnoreCase))
                {
                    // Color code the log entry
                    var color = e.IsError ? Color.Red : Color.White;
                    
                    logTextBox.SelectionStart = logTextBox.TextLength;
                    logTextBox.SelectionLength = 0;
                    logTextBox.SelectionColor = color;
                    logTextBox.AppendText(e.LogEntry + Environment.NewLine);
                    
                    if (autoScrollCheckBox.Checked)
                    {
                        logTextBox.ScrollToCaret();
                    }
                }
            }
        }
        
        // UI Event Handlers
        private void AddApplication_Click(object sender, EventArgs e)
        {
            var newApp = new NodeJsApplication
            {
                Name = "New Application",
                StartCommand = "npm run start",
                Port = 3000
            };
            
            _applications.Add(newApp);
            RefreshApplicationsList();
            RefreshLogsList();
            
            // Select the new application
            var item = applicationsListView.Items.Cast<ListViewItem>()
                .FirstOrDefault(i => ((NodeJsApplication)i.Tag).Id == newApp.Id);
            if (item != null)
            {
                item.Selected = true;
                applicationsListView.Focus();
                EditApplication_Click(sender, e);
            }
        }
        
        private async void RemoveApplication_Click(object sender, EventArgs e)
        {
            if (_selectedApplication == null) return;
            
            if (!ConfirmAction($"Are you sure you want to remove '{_selectedApplication.GetDisplayName()}'?"))
                return;
            
            try
            {
                // Stop application if running
                if (_selectedApplication.IsRunning)
                {
                    await _processManager.StopApplicationAsync(_selectedApplication);
                }
                
                // Uninstall service if installed
                try
                {
                    if (await _serviceManager.ServiceExistsAsync(_selectedApplication.ServiceName))
                    {
                        await _serviceManager.UninstallServiceAsync(_selectedApplication);
                    }
                }
                catch
                {
                    // Ignore service uninstall errors during removal
                }
                
                // Remove from configuration
                await _configManager.DeleteApplicationConfigAsync(_selectedApplication.Id);
                
                _applications.Remove(_selectedApplication);
                await SaveApplicationsAsync();
                
                await RefreshApplicationsList();
                RefreshLogsList();
                ClearApplicationDetails();
                _selectedApplication = null;
                _isEditing = false;
                await UpdateUI();
                
                SetStatus("Application removed successfully.");
            }
            catch (Exception ex)
            {
                ShowError($"Failed to remove application: {ex.Message}");
            }
        }
        
        private async void StartApplication_Click(object sender, EventArgs e)
        {
            if (_selectedApplication == null) return;
            
            try
            {
                SetStatus($"Starting {_selectedApplication.GetDisplayName()}...");
                progressBar.Visible = true;
                
                bool success = false;
                
                // Check if it's installed as a service
                try
                {
                    if (await _serviceManager.ServiceExistsAsync(_selectedApplication.ServiceName))
                    {
                        // Try to start as a service first
                        success = await _serviceManager.StartServiceAsync(_selectedApplication.ServiceName);
                        if (success)
                        {
                            _selectedApplication.IsRunning = true;
                            SetStatus($"{_selectedApplication.GetDisplayName()} service started successfully.");
                        }
                        else
                        {
                            SetStatus($"Failed to start {_selectedApplication.GetDisplayName()} service. Trying direct process start...");
                            success = await _processManager.StartApplicationAsync(_selectedApplication);
                        }
                    }
                    else
                    {
                        // Start as regular process
                        success = await _processManager.StartApplicationAsync(_selectedApplication);
                    }
                }
                catch
                {
                    // If service check fails, fall back to direct process start
                    success = await _processManager.StartApplicationAsync(_selectedApplication);
                }
                
                if (success)
                {
                    SetStatus($"{_selectedApplication.GetDisplayName()} started successfully.");
                    await RefreshApplicationsList();
                }
                else
                {
                    SetStatus($"Failed to start {_selectedApplication.GetDisplayName()}.");
                }
                
                await SaveApplicationsAsync();
            }
            catch (Exception ex)
            {
                ShowError($"Failed to start application: {ex.Message}");
            }
            finally
            {
                progressBar.Visible = false;
            }
        }
        
        private async void StopApplication_Click(object sender, EventArgs e)
        {
            if (_selectedApplication == null) return;
            
            try
            {
                SetStatus($"Stopping {_selectedApplication.GetDisplayName()}...");
                progressBar.Visible = true;
                
                // Disable the stop button to prevent multiple clicks
                stopAppButton.Enabled = false;
                
                bool success = false;
                
                // Create a cancellation token with timeout for the entire operation
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60)); // 60 second total timeout
                
                try
                {
                    // Check if it's running as a service (with timeout)
                    bool serviceExists = false;
                    try
                    {
                        serviceExists = await _serviceManager.ServiceExistsAsync(_selectedApplication.ServiceName);
                    }
                    catch (Exception ex)
                    {
                        SetStatus($"Error checking service existence: {ex.Message}");
                        serviceExists = false; // Assume no service on error
                    }
                    
                    if (serviceExists)
                    {
                        SetStatus($"Stopping {_selectedApplication.GetDisplayName()} service...");
                        // Try to stop the service first
                        var serviceStopTask = _serviceManager.StopServiceAsync(_selectedApplication.ServiceName);
                        success = await serviceStopTask.WaitAsync(cts.Token);
                        
                        if (success)
                        {
                            _selectedApplication.IsRunning = false;
                            SetStatus($"{_selectedApplication.GetDisplayName()} service stopped successfully.");
                        }
                        else
                        {
                            SetStatus($"Service stop timed out or failed. Trying direct process stop...");
                        }
                    }
                    
                    // Also try to stop the direct process (in case both are running or service stop failed)
                    SetStatus($"Stopping {_selectedApplication.GetDisplayName()} process...");
                    var processStopTask = _processManager.StopApplicationAsync(_selectedApplication);
                    var processSuccess = await processStopTask.WaitAsync(cts.Token);
                    success = success || processSuccess;
                    
                    if (success)
                    {
                        SetStatus($"{_selectedApplication.GetDisplayName()} stopped successfully.");
                        await RefreshApplicationsList();
                    }
                    else
                    {
                        SetStatus($"Failed to stop {_selectedApplication.GetDisplayName()}.");
                    }
                }
                catch (OperationCanceledException)
                {
                    SetStatus($"Stop operation timed out for {_selectedApplication.GetDisplayName()}.");
                    ShowError("The stop operation timed out. The application may still be running. Please check the process manually if needed.");
                }
                
                await SaveApplicationsAsync();
            }
            catch (Exception ex)
            {
                ShowError($"Failed to stop application: {ex.Message}");
                SetStatus("Stop operation failed.");
            }
            finally
            {
                progressBar.Visible = false;
                await UpdateUI(); // This will re-enable the stop button if needed
            }
        }
        
        private async void RestartApplication_Click(object sender, EventArgs e)
        {
            if (_selectedApplication == null) return;
            
            try
            {
                SetStatus($"Restarting {_selectedApplication.GetDisplayName()}...");
                progressBar.Visible = true;
                
                var success = await _processManager.RestartApplicationAsync(_selectedApplication);
                if (success)
                {
                    SetStatus($"{_selectedApplication.GetDisplayName()} restarted successfully.");
                }
                else
                {
                    SetStatus($"Failed to restart {_selectedApplication.GetDisplayName()}.");
                }
                
                await SaveApplicationsAsync();
            }
            catch (Exception ex)
            {
                ShowError($"Failed to restart application: {ex.Message}");
            }
            finally
            {
                progressBar.Visible = false;
            }
        }
        
        private async void InstallService_Click(object sender, EventArgs e)
        {
            if (_selectedApplication == null) return;
            
            try
            {
                SetStatus($"Installing Windows service for {_selectedApplication.GetDisplayName()}...");
                progressBar.Visible = true;
                
                var success = await _serviceManager.InstallServiceAsync(_selectedApplication);
                if (success)
                {
                    SetStatus($"Windows service installed successfully for {_selectedApplication.GetDisplayName()}.");
                    await RefreshApplicationsList();
                }
                else
                {
                    SetStatus($"Failed to install Windows service.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Failed to install Windows service: {ex.Message}");
            }
            finally
            {
                progressBar.Visible = false;
                UpdateUISync();
            }
        }
        
        private async void UninstallService_Click(object sender, EventArgs e)
        {
            if (_selectedApplication == null) return;
            
            if (!ConfirmAction($"Are you sure you want to uninstall the Windows service for '{_selectedApplication.GetDisplayName()}'?"))
                return;
            
            try
            {
                SetStatus($"Uninstalling Windows service for {_selectedApplication.GetDisplayName()}...");
                progressBar.Visible = true;
                
                var success = await _serviceManager.UninstallServiceAsync(_selectedApplication);
                if (success)
                {
                    SetStatus($"Windows service uninstalled successfully.");
                    await RefreshApplicationsList();
                }
                else
                {
                    SetStatus($"Failed to uninstall Windows service.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Failed to uninstall Windows service: {ex.Message}");
            }
            finally
            {
                progressBar.Visible = false;
                UpdateUISync();
            }
        }
        
        private void EditApplication_Click(object sender, EventArgs e)
        {
            if (_selectedApplication == null) return;
            
            _isEditing = true;
            LoadApplicationDetails(_selectedApplication);
            UpdateUISync();
        }
        
        private async void SaveApplication_Click(object sender, EventArgs e)
        {
            if (_selectedApplication == null || !_isEditing) return;
            
            try
            {
                var oldAutoStart = _selectedApplication.AutoStart;
                var oldServiceName = _selectedApplication.ServiceName;
                
                SaveApplicationDetails(_selectedApplication);
                
                // Handle service name change (due to application name change)
                if (oldServiceName != _selectedApplication.ServiceName)
                {
                    try
                    {
                        // Migrate the service if it exists
                        await _serviceManager.MigrateServiceAsync(oldServiceName, _selectedApplication);
                        SetStatus($"Service migrated from '{oldServiceName}' to '{_selectedApplication.ServiceName}'.");
                    }
                    catch (Exception ex)
                    {
                        ShowError($"Failed to migrate service: {ex.Message}");
                        // Continue with saving even if migration fails
                    }
                }
                
                // Handle auto-start setting change
                if (oldAutoStart != _selectedApplication.AutoStart)
                {
                    await _serviceManager.SetAutoStartAsync(_selectedApplication, _selectedApplication.AutoStart);
                }
                
                await SaveApplicationsAsync();
                
                _isEditing = false;
                await RefreshApplicationsList();
                RefreshLogsList();
                await UpdateUI();
                
                SetStatus("Application saved successfully.");
            }
            catch (Exception ex)
            {
                ShowError($"Failed to save application: {ex.Message}");
            }
        }
        
        private void CancelEdit_Click(object sender, EventArgs e)
        {
            if (_selectedApplication == null) return;
            
            _isEditing = false;
            LoadApplicationDetails(_selectedApplication);
            UpdateUISync();
        }
        
        private void Browse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                pathTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }
        
        private void CommandHelp_Click(object sender, EventArgs e)
        {
            var helpMessage = @"Common Start Commands:

📦 Node.js Applications:
• npm start
• npm run start
• npm run dev
• node app.js
• node server.js
• node index.js

☕ Java Applications:
• java -jar myapp.jar
• java -jar target/myapp-1.0.jar
• java -cp ""lib/*"" com.example.MainClass
• java -Xmx512m -jar myapp.jar

🐍 Python Applications:
• python app.py
• python -m flask run
• python manage.py runserver
• uvicorn main:app --host 0.0.0.0 --port 8000

🌐 Web Servers:
• http-server
• live-server
• serve -s build

💡 Tips:
• Use full paths if needed: java -jar C:\path\to\app.jar
• Add JVM options: java -Xmx1g -jar myapp.jar
• Set environment: set NODE_ENV=production && npm start
• Background with logs: your-command > app.log 2>&1";

            MessageBox.Show(helpMessage, "Start Command Examples", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private async void Refresh_Click(object sender, EventArgs e)
        {
            await LoadApplicationsAsync();
        }
        
        private void Settings_Click(object sender, EventArgs e)
        {
            // TODO: Open settings dialog
            ShowInfo("Settings dialog not implemented yet.");
        }
        
        private void About_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "AlwaysDown\n\n" +
                "Keep your applications running - always down, never down!\n\n" +
                "Supported Applications:\n" +
                "• Node.js (npm, yarn, node)\n" +
                "• Java (java -jar, spring boot, etc.)\n" +
                "• Python (flask, django, fastapi)\n" +
                "• Any command-line application\n\n" +
                "Features:\n" +
                "• Deploy and monitor applications\n" +
                "• Start, stop, and restart processes\n" +
                "• Auto-start with Windows\n" +
                "• Real-time log viewing\n" +
                "• System tray integration\n\n" +
                "Version 1.1\n" +
                "Built with .NET 8 and Windows Forms",
                "About AlwaysDown",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        
        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
        private void ApplicationsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (applicationsListView.SelectedItems.Count > 0)
            {
                _selectedApplication = (NodeJsApplication)applicationsListView.SelectedItems[0].Tag;
                LoadApplicationDetails(_selectedApplication);
            }
            else
            {
                _selectedApplication = null;
                ClearApplicationDetails();
            }
            
            _isEditing = false;
            UpdateUISync();
        }
        
        private async void LogAppsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedApp = logAppsListBox.SelectedItem as NodeJsApplication;
            if (selectedApp != null)
            {
                var logs = _processManager.GetApplicationLogs(selectedApp.Id);
                
                // If the application is running as a service, also show service logs
                try
                {
                    if (await _serviceManager.ServiceExistsAsync(selectedApp.ServiceName))
                    {
                        var serviceLogs = await _serviceManager.ReadServiceLogsAsync(selectedApp);
                        if (!string.IsNullOrEmpty(serviceLogs) && serviceLogs != "No service logs found.")
                        {
                            logs += "\n\n=== SERVICE LOGS ===\n" + serviceLogs;
                        }
                    }
                }
                catch
                {
                    // Ignore service log errors
                }
                
                logTextBox.Text = logs;
                
                if (autoScrollCheckBox.Checked)
                {
                    logTextBox.ScrollToCaret();
                }
            }
        }
        
        private void LogFilter_TextChanged(object sender, EventArgs e)
        {
            // Refresh logs with filter
            LogAppsListBox_SelectedIndexChanged(sender, e);
        }
        
        private void ClearLogs_Click(object sender, EventArgs e)
        {
            var selectedApp = logAppsListBox.SelectedItem as NodeJsApplication;
            if (selectedApp != null)
            {
                _processManager.ClearApplicationLogs(selectedApp.Id);
                logTextBox.Clear();
            }
        }
        
        private async void RefreshTimer_Tick(object sender, EventArgs e)
        {
            // Update running status for all applications
            foreach (var app in _applications)
            {
                var wasRunning = app.IsRunning;
                app.IsRunning = _processManager.IsApplicationRunning(app.Id);
                
                if (wasRunning != app.IsRunning)
                {
                    await RefreshApplicationsList();
                }
            }
        }
        
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if we should minimize to tray instead of closing
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(2000, "AlwaysDown", "Application minimized to system tray", ToolTipIcon.Info);
            }
            else
            {
                // Clean up resources
                refreshTimer.Stop();
                _processManager?.Dispose();
                notifyIcon.Visible = false;
            }
        }
        
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon.Visible = true;
            }
        }
        
        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            notifyIcon.Visible = false;
        }
        
        private void ShowTrayMenuItem_Click(object sender, EventArgs e)
        {
            NotifyIcon_DoubleClick(sender, e);
        }
    }
}
