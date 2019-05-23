param (
    [Parameter(Mandatory=$true)]
    [string]
    $ResourceManagerOutput
    )

$json = ConvertFrom-Json $ResourceManagerOutput

Write-Host "##vso[task.setvariable variable=CONTAINER_REGISTRY_SERVER;isOutput=true]$($json.acrUrl.value)"
Write-Host "##vso[task.setvariable variable=CONTAINER_REGISTRY_SERVER_NAME;isOutput=true]$($json.acrName.value)"
Write-Host "##vso[task.setvariable variable=SUBSCRIPTION_ID;isOutput=true]$($json.subscriptionID.value)"