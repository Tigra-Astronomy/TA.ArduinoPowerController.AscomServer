$driver = New-Object -ComObject ASCOM.ArduinoPowerController.Switch
try 
{
    $driver.Connected = $true
    while ($driver.Connected) 
    {
        for ($i = 0; $i -lt 8; $i++) 
        {
            $driver.SetSwitch($i, $true)
            Start-Sleep -Milliseconds 100
            $driver.SetSwitch($i, $false)
            Start-Sleep -Milliseconds 100
        }
    }
}
finally
{
    $driver.Dispose()
}