param (
    [Parameter(Mandatory=$true)]
    [string]
    $ResourceManagerOutput,

    [Parameter(Mandatory=$true)]
    [string]
    $DeploymentTemplatePath
    )

$rmJson = ConvertFrom-Json $ResourceManagerOutput

Write-Host "##vso[task.setvariable variable=FunctionAppName;isOutput=true]$($rmJson.functionAppName.value)"
Write-Host "##vso[task.setvariable variable=IoTHubName;isOutput=true]$($rmJson.IoTHubName.value)"

$deploymentJson = Get-Content $DeploymentTemplatePath | ConvertFrom-Json
Write-Host "##vso[task.setvariable variable=DEVOPS_IOTEDGE_REGISTRY_URL;isOutput=true]$($deploymentJson.modulesContent.'$edgeAgent'.'properties.desired'.runtime.settings.registryCredentials.YourACR.address.value)"