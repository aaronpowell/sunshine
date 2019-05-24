param (
    [Parameter(Mandatory=$true)]
    [string]
    $ResourceManagerOutput
    )

$json = ConvertFrom-Json $ResourceManagerOutput

Write-Host "##vso[task.setvariable variable=ARTIFACT_STORAGE_NAME;isOutput=true]$($json.artifactStorageName.value)"

Write-Host $json.artifactStorageName.value