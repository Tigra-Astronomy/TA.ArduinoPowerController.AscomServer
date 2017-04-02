Push-Location ..\bin\Debug
Add-Type -path .\ASCOM.ArduinoPowerController.Switch.dll
$global:sw=New-Object ASCOM.ArduinoPowerController.Switch
$global:sw.Connected = $true
Pop-Location
