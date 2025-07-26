using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace WinForm.Models
{
    public class NodeJsApplication
    {
        private string _name = string.Empty;
        
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public string Name 
        { 
            get => _name;
            set 
            {
                _name = value;
                // Update service name when name changes
                UpdateServiceName();
            }
        }
        
        private string _projectPath = string.Empty;
        
        public string ProjectPath 
        { 
            get => _projectPath;
            set 
            {
                _projectPath = value;
                // Update service name when path changes (in case name is derived from path)
                if (string.IsNullOrEmpty(Name))
                {
                    UpdateServiceName();
                }
            }
        }
        public string StartCommand { get; set; } = "npm run start";
        public string Description { get; set; } = string.Empty;
        public bool AutoStart { get; set; } = false;
        public bool IsRunning { get; set; } = false;
        public DateTime? LastStarted { get; set; }
        public DateTime? LastStopped { get; set; }
        public int? ProcessId { get; set; }
        public string LogFilePath { get; set; } = string.Empty;
        public Dictionary<string, string> EnvironmentVariables { get; set; } = new Dictionary<string, string>();
        public string WorkingDirectory { get; set; } = string.Empty;
        public int Port { get; set; } = 3000;
        public string ServiceName { get; set; } = string.Empty;
        
        public NodeJsApplication()
        {
            UpdateServiceName();
            UpdateLogFilePath();
        }
        
        private void UpdateServiceName()
        {
            ServiceName = GenerateServiceName();
            UpdateLogFilePath();
        }
        
        private void UpdateLogFilePath()
        {
            LogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                "AlwaysDown", "Logs", $"{ServiceName}.log");
        }
        
        private string GenerateServiceName()
        {
            var baseName = !string.IsNullOrEmpty(Name) ? Name : Path.GetFileName(ProjectPath);
            
            if (string.IsNullOrEmpty(baseName))
            {
                baseName = "UnnamedApp";
            }
            
            // Clean the name to make it suitable for a Windows service name
            // Remove invalid characters and replace with underscores
            var cleanName = Regex.Replace(baseName, @"[^\w\-_\.]", "_");
            
            // Remove multiple consecutive underscores
            cleanName = Regex.Replace(cleanName, @"_{2,}", "_");
            
            // Trim underscores from start and end
            cleanName = cleanName.Trim('_');
            
            // Ensure it's not empty after cleaning
            if (string.IsNullOrEmpty(cleanName))
            {
                cleanName = "CleanedApp";
            }
            
            // Prefix with AlwaysDown to avoid conflicts and make it identifiable
            return $"AlwaysDown_{cleanName}";
        }
        
        public string GetDisplayName()
        {
            return !string.IsNullOrEmpty(Name) ? Name : Path.GetFileName(ProjectPath);
        }
        
        public string GetStatus()
        {
            if (IsRunning && ProcessId.HasValue)
                return $"Running (PID: {ProcessId})";
            return "Stopped";
        }
    }
} 