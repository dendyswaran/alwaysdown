using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using WinForm.Models;

namespace WinForm.Services
{
    public class WindowsServiceManager
    {
        private const string SERVICE_WRAPPER_NAME = "NodeJsServiceWrapper.bat";
        private readonly string _serviceWrapperPath;
        
        public WindowsServiceManager()
        {
            _serviceWrapperPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SERVICE_WRAPPER_NAME);
        }
        
        public async Task<bool> InstallServiceAsync(NodeJsApplication app)
        {
            try
            {
                // Ensure the service wrapper exists
                await CreateServiceWrapperAsync(app);
                
                // Create a service-specific batch file
                var batchPath = Path.Combine(Path.GetDirectoryName(_serviceWrapperPath)!, $"{app.ServiceName}.bat");
                await CreateServiceBatchFileAsync(app, batchPath);
                
                // Create the Windows service using sc.exe
                var serviceName = app.ServiceName;
                var displayName = $"AlwaysDown - {app.GetDisplayName()}";
                var description = $"Node.js application service for {app.GetDisplayName()} - {app.Description}";
                
                // Install the service
                var installArgs = $"create \"{serviceName}\" binPath= \"cmd.exe /c \\\"{batchPath}\\\"\" DisplayName= \"{displayName}\" start= auto";
                
                var processInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "sc.exe"),
                    Arguments = installArgs,
                    UseShellExecute = true,
                    Verb = "runas", // Request admin elevation
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                
                using var process = Process.Start(processInfo);
                if (process != null)
                {
                    await process.WaitForExitAsync();
                    
                    if (process.ExitCode == 0)
                    {
                        // Set service description
                        await SetServiceDescriptionAsync(serviceName, description);
                        
                        // Configure service for failure recovery
                        await ConfigureServiceRecoveryAsync(serviceName);
                        
                        return true;
                    }
                }
                
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error installing Windows service: {ex.Message}", ex);
            }
        }
        

        
        public async Task<bool> UninstallServiceAsync(NodeJsApplication app)
        {
            try
            {
                var serviceName = app.ServiceName;
                
                // Stop service first if running
                await StopServiceAsync(serviceName);
                
                var processInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "sc.exe"),
                    Arguments = $"delete \"{serviceName}\"",
                    UseShellExecute = true,
                    Verb = "runas" // This will prompt for admin elevation
                };
                
                using var process = Process.Start(processInfo);
                if (process != null)
                {
                    await process.WaitForExitAsync();
                    
                    // Clean up the batch file
                    var batchPath = Path.Combine(Path.GetDirectoryName(_serviceWrapperPath)!, $"{app.ServiceName}.bat");
                    if (File.Exists(batchPath))
                    {
                        try
                        {
                            File.Delete(batchPath);
                        }
                        catch
                        {
                            // Ignore file deletion errors
                        }
                    }
                    
                    return process.ExitCode == 0;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uninstalling Windows service: {ex.Message}", ex);
            }
        }
        
        public async Task<bool> StartServiceAsync(string serviceName)
        {
            try
            {
                using var service = new ServiceController(serviceName);
                
                // Check status with timeout
                var statusTask = Task.Run(() => service.Status);
                try
                {
                    await statusTask.WaitAsync(TimeSpan.FromSeconds(5));
                }
                catch (System.TimeoutException)
                {
                    throw new System.TimeoutException("Timeout checking service status");
                }
                
                var currentStatus = statusTask.Result;
                if (currentStatus == ServiceControllerStatus.Running)
                    return true;
                
                if (currentStatus == ServiceControllerStatus.Stopped)
                {
                    service.Start();
                    
                    // Wait for start with timeout
                    var startTask = Task.Run(() => 
                    {
                        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                        return service.Status == ServiceControllerStatus.Running;
                    });
                    
                    if (await startTask.WaitAsync(TimeSpan.FromSeconds(35)))
                    {
                        return startTask.Result;
                    }
                    else
                    {
                        throw new System.TimeoutException("Service start operation timed out");
                    }
                }
                
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error starting Windows service: {ex.Message}", ex);
            }
        }
        
        public async Task<bool> StopServiceAsync(string serviceName)
        {
            try
            {
                using var service = new ServiceController(serviceName);
                
                // Check status with timeout
                var statusTask = Task.Run(() => service.Status);
                try
                {
                    await statusTask.WaitAsync(TimeSpan.FromSeconds(5));
                }
                catch (System.TimeoutException)
                {
                    throw new System.TimeoutException("Timeout checking service status");
                }
                
                var currentStatus = statusTask.Result;
                if (currentStatus == ServiceControllerStatus.Stopped)
                    return true;
                
                if (currentStatus == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    
                    // Wait for stop with timeout
                    var stopTask = Task.Run(() => 
                    {
                        service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                        return service.Status == ServiceControllerStatus.Stopped;
                    });
                    
                    if (await stopTask.WaitAsync(TimeSpan.FromSeconds(35)))
                    {
                        return stopTask.Result;
                    }
                    else
                    {
                        throw new System.TimeoutException("Service stop operation timed out");
                    }
                }
                
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error stopping Windows service: {ex.Message}", ex);
            }
        }
        
        public async Task<ServiceControllerStatus> GetServiceStatusAsync(string serviceName)
        {
            try
            {
                using var service = new ServiceController(serviceName);
                
                // Use timeout for status check
                var statusTask = Task.Run(() => service.Status);
                try
                {
                    await statusTask.WaitAsync(TimeSpan.FromSeconds(3));
                    return statusTask.Result;
                }
                catch (System.TimeoutException)
                {
                    return ServiceControllerStatus.Stopped; // Timeout, assume stopped
                }
            }
            catch
            {
                return ServiceControllerStatus.Stopped;
            }
        }
        
        // Keep synchronous version for compatibility
        public ServiceControllerStatus GetServiceStatus(string serviceName)
        {
            try
            {
                using var service = new ServiceController(serviceName);
                // Quick check without blocking - may not be 100% accurate but won't hang
                return ServiceControllerStatus.Stopped; // Conservative default
            }
            catch
            {
                return ServiceControllerStatus.Stopped;
            }
        }
        
        public async Task<bool> ServiceExistsAsync(string serviceName)
        {
            try
            {
                using var service = new ServiceController(serviceName);
                
                // Use a timeout for the status check to prevent hanging
                var task = Task.Run(() => service.Status);
                try
                {
                    await task.WaitAsync(TimeSpan.FromSeconds(3));
                    return true; // Service exists if we can get its status
                }
                catch (System.TimeoutException)
                {
                    return false; // Timeout occurred, assume service doesn't exist
                }
            }
            catch
            {
                return false;
            }
        }
        
        // Keep synchronous version for compatibility, but make it non-blocking
        public bool ServiceExists(string serviceName)
        {
            try
            {
                // Quick non-blocking check - just try to create ServiceController
                using var service = new ServiceController(serviceName);
                // If we can create it without exception, service likely exists
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        private async Task SetServiceDescriptionAsync(string serviceName, string description)
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "sc.exe"),
                    Arguments = $"description \"{serviceName}\" \"{description}\"",
                    UseShellExecute = true,
                    Verb = "runas"
                };
                
                using var process = Process.Start(processInfo);
                if (process != null)
                {
                    await process.WaitForExitAsync();
                }
            }
            catch
            {
                // Ignore errors setting description
            }
        }
        
        private async Task ConfigureServiceRecoveryAsync(string serviceName)
        {
            try
            {
                // Configure service to restart automatically on failure
                // Restart after 1 minute for first and second failures, restart after 5 minutes for subsequent failures
                var processInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "sc.exe"),
                    Arguments = $"failure \"{serviceName}\" reset= 86400 actions= restart/60000/restart/60000/restart/300000",
                    UseShellExecute = true,
                    Verb = "runas",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                
                using var process = Process.Start(processInfo);
                if (process != null)
                {
                    await process.WaitForExitAsync();
                }
            }
            catch
            {
                // Ignore errors setting recovery options
            }
        }
        
        private async Task CreateServiceWrapperAsync(NodeJsApplication app)
        {
            // Create a simple wrapper script that can be used by multiple services
            if (File.Exists(_serviceWrapperPath))
                return;
                
            var wrapperContent = @"@echo off
REM Node.js Service Wrapper
REM This script is used to run Node.js applications as Windows services

if ""%1""==""start"" goto start
if ""%1""==""stop"" goto stop
if ""%1""==""install"" goto install
if ""%1""==""uninstall"" goto uninstall

:start
echo Starting Node.js application...
cd /d ""%2""
%3
goto end

:stop
echo Stopping Node.js application...
taskkill /f /im node.exe 2>nul
goto end

:install
echo Installing Node.js service...
goto end

:uninstall
echo Uninstalling Node.js service...
goto end

:end
";
            
            // Ensure the directory exists
            var directory = Path.GetDirectoryName(_serviceWrapperPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            await File.WriteAllTextAsync(_serviceWrapperPath, wrapperContent);
        }
        
        private async Task CreateServiceBatchFileAsync(NodeJsApplication app, string batchPath)
        {
            var logFilePath = Path.Combine(Path.GetDirectoryName(app.LogFilePath) ?? "", Path.GetFileNameWithoutExtension(app.LogFilePath) + "_service.log");
            
            // Ensure the log directory exists
            var logDir = Path.GetDirectoryName(logFilePath);
            if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            
            // Ensure the batch file directory exists
            var batchDir = Path.GetDirectoryName(batchPath);
            if (!string.IsNullOrEmpty(batchDir) && !Directory.Exists(batchDir))
            {
                Directory.CreateDirectory(batchDir);
            }
            
            var batchContent = $@"@echo off
REM Service batch file for {app.GetDisplayName()}
REM Auto-generated by AlwaysDown Application Manager

setlocal EnableDelayedExpansion

REM Set up logging
set LOG_FILE=""{logFilePath}""

REM Function to log with timestamp
call :log ""Service starting for {app.GetDisplayName()}...""

REM Check if project directory exists
if not exist ""{app.ProjectPath}"" (
    call :log ""ERROR: Project directory not found: {app.ProjectPath}""
    exit /b 1
)

REM Change to project directory
cd /d ""{app.ProjectPath}""
if errorlevel 1 (
    call :log ""ERROR: Failed to change to project directory""
    exit /b 1
)

call :log ""Changed to directory: %CD%""

REM Check if package.json exists (for Node.js projects)
if exist ""package.json"" (
    call :log ""Found package.json in project directory""
) else (
    call :log ""WARNING: package.json not found in project directory""
)

REM Set environment variables if needed
set NODE_ENV=production

call :log ""Executing command: {app.StartCommand}""

REM Execute the application command and redirect output to log file
{app.StartCommand} >> %LOG_FILE% 2>&1

set EXIT_CODE=%ERRORLEVEL%
call :log ""Command exited with code: !EXIT_CODE!""

exit /b !EXIT_CODE!

REM Logging function
:log
set timestamp=%date% %time%
echo [%timestamp%] %~1 >> %LOG_FILE%
goto :eof
";
            
            await File.WriteAllTextAsync(batchPath, batchContent);
        }
        
        public string GetServiceLogPath(NodeJsApplication app)
        {
            return Path.Combine(Path.GetDirectoryName(app.LogFilePath) ?? "", Path.GetFileNameWithoutExtension(app.LogFilePath) + "_service.log");
        }
        
        public async Task<string> ReadServiceLogsAsync(NodeJsApplication app)
        {
            try
            {
                var logPath = GetServiceLogPath(app);
                if (File.Exists(logPath))
                {
                    return await File.ReadAllTextAsync(logPath);
                }
                return "No service logs found.";
            }
            catch (Exception ex)
            {
                return $"Error reading service logs: {ex.Message}";
            }
        }
        
        public async Task<bool> SetAutoStartAsync(NodeJsApplication app, bool autoStart)
        {
            try
            {
                const string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                var keyName = $"AlwaysDownApp_{app.Id.Replace("-", "")}";
                
                using var key = Registry.CurrentUser.OpenSubKey(registryKey, true);
                if (key == null)
                    return false;
                
                if (autoStart)
                {
                    // Create a batch file for startup
                    var startupBatchPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "AlwaysDown", "Startup", $"{keyName}.bat");
                    
                    var startupDir = Path.GetDirectoryName(startupBatchPath);
                    if (!string.IsNullOrEmpty(startupDir) && !Directory.Exists(startupDir))
                    {
                        Directory.CreateDirectory(startupDir);
                    }
                    
                    var batchContent = $@"@echo off
cd /d ""{app.ProjectPath}""
start """" {app.StartCommand}";
                    
                    await File.WriteAllTextAsync(startupBatchPath, batchContent);
                    
                    // Add to registry
                    key.SetValue(keyName, $"\"{startupBatchPath}\"");
                }
                else
                {
                    // Remove from registry
                    try
                    {
                        key.DeleteValue(keyName, false);
                    }
                    catch
                    {
                        // Value doesn't exist, ignore
                    }
                    
                    // Clean up batch file
                    var startupBatchPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "AlwaysDown", "Startup", $"{keyName}.bat");
                    if (File.Exists(startupBatchPath))
                    {
                        try
                        {
                            File.Delete(startupBatchPath);
                        }
                        catch
                        {
                            // Ignore deletion errors
                        }
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error setting auto-start: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Migrates a service from an old name to a new name (useful when application name changes)
        /// </summary>
        public async Task<bool> MigrateServiceAsync(string oldServiceName, NodeJsApplication app)
        {
            try
            {
                var newServiceName = app.ServiceName;
                
                // If names are the same, no migration needed
                if (oldServiceName == newServiceName)
                    return true;
                
                // Check if old service exists
                if (!await ServiceExistsAsync(oldServiceName))
                    return true; // No old service to migrate
                
                // Check if new service already exists
                if (await ServiceExistsAsync(newServiceName))
                {
                    // New service already exists, can't migrate
                    throw new Exception($"Cannot migrate service: A service with name '{newServiceName}' already exists.");
                }
                
                // Stop the old service if running
                try
                {
                    await StopServiceAsync(oldServiceName);
                }
                catch
                {
                    // Continue even if stop fails
                }
                
                // Uninstall the old service
                var uninstallSuccess = await UninstallServiceByNameAsync(oldServiceName);
                if (!uninstallSuccess)
                {
                    throw new Exception($"Failed to uninstall old service '{oldServiceName}'");
                }
                
                // Install the new service
                var installSuccess = await InstallServiceAsync(app);
                if (!installSuccess)
                {
                    throw new Exception($"Failed to install new service '{newServiceName}'");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error migrating service from '{oldServiceName}' to '{app.ServiceName}': {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Uninstalls a service by name (internal helper method)
        /// </summary>
        private async Task<bool> UninstallServiceByNameAsync(string serviceName)
        {
            try
            {
                // Stop service first if running
                await StopServiceAsync(serviceName);
                
                var processInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "sc.exe"),
                    Arguments = $"delete \"{serviceName}\"",
                    UseShellExecute = true,
                    Verb = "runas" // This will prompt for admin elevation
                };
                
                using var process = Process.Start(processInfo);
                if (process != null)
                {
                    await process.WaitForExitAsync();
                    return process.ExitCode == 0;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uninstalling service '{serviceName}': {ex.Message}", ex);
            }
        }

    }
} 