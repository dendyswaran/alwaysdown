using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinForm.Models;

namespace WinForm.Services
{
    public class NodeJsProcessManager
    {
        private readonly Dictionary<string, Process> _runningProcesses;
        private readonly Dictionary<string, StringBuilder> _processLogs;
        
        public event EventHandler<ProcessStatusChangedEventArgs>? ProcessStatusChanged;
        public event EventHandler<ProcessLogEventArgs>? ProcessLogReceived;
        
        public NodeJsProcessManager()
        {
            _runningProcesses = new Dictionary<string, Process>();
            _processLogs = new Dictionary<string, StringBuilder>();
        }
        
        public async Task<bool> StartApplicationAsync(NodeJsApplication app)
        {
            try
            {
                if (_runningProcesses.ContainsKey(app.Id))
                {
                    await StopApplicationAsync(app);
                }
                
                if (!Directory.Exists(app.ProjectPath))
                {
                    throw new DirectoryNotFoundException($"Project path not found: {app.ProjectPath}");
                }
                
                // Ensure log directory exists
                var logDir = Path.GetDirectoryName(app.LogFilePath);
                if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                
                ProcessStartInfo processInfo;
                
                // Try to start the process directly without cmd.exe wrapper when possible
                if (TryParseDirectCommand(app.StartCommand, out var fileName, out var arguments))
                {
                    LogMessage(app.Id, $"Starting directly: {fileName} {arguments}");
                    processInfo = new ProcessStartInfo
                    {
                        FileName = fileName,
                        Arguments = arguments,
                        WorkingDirectory = !string.IsNullOrEmpty(app.WorkingDirectory) ? app.WorkingDirectory : app.ProjectPath,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };
                }
                else
                {
                    LogMessage(app.Id, $"Starting with cmd wrapper: {app.StartCommand}");
                    processInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c {app.StartCommand}",
                        WorkingDirectory = !string.IsNullOrEmpty(app.WorkingDirectory) ? app.WorkingDirectory : app.ProjectPath,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };
                }
                
                // Add environment variables
                foreach (var envVar in app.EnvironmentVariables)
                {
                    processInfo.Environment[envVar.Key] = envVar.Value;
                }
                
                var process = new Process { StartInfo = processInfo };
                
                // Initialize log buffer
                _processLogs[app.Id] = new StringBuilder();
                
                // Set up output redirection
                process.OutputDataReceived += (sender, e) => HandleProcessOutput(app.Id, e.Data, false);
                process.ErrorDataReceived += (sender, e) => HandleProcessOutput(app.Id, e.Data, true);
                process.Exited += (sender, e) => HandleProcessExit(app.Id);
                process.EnableRaisingEvents = true;
                
                if (process.Start())
                {
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    
                    _runningProcesses[app.Id] = process;
                    
                    app.IsRunning = true;
                    app.ProcessId = process.Id;
                    app.LastStarted = DateTime.Now;
                    
                    ProcessStatusChanged?.Invoke(this, new ProcessStatusChangedEventArgs(app.Id, true, process.Id));
                    
                    LogMessage(app.Id, $"Application started successfully. PID: {process.Id}");
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                LogMessage(app.Id, $"Failed to start application: {ex.Message}", true);
                return false;
            }
        }
        
