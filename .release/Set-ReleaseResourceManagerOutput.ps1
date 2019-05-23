param (
    [Parameter(Mandatory=$true)]
    [string]
    $ResourceManagerOutput
    )

New-Item -Path $env:SYSTEM_DEFAULTWORKINGDIRECTORY/.arm -ItemType Directory

Set-Content -Path $env:SYSTEM_DEFAULTWORKINGDIRECTORY/.arm/release-output.json -Value $ResourceManagerOutput