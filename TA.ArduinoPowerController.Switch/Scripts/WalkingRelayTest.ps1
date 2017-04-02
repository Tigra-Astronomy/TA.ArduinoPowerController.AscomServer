
$sw = New-Object -ComObject  ASCOM.ArduinoPowerController.Switch
$sw.Connected = $true

Function WalkState([bool] $state)
    {
    For ($i=0; $i -lt 8; $i++)
        {
        $sw.SetSwitch($i, $state)
        Start-Sleep -Milliseconds 250
        }
    }

While ($true)
	{
	WalkState($true)
	WalkState($false)
	}