        public async Task<bool> StopApplicationAsync(NodeJsApplication app)
        {
            try
            {
                if (!_runningProcesses.TryGetValue(app.Id, out var process))
                {
                    app.IsRunning = false;
                    app.ProcessId = null;
                    return true;
                }
                
                LogMessage(app.Id, "Stopping application...");
                
                // Kill the entire process tree to ensure all child processes are terminated
                if (!process.HasExited)
                {
                    try
                    {
                        LogMessage(app.Id, $"Attempting to kill process tree for PID: {process.Id}");
                        
                        // First try to kill the entire process tree using taskkill
                        var success = await KillProcessTreeAsync(process.Id, app.Id);
                        
                        if (!success)
                        {
                            LogMessage(app.Id, "Taskkill failed, trying direct process termination...");
                            
                            // Fallback: try direct process kill
                            if (!process.HasExited)
                            {
                                process.Kill(true);
                                
                                // Wait for process termination with timeout
                                var killTask = Task.Run(() => process.WaitForExit(5000));
                                try
                                {
                                    await killTask.WaitAsync(TimeSpan.FromSeconds(6));
                                    LogMessage(app.Id, "Process terminated successfully");
                                }
                                catch (System.TimeoutException)
                                {
                                    LogMessage(app.Id, "Process termination timed out", true);
                                }
                            }
                        }
                        
                        // Additional cleanup: Kill any remaining processes by name if we know the command
                        await KillProcessesByCommandAsync(app.StartCommand, app.Id);
                    }
                    catch (Exception ex)
                    {
                        LogMessage(app.Id, $"Error during process termination: {ex.Message}", true);
                    }
                }
                
                _runningProcesses.Remove(app.Id);
                process.Dispose();
                
                app.IsRunning = false;
                app.ProcessId = null;
                app.LastStopped = DateTime.Now;
                
                ProcessStatusChanged?.Invoke(this, new ProcessStatusChangedEventArgs(app.Id, false, null));
                
                LogMessage(app.Id, "Application stop process completed.");
                return true;
            }
            catch (Exception ex)
            {
                LogMessage(app.Id, $"Error stopping application: {ex.Message}", true);
                return false;
            }
        }
        
