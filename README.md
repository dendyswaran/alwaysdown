# AlwaysDown

Keep your applications running - always down, never down! A comprehensive Windows Forms application for deploying, managing, and monitoring various applications (Node.js, Java, Python, etc.) with persistence across PC sessions.

## Features

### 🚀 Application Management
- **Deploy Any Application**: Add and configure Node.js, Java, Python, or any command-line applications
- **Process Control**: Start, stop, and restart applications with real-time status monitoring
- **Auto-Start Support**: Configure applications to start automatically with Windows
- **Configuration Management**: Save and load application configurations with JSON-based storage

### 📊 Monitoring & Logging
- **Real-time Status**: Monitor running status, process IDs, and last start/stop times
- **Live Logs**: View application logs in real-time with color-coded output
- **Log Filtering**: Filter log output by keywords for easier debugging
- **Auto-scroll**: Automatically scroll to latest log entries

### 🔧 System Integration
- **Windows Services**: Convert applications into Windows services with meaningful names for persistence
- **Service Name Generation**: Services are named using your application name (e.g., "AlwaysDown_MyWebApp" instead of random IDs)
- **Service Migration**: Automatically handles service name changes when you rename applications
- **System Tray**: Minimize to system tray for background operation
- **Administrator Privileges**: Automatic elevation for service management operations
- **Auto-start**: Configure applications to start automatically with Windows

### 💻 User Interface
- **Modern UI**: Clean, intuitive Windows Forms interface with tabbed layout
- **Context Menus**: Right-click context menus for quick actions
- **Keyboard Shortcuts**: Standard Windows keyboard shortcuts support
- **Responsive Design**: Resizable windows with proper layout management

## System Requirements

- **Operating System**: Windows 10 or later
- **Framework**: .NET 8.0 Runtime
- **Privileges**: Administrator rights (for Windows service management)
- **Node.js**: Node.js runtime installed on the system
- **NPM**: NPM package manager (typically included with Node.js)

## Installation

1. **Download** the latest release from the releases page
2. **Extract** the application files to a folder of your choice
3. **Run** `AlwaysDown.exe` as Administrator (required for service management)

## Usage

### Adding a New Application

1. Click **"Add App"** button or use **File → Add Application**
2. Fill in the application details:
   - **Name**: Display name for your application (this will be used for the service name)
   - **Description**: Optional description
   - **Project Path**: Path to your Node.js project directory
   - **Start Command**: Command to start your application (e.g., `npm run start`)
   - **Port**: Port number your application uses
   - **Auto Start**: Enable to start with Windows
3. Click **"Save"** to save the configuration

### Managing Applications

#### Starting/Stopping Applications
- Select an application from the list
- Use toolbar buttons: **Start**, **Stop**, **Restart**
- Or right-click for context menu options

#### Installing as Windows Service
- Select an application
- Click **"Install Service"** button
- The application will be installed as a Windows service with a meaningful name
- Service name format: `AlwaysDown_[YourAppName]` (e.g., "AlwaysDown_WebAPI", "AlwaysDown_ChatBot")
- Service will automatically start on system boot
- If you rename your application, the service will be automatically migrated to the new name

#### Service Name Examples
```
Application Name: "My Web API"     → Service Name: "AlwaysDown_My_Web_API"
Application Name: "ChatBot"        → Service Name: "AlwaysDown_ChatBot"
Application Name: "Data Processor" → Service Name: "AlwaysDown_Data_Processor"
```

#### Viewing Logs
1. Switch to the **"Logs"** tab
2. Select an application from the left panel
3. View real-time logs in the right panel
4. Use the filter box to search log content
5. Enable **"Auto Scroll"** for continuous monitoring

### Configuration Files

The application stores configurations in:
```
%LocalAppData%\AlwaysDown\Config\
├── applications.json          # Main applications list
├── settings.json             # Application settings
└── [app-id].json            # Individual app configs for services
```

Logs are stored in:
```
%LocalAppData%\AlwaysDown\Logs\
└── [service-name].log       # Individual application logs (named after service)
```

## Architecture

### Core Components

- **NodeJsProcessManager**: Manages Node.js processes, handles start/stop/restart operations (turns out it also supports python and java)
- **WindowsServiceManager**: Handles Windows service installation and management
- **ConfigurationManager**: Manages application configuration and settings persistence
- **NodeJsApplication**: Data model representing a Node.js application

### Service Architecture

When an application is installed as a Windows service:

1. A service host executable (`NodeJsServiceHost.exe`) is created
2. The Windows service is registered with the system
3. Service configuration is stored in JSON format
4. The service automatically restarts failed applications
5. Logs are written to Windows Event Log and local files

## Development

### Building from Source

1. **Clone** the repository
2. **Open** the solution in Visual Studio 2022 or later
3. **Restore** NuGet packages
4. **Build** the solution (Debug or Release configuration)

### Dependencies

- **Newtonsoft.Json**: JSON serialization and deserialization
- **System.ServiceProcess.ServiceController**: Windows service management
- **Microsoft.Extensions.Hosting.WindowsServices**: Windows service hosting

## Building Release

The project includes automated build scripts for creating different types of release packages. All scripts are located in the root directory and should be run from a command prompt with administrator privileges.

### Prerequisites

- **.NET 8.0 SDK**: Required for building the application
- **Windows 10/11**: Build environment
- **Administrator privileges**: Required for some build operations
- **NSIS (Optional)**: For creating installers - download from [https://nsis.sourceforge.io/](https://nsis.sourceforge.io/)

### Release Build Process

#### 1. Basic Release Build

Run the main build script to create both standalone and framework-dependent versions:

```batch
build-release.bat
```

This script performs the following steps:
1. **Cleans** previous builds (removes existing `Release` folder)
2. **Restores** NuGet dependencies
3. **Builds** the Release configuration
4. **Publishes** self-contained executable (~100MB, includes .NET runtime)
5. **Publishes** framework-dependent version (~1MB, requires .NET 8.0 Runtime)
6. **Copies** documentation files (README.md, USAGE_EXAMPLE.md, LICENSE)
7. **Renames** executables to `AlwaysDown.exe`

**Output Structure:**
```
Release/
├── AlwaysDown-Standalone/           # Self-contained version
│   ├── AlwaysDown.exe              # Main executable
│   ├── *.dll                       # Runtime libraries
│   └── WinForm.pdb                 # Debug symbols
├── AlwaysDown-FrameworkDependent/  # Framework-dependent version
│   ├── AlwaysDown.exe              # Main executable
│   ├── *.dll                       # Application libraries
│   └── WinForm.pdb                 # Debug symbols
├── README.md                       # Documentation
├── USAGE_EXAMPLE.md               # Quick start guide
└── LICENSE                        # License file
```

#### 2. Creating Portable Packages

Create ZIP packages for easy distribution:

```batch
create-portable.bat
```

This script:
- **Automatically runs** `build-release.bat` if needed
- **Creates** ZIP packages using PowerShell compression
- **Generates** portable README with setup instructions

**Output Files:**
- `AlwaysDown-v1.1.0-Portable-Standalone.zip` (~100MB)
- `AlwaysDown-v1.1.0-Portable-FrameworkDependent.zip` (~1MB)
- `PORTABLE-README.txt` (Setup instructions)

#### 3. Creating Windows Installer

Create a professional Windows installer (requires NSIS):

```batch
create-installer.bat
```

This script:
- **Checks** for NSIS installation
- **Automatically runs** `build-release.bat` if needed
- **Generates** NSIS installer script
- **Compiles** installer executable

**Output:**
- `AlwaysDown-Setup-v1.1.0.exe` (Windows installer)

**Installer Features:**
- Professional installation wizard
- Desktop and Start Menu shortcuts
- Add/Remove Programs integration
- Clean uninstallation support

### Release Types Comparison

| Type | Size | Dependencies | Use Case |
|------|------|--------------|----------|
| **Standalone** | ~100MB | None | End users, no .NET installed |
| **Framework-Dependent** | ~1MB | .NET 8.0 Runtime | Developers, .NET already installed |
| **Installer** | ~100MB | None | Professional deployment |

### Manual Build Commands

For advanced users, you can build manually using .NET CLI:

```batch
# Clean and restore
dotnet clean WinForm\WinForm.csproj
dotnet restore WinForm\WinForm.csproj

# Build Release configuration
dotnet build WinForm\WinForm.csproj -c Release

# Publish self-contained
dotnet publish WinForm\WinForm.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -o Output\Standalone

# Publish framework-dependent
dotnet publish WinForm\WinForm.csproj -c Release -r win-x64 --self-contained false -o Output\FrameworkDependent
```

### Build Troubleshooting

**Common Issues:**

- **"dotnet command not found"**: Install .NET 8.0 SDK
- **"Access denied"**: Run command prompt as Administrator
- **"NSIS not found"**: Install NSIS and add to PATH, or use portable packages instead
- **"Build failed"**: Check that all NuGet packages are restored correctly

**Build Environment:**
- Ensure Windows Defender or antivirus isn't blocking the build process
- Check that the `WinForm` folder structure is intact
- Verify all project files (.csproj, .cs) are present

### Project Structure

```
WinForm/
├── Models/
│   └── NodeJsApplication.cs     # Application data model
├── Services/
│   ├── NodeJsProcessManager.cs  # Process management
│   ├── WindowsServiceManager.cs # Service management
│   └── ConfigurationManager.cs  # Configuration handling
├── Form1.cs                     # Main form logic
├── Form1.Designer.cs            # UI design
├── Program.cs                   # Application entry point
├── WinForm.csproj              # Project configuration
└── app.manifest                # Administrator privileges manifest
```

## Troubleshooting

### Common Issues

**Application won't start as Administrator**
- Ensure the application manifest requests administrator privileges
- Right-click the executable and select "Run as administrator"

**Node.js application won't start**
- Verify Node.js is installed and accessible from PATH
- Check that the project path contains a valid Node.js application
- Ensure the start command is correct (e.g., `npm run start`)

**Windows service installation fails**
- Ensure running as Administrator
- Check Windows Event Log for detailed error messages
- Verify the service name doesn't conflict with existing services

**Logs not appearing**
- Check that the application is actually running
- Verify log file permissions in `%LocalAppData%\NodeJsManager\Logs\`
- Ensure the Node.js application outputs to stdout/stderr

### Log Locations

- **Application Logs**: `%LocalAppData%\AlwaysDown\Logs\`
- **Windows Event Log**: Look for events from services starting with "AlwaysDownApp_"
- **Configuration**: `%LocalAppData%\AlwaysDown\Config\`

## Security Considerations

- The application requires Administrator privileges for Windows service management
- Service executables are created in the application directory
- Configuration files may contain sensitive information (paths, commands)
- Network ports specified in applications should be properly secured

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## Support

For issues, questions, or feature requests, please create an issue in the GitHub repository.

---

**Note**: This application is designed for development and testing environments. For production deployments, consider additional security hardening and monitoring solutions. 