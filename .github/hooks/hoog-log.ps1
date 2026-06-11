param($payload)

if ($payload) {
    Add-Content -Path 'C:\Users\CinoP\OneDrive\Desktop\VSCode\ASPNET\mvc26\mvc26\logs\agent_log.txt' -Value $payload
}
elseif ($input) {
    $input | Out-String | Add-Content -Path 'C:\Users\CinoP\OneDrive\Desktop\VSCode\ASPNET\mvc26\mvc26\logs\agent_log.txt'
}