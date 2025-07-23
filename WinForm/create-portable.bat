@echo off
echo ========================================
echo   AlwaysDown - Portable Package Creator
echo ========================================
echo.

cd /d "%~dp0"

REM Check if release build exists
if not exist "Release\AlwaysDown-Standalone\AlwaysDown.exe" (
    echo Release build not found. Running build-release.bat first...
    call build-release.bat
    if errorlevel 1 (
        echo ERROR: Build failed
        pause
        exit /b 1
    )
)

REM Check if PowerShell is available for ZIP creation
powershell -Command "Get-Command Compress-Archive" >nul 2>&1
if errorlevel 1 (
    echo PowerShell with Compress-Archive is not available.
    echo Please create ZIP files manually from the Release folder.
    pause
    exit /b 1
)

echo Creating portable packages...

REM Create standalone portable package
echo [1/2] Creating standalone portable package...
powershell -Command "Compress-Archive -Path 'Release\AlwaysDown-Standalone\*' -DestinationPath 'Release\AlwaysDown-v1.1.0-Portable-Standalone.zip' -Force"

REM Create framework-dependent portable package
echo [2/2] Creating framework-dependent portable package...
powershell -Command "Compress-Archive -Path 'Release\AlwaysDown-FrameworkDependent\*' -DestinationPath 'Release\AlwaysDown-v1.1.0-Portable-FrameworkDependent.zip' -Force"

REM Create a README for the portable packages
(
echo AlwaysDown v1.1.0 - Portable Distribution
echo ========================================
echo.
echo This package contains AlwaysDown in portable format.
echo No installation required - just extract and run!
echo.
echo CONTENTS:
echo ---------
echo AlwaysDown.exe         - Main application
echo *.dll                  - Required libraries
echo README.md              - Full documentation
echo USAGE_EXAMPLE.md       - Quick start guide
echo.
echo SYSTEM REQUIREMENTS:
echo -------------------
echo - Windows 10/11 ^(64-bit^)
echo - Administrator privileges ^(for some features^)
echo.
echo STANDALONE VERSION:
echo - Includes .NET 8.0 runtime
echo - Works on any Windows machine
echo - Larger file size ^(~100MB^)
echo.
echo FRAMEWORK-DEPENDENT VERSION:
echo - Requires .NET 8.0 Desktop Runtime
echo - Download from: https://dotnet.microsoft.com/download/dotnet/8.0
echo - Smaller file size ^(~1MB^)
echo.
echo FIRST RUN:
echo ----------
echo 1. Extract all files to a folder
echo 2. Right-click AlwaysDown.exe
echo 3. Select "Run as administrator"
echo 4. Follow the usage guide in USAGE_EXAMPLE.md
echo.
echo FEATURES:
echo ---------
echo ✓ Manage Node.js applications
echo ✓ Manage Java applications ^(java -jar^)
echo ✓ Manage Python applications
echo ✓ Auto-start with Windows
echo ✓ Real-time log viewing
echo ✓ System tray integration
echo.
echo For support and updates, visit:
echo https://github.com/your-username/AlwaysDown
) > "Release\PORTABLE-README.txt"

echo.
echo ========================================
echo   Portable packages created successfully!
echo ========================================
echo.
echo Output files:
echo   - AlwaysDown-v1.1.0-Portable-Standalone.zip        (~100MB)
echo   - AlwaysDown-v1.1.0-Portable-FrameworkDependent.zip (~1MB)
echo   - PORTABLE-README.txt                              (Instructions)
echo.
echo These ZIP files can be distributed and run on any Windows machine.
echo Users just need to extract and run AlwaysDown.exe as administrator.
echo.
pause 