        private async Task<bool> KillProcessTreeAsync(int processId, string appId)
        {
            try
            {
                LogMessage(appId, $"Executing: taskkill /F /T /PID {processId}");
                
                var processInfo = new ProcessStartInfo
                {
                    FileName = "taskkill",
                    Arguments = $"/F /T /PID {processId}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                
                using var taskkillProcess = new Process { StartInfo = processInfo };
                taskkillProcess.Start();
                
                var output = await taskkillProcess.StandardOutput.ReadToEndAsync();
                var error = await taskkillProcess.StandardError.ReadToEndAsync();
                
                await taskkillProcess.WaitForExitAsync();
                
                if (taskkillProcess.ExitCode == 0)
                {
                    LogMessage(appId, "Process tree killed successfully");
                    if (!string.IsNullOrEmpty(output))
                    {
                        LogMessage(appId, $"Taskkill output: {output.Trim()}");
                    }
                    return true;
                }
                else
                {
                    LogMessage(appId, $"Taskkill failed with exit code {taskkillProcess.ExitCode}", true);
                    if (!string.IsNullOrEmpty(error))
                    {
                        LogMessage(appId, $"Taskkill error: {error.Trim()}", true);
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogMessage(appId, $"Error executing taskkill: {ex.Message}", true);
                return false;
            }
        }
        
        private async Task KillProcessesByCommandAsync(string startCommand, string appId)
        {
            try
            {
                // Extract the main executable name from the command
                string processName = null;
                
                if (startCommand.Contains("node"))
                {
                    processName = "node";
                }
                else if (startCommand.Contains("java"))
                {
                    processName = "java";
                }
                else if (startCommand.Contains("python"))
                {
                    processName = "python";
                }
                else if (startCommand.Contains("npm"))
                {
                    processName = "node"; // npm usually spawns node processes
                }
                
                if (!string.IsNullOrEmpty(processName))
                {
                    LogMessage(appId, $"Attempting to kill any remaining {processName} processes...");
                    
                    var processInfo = new ProcessStartInfo
                    {
                        FileName = "taskkill",
                        Arguments = $"/F /IM {processName}.exe",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };
                    
                    using var taskkillProcess = new Process { StartInfo = processInfo };
                    taskkillProcess.Start();
                    
                    await taskkillProcess.WaitForExitAsync();
                    
                    if (taskkillProcess.ExitCode == 0)
                    {
                        LogMessage(appId, $"Successfully killed remaining {processName} processes");
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage(appId, $"Error killing processes by name: {ex.Message}", true);
            }
        }
        
        private bool TryParseDirectCommand(string command, out string fileName, out string arguments)
        {
            fileName = null;
            arguments = null;
            
            try
            {
                var parts = command.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return false;
                
                var firstPart = parts[0].ToLower();
                
                // Handle common direct commands
                if (firstPart == "node" || firstPart == "node.exe")
                {
                    fileName = "node";
                    arguments = string.Join(" ", parts.Skip(1));
                    return true;
                }
                else if (firstPart == "java" || firstPart == "java.exe")
                {
                    fileName = "java";
                    arguments = string.Join(" ", parts.Skip(1));
                    return true;
                }
                else if (firstPart == "python" || firstPart == "python.exe")
                {
                    fileName = "python";
                    arguments = string.Join(" ", parts.Skip(1));
                    return true;
                }
                else if (firstPart.EndsWith(".exe"))
                {
                    fileName = firstPart;
                    arguments = string.Join(" ", parts.Skip(1));
                    return true;
                }
                
                // Don't parse npm, yarn, etc. directly as they need shell processing
                return false;
            }
            catch
            {
                return false;
            }
        }
        
        public async Task<bool> RestartApplicationAsync(NodeJsApplication app)
        {
            LogMessage(app.Id, "Restarting application...");
            await StopApplicationAsync(app);
            await Task.Delay(1000); // Brief delay before restart
            return await StartApplicationAsync(app);
        }
        
        public bool IsApplicationRunning(string appId)
        {
            if (!_runningProcesses.TryGetValue(appId, out var process))
                return false;
                
            try
            {
                return !process.HasExited;
            }
            catch
            {
                return false;
            }
        }
        
        public string GetApplicationLogs(string appId)
        {
            if (_processLogs.TryGetValue(appId, out var logs))
            {
                return logs.ToString();
            }
            return string.Empty;
        }
        
        public void ClearApplicationLogs(string appId)
        {
            if (_processLogs.ContainsKey(appId))
            {
                _processLogs[appId].Clear();
            }
        }
        
        private void HandleProcessOutput(string appId, string? data, bool isError)
        {
            if (string.IsNullOrEmpty(data)) return;
            
            LogMessage(appId, data, isError);
        }
        
        private void HandleProcessExit(string appId)
        {
            if (_runningProcesses.TryGetValue(appId, out var process))
            {
                var exitCode = process.ExitCode;
                _runningProcesses.Remove(appId);
                
                LogMessage(appId, $"Application exited with code: {exitCode}", exitCode != 0);
                ProcessStatusChanged?.Invoke(this, new ProcessStatusChangedEventArgs(appId, false, null));
            }
        }
        
        private void LogMessage(string appId, string message, bool isError = false)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logLevel = isError ? "ERROR" : "INFO";
            var logEntry = $"[{timestamp}] [{logLevel}] {message}";
            
            if (_processLogs.TryGetValue(appId, out var logs))
            {
                logs.AppendLine(logEntry);
                
                // Keep only last 1000 lines to prevent memory issues
                var lines = logs.ToString().Split('\n');
                if (lines.Length > 1000)
                {
                    logs.Clear();
                    logs.AppendLine(string.Join('\n', lines.TakeLast(500)));
                }
            }
            
            ProcessLogReceived?.Invoke(this, new ProcessLogEventArgs(appId, logEntry, isError));
        }
        
        public void Dispose()
        {
            foreach (var process in _runningProcesses.Values)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill(true);
                    }
                    process.Dispose();
                }
                catch { }
            }
            _runningProcesses.Clear();
            _processLogs.Clear();
        }
    }
    
    public class ProcessStatusChangedEventArgs : EventArgs
    {
        public string ApplicationId { get; }
        public bool IsRunning { get; }
        public int? ProcessId { get; }
        
        public ProcessStatusChangedEventArgs(string applicationId, bool isRunning, int? processId)
        {
            ApplicationId = applicationId;
            IsRunning = isRunning;
            ProcessId = processId;
        }
    }
    
    public class ProcessLogEventArgs : EventArgs
    {
        public string ApplicationId { get; }
        public string LogEntry { get; }
        public bool IsError { get; }
        
        public ProcessLogEventArgs(string applicationId, string logEntry, bool isError)
        {
            ApplicationId = applicationId;
            LogEntry = logEntry;
            IsError = isError;
        }
    }
} 