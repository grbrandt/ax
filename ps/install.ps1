# Define variables
$downloadUrl = "https://github.com/grbrandt/ax/releases/download/v1.0.0/ax-v1.0.0-x64.zip"
$destinationPath = "$env:LOCALAPPDATA\ax"
$zipPath = "$destinationPath\ax.zip"
$exePath = "$destinationPath\ax.exe"

# Create destination directory if it doesn't exist
if (!(Test-Path -Path $destinationPath)) {
    New-Item -ItemType Directory -Path $destinationPath -Force | Out-Null
}

# Download the zip file
Write-Output "Downloading ax..."
Invoke-WebRequest -Uri $downloadUrl -OutFile $zipPath

# Extract the zip file
Write-Output "Extracting ax..."
Expand-Archive -Path $zipPath -DestinationPath $destinationPath -Force

# Clean up the zip file
Remove-Item -Path $zipPath -Force

# Find ax.exe inside extracted folder
$extractedFiles = Get-ChildItem -Path $destinationPath -Recurse -Filter "ax.exe"
if ($extractedFiles.Count -gt 0) {
    $exePath = $extractedFiles[0].FullName
}

# Ensure ax.exe exists
if (!(Test-Path -Path $exePath)) {
    Write-Output "Error: ax.exe not found after extraction."
    exit 1
}

# Add alias for this session
Write-Output "Creating alias 'ax'..."
Set-Alias -Name ax -Value $exePath -Scope Global

# Persist alias by adding to PowerShell profile
$profilePath = "$PROFILE"
if (!(Test-Path $profilePath)) {
    New-Item -ItemType File -Path $profilePath -Force | Out-Null
}
$aliasCommand = "`nSet-Alias -Name ax -Value '$exePath' -Scope Global"
if (!(Select-String -Path $profilePath -Pattern "Set-Alias -Name ax" -Quiet)) {
    Add-Content -Path $profilePath -Value $aliasCommand
    Write-Output "Alias 'ax' added to profile for persistence."
}

Write-Output "Installation complete. You can now use 'ax' in PowerShell."
