@echo off
echo ========================================
echo   AlwaysDown - Installer Creator
echo ========================================
echo.

cd /d "%~dp0"

REM Check if NSIS is installed
where makensis >nul 2>&1
if errorlevel 1 (
    echo NSIS (Nullsoft Scriptable Install System) is not installed or not in PATH.
    echo.
    echo To create an installer, please:
    echo 1. Download NSIS from https://nsis.sourceforge.io/
    echo 2. Install NSIS
    echo 3. Add NSIS to your PATH environment variable
    echo 4. Run this script again
    echo.
    echo Alternative: Use the ZIP packages created by build-release.bat
    pause
    exit /b 1
)

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

echo Creating installer script...

REM Create NSIS installer script
(
echo !define APPNAME "AlwaysDown"
echo !define APPVERSION "1.1.0"
echo !define APPCOMPANY "AlwaysDown"
echo !define APPEXE "AlwaysDown.exe"
echo.
echo Name "${APPNAME}"
echo OutFile "Release\AlwaysDown-Setup-v${APPVERSION}.exe"
echo InstallDir "$PROGRAMFILES64\${APPNAME}"
echo RequestExecutionLevel admin
echo.
echo Page directory
echo Page instfiles
echo UninstPage uninstConfirm
echo UninstPage instfiles
echo.
echo Section "MainSection" SEC01
echo     SetOutPath "$INSTDIR"
echo     File /r "Release\AlwaysDown-Standalone\*"
echo     
echo     ; Create desktop shortcut
echo     CreateShortCut "$DESKTOP\${APPNAME}.lnk" "$INSTDIR\${APPEXE}"
echo     
echo     ; Create start menu shortcut
echo     CreateDirectory "$SMPROGRAMS\${APPNAME}"
echo     CreateShortCut "$SMPROGRAMS\${APPNAME}\${APPNAME}.lnk" "$INSTDIR\${APPEXE}"
echo     CreateShortCut "$SMPROGRAMS\${APPNAME}\Uninstall ${APPNAME}.lnk" "$INSTDIR\Uninstall.exe"
echo     
echo     ; Write uninstaller
echo     WriteUninstaller "$INSTDIR\Uninstall.exe"
echo     
echo     ; Add to Add/Remove Programs
echo     WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayName" "${APPNAME}"
echo     WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "UninstallString" "$INSTDIR\Uninstall.exe"
echo     WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayVersion" "${APPVERSION}"
echo     WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "Publisher" "${APPCOMPANY}"
echo     WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoModify" 1
echo     WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoRepair" 1
echo SectionEnd
echo.
echo Section "Uninstall"
echo     Delete "$INSTDIR\*.*"
echo     RMDir /r "$INSTDIR"
echo     Delete "$DESKTOP\${APPNAME}.lnk"
echo     RMDir /r "$SMPROGRAMS\${APPNAME}"
echo     DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
echo SectionEnd
) > installer.nsi

echo Compiling installer...
makensis installer.nsi

if exist "Release\AlwaysDown-Setup-v1.1.0.exe" (
    echo.
    echo ========================================
    echo     Installer created successfully!
    echo ========================================
    echo.
    echo Output: Release\AlwaysDown-Setup-v1.1.0.exe
    echo.
    del installer.nsi
) else (
    echo ERROR: Failed to create installer
    pause
    exit /b 1
)

pause 