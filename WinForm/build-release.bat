@echo off
echo ========================================
echo    AlwaysDown - Release Builder
echo ========================================
echo.

cd /d "%~dp0"

REM Clean previous builds
echo [1/6] Cleaning previous builds...
if exist "Release" rmdir /s /q "Release"
mkdir "Release"

REM Restore dependencies
echo [2/6] Restoring dependencies...
dotnet restore WinForm\WinForm.csproj
if errorlevel 1 (
    echo ERROR: Failed to restore dependencies
    pause
    exit /b 1
)

REM Build Release configuration
echo [3/6] Building Release configuration...
dotnet build WinForm\WinForm.csproj -c Release --no-restore
if errorlevel 1 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

REM Publish self-contained executable
echo [4/6] Publishing self-contained executable...
dotnet publish WinForm\WinForm.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -o "Release\AlwaysDown-Standalone"
if errorlevel 1 (
    echo ERROR: Publish failed
    pause
    exit /b 1
)

REM Publish framework-dependent version
echo [5/6] Publishing framework-dependent version...
dotnet publish WinForm\WinForm.csproj -c Release -r win-x64 --self-contained false -o "Release\AlwaysDown-FrameworkDependent"
if errorlevel 1 (
    echo ERROR: Framework-dependent publish failed
    pause
    exit /b 1
)

REM Copy documentation and additional files
echo [6/6] Copying documentation...
copy "README.md" "Release\"
copy "USAGE_EXAMPLE.md" "Release\"
copy "LICENSE" "Release\" 2>nul

REM Rename executables for clarity
if exist "Release\AlwaysDown-Standalone\WinForm.exe" (
    ren "Release\AlwaysDown-Standalone\WinForm.exe" "AlwaysDown.exe"
)
if exist "Release\AlwaysDown-FrameworkDependent\WinForm.exe" (
    ren "Release\AlwaysDown-FrameworkDependent\WinForm.exe" "AlwaysDown.exe"
)

echo.
echo ========================================
echo       Build completed successfully!
echo ========================================
echo.
echo Output directories:
echo   - Release\AlwaysDown-Standalone\        (Self-contained, ~100MB)
echo   - Release\AlwaysDown-FrameworkDependent\ (Requires .NET 8, ~1MB)
echo.
echo The self-contained version includes the .NET runtime and can run on any Windows machine.
echo The framework-dependent version requires .NET 8.0 Desktop Runtime to be installed.
echo.
pause 