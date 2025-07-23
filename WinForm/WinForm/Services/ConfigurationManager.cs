using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WinForm.Models;

namespace WinForm.Services
{
    public class ConfigurationManager
    {
        private readonly string _configDirectory;
        private readonly string _applicationsConfigFile;
        
        public ConfigurationManager()
        {
            _configDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AlwaysDown", "Config");
            _applicationsConfigFile = Path.Combine(_configDirectory, "applications.json");
            
            // Ensure config directory exists
            if (!Directory.Exists(_configDirectory))
            {
                Directory.CreateDirectory(_configDirectory);
            }
        }
        
        public async Task<List<NodeJsApplication>> LoadApplicationsAsync()
        {
            try
            {
                if (!File.Exists(_applicationsConfigFile))
                {
                    return new List<NodeJsApplication>();
                }
                
                var json = await File.ReadAllTextAsync(_applicationsConfigFile);
                var applications = JsonConvert.DeserializeObject<List<NodeJsApplication>>(json) ?? new List<NodeJsApplication>();
                
                return applications;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load applications configuration: {ex.Message}", ex);
            }
        }
        
        public async Task SaveApplicationsAsync(List<NodeJsApplication> applications)
        {
            try
            {
                var json = JsonConvert.SerializeObject(applications, Formatting.Indented);
                await File.WriteAllTextAsync(_applicationsConfigFile, json);
                
                // Save individual application configs for Windows services
                foreach (var app in applications)
                {
                    await SaveApplicationConfigAsync(app);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save applications configuration: {ex.Message}", ex);
            }
        }
        
        public async Task SaveApplicationConfigAsync(NodeJsApplication app)
        {
            try
            {
                var configPath = Path.Combine(_configDirectory, $"{app.Id}.json");
                var config = new
                {
                    app.Id,
                    app.Name,
                    app.ProjectPath,
                    app.StartCommand,
                    app.Description,
                    app.AutoStart,
                    app.EnvironmentVariables,
                    app.WorkingDirectory,
                    app.Port,
                    app.LogFilePath
                };
                
                var json = JsonConvert.SerializeObject(config, Formatting.Indented);
                await File.WriteAllTextAsync(configPath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save application configuration: {ex.Message}", ex);
            }
        }
        
        public async Task DeleteApplicationConfigAsync(string applicationId)
        {
            try
            {
                var configPath = Path.Combine(_configDirectory, $"{applicationId}.json");
                if (File.Exists(configPath))
                {
                    File.Delete(configPath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete application configuration: {ex.Message}", ex);
            }
        }
        
        public async Task<AppSettings> LoadAppSettingsAsync()
        {
            try
            {
                var settingsPath = Path.Combine(_configDirectory, "settings.json");
                
                if (!File.Exists(settingsPath))
                {
                    var defaultSettings = new AppSettings();
                    await SaveAppSettingsAsync(defaultSettings);
                    return defaultSettings;
                }
                
                var json = await File.ReadAllTextAsync(settingsPath);
                return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load application settings: {ex.Message}", ex);
            }
        }
        
        public async Task SaveAppSettingsAsync(AppSettings settings)
        {
            try
            {
                var settingsPath = Path.Combine(_configDirectory, "settings.json");
                var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                await File.WriteAllTextAsync(settingsPath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save application settings: {ex.Message}", ex);
            }
        }
        
        public string GetLogDirectory()
        {
            var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AlwaysDown", "Logs");
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            return logDir;
        }
        
        public void CleanupOldLogs(int daysToKeep = 30)
        {
            try
            {
                var logDir = GetLogDirectory();
                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
                
                var logFiles = Directory.GetFiles(logDir, "*.log");
                foreach (var logFile in logFiles)
                {
                    var fileInfo = new FileInfo(logFile);
                    if (fileInfo.CreationTime < cutoffDate)
                    {
                        File.Delete(logFile);
                    }
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
    
    public class AppSettings
    {
        public bool StartMinimized { get; set; } = false;
        public bool MinimizeToTray { get; set; } = true;
        public bool AutoStartApplications { get; set; } = true;
        public int LogRetentionDays { get; set; } = 30;
        public bool EnableLogging { get; set; } = true;
        public string DefaultNodeJsPath { get; set; } = "node";
        public string DefaultNpmPath { get; set; } = "npm";
        public bool CheckForUpdates { get; set; } = true;
        public int RefreshInterval { get; set; } = 5000; // milliseconds
        public Dictionary<string, string> GlobalEnvironmentVariables { get; set; } = new Dictionary<string, string>();
    }
} 