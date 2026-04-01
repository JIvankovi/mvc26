# Hook script to log user commands
# Reads JSON from stdin, extracts the user prompt, and logs it

# Read JSON from stdin
$jsonInput = [Console]::In.ReadToEnd()

try {
    # Parse the JSON input
    $hookData = $jsonInput | ConvertFrom-Json
    
    # Extract the user prompt (the actual command text)
    $userPrompt = if ($hookData.userPrompt) { $hookData.userPrompt } else { "N/A" }
    
    # Create timestamp
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    
    # Prepare log entry
    $logEntry = "[$timestamp] $userPrompt"
    
    # Ensure logs directory exists
    $logDir = "logs"
    if (-not (Test-Path $logDir)) {
        New-Item -Path $logDir -ItemType Directory -Force | Out-Null
    }
    
    # Append to log file
    $logFile = "logs/agent_log"
    Add-Content -Path $logFile -Value $logEntry -Encoding UTF8
    
    # Return success - allow the command to continue
    $output = @{
        continue = $true
    } | ConvertTo-Json -Compress
    
    Write-Output $output
    exit 0
    
} catch {
    # On error, log the error but don't block the command
    Write-Error "Logging failed: $_"
    
    # Return continue to not block user interaction
    $output = @{
        continue = $true
    } | ConvertTo-Json -Compress
    
    Write-Output $output
    exit 0
}
