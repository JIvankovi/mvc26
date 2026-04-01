#!/usr/bin/env pwsh
# Read JSON from stdin
$inputJson = [Console]::In.ReadToEnd()
$data = $inputJson | ConvertFrom-Json

# Extract the prompt
$prompt = $data.prompt

# Create timestamp
$timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'

# Create log entry
$logEntry = "[$timestamp] $prompt"

# Ensure logs directory exists
if (-not (Test-Path "logs")) {
    New-Item -Path "logs" -ItemType Directory -Force | Out-Null
}

# Append to log file
Add-Content -Path "logs/agent_log.txt" -Value $logEntry -Encoding UTF8

# Exit successfully
exit 0
