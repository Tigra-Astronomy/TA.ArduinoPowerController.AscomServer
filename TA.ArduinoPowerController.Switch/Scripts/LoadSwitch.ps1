Push-Location ..\bin\Debug
Add-Type -path .\ASCOM.K8056.Switch.dll
$global:sw=New-Object ASCOM.K8056.Switch
$global:sw.Connected = $true
Pop-Location
