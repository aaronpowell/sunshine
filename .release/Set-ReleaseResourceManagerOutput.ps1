param (
    [Parameter(Mandatory=$true)]
    [string]
    $ResourceManagerOutput
    )

Set-Content -Path $(env:SYSTEM_DEFAULTWORKINGDIRECTORY)/.arm/release-output.json -Value $ResourceManagerOutput