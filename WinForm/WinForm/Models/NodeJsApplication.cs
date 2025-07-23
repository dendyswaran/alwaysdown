using System;
using System.Collections.Generic;
using System.IO;

namespace WinForm.Models
{
    public class NodeJsApplication
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string ProjectPath { get; set; } = string.Empty;
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
            ServiceName = $"AlwaysDownApp_{Id.Replace("-", "")}";
            LogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                "AlwaysDown", "Logs", $"{ServiceName}.log");
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