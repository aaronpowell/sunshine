param (
    [Parameter(Mandatory=$true)]
    [string]
    $ResourceManagerOutput
    )

$json = ConvertFrom-Json $ResourceManagerOutput

Write-Host "##vso[task.setvariable variable=CONTAINER_REGISTRY_SERVER]$($json.acrUrl.value)"
Write-Host "##vso[task.setvariable variable=CONTAINER_REGISTRY_SERVER_NAME]$($json.acrName.value)"
Write-Host "##vso[task.setvariable variable=SUBSCRIPTION_ID]$($json.subscriptionID.value)"