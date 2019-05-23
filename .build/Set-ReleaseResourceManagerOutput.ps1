param (
    [Parameter(Mandatory=$true)]
    [string]
    $ResourceManagerOutput
    )

Set-Content -Path $env:BUILD_ARTIFACTSTAGINGDIRECTORY/release-output.json -Value $ResourceManagerOutput