# Simple test - directly append to log
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$logEntry = "[$timestamp] Direct test message"

# Ensure logs directory exists
if (-not (Test-Path "logs")) {
    New-Item -Path "logs" -ItemType Directory -Force | Out-Null
}

# Append to log file
Add-Content -Path "logs\agent_log.txt" -Value $logEntry -Encoding UTF8

Write-Host "Test completed. Check logs\agent_log.txt"
Get-Content "logs\agent_log.txt"
