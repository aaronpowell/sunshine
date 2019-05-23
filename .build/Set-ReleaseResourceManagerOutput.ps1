param (
    [Parameter(Mandatory=$true)]
    [string]
    $ResourceManagerOutput
    )

$rmJson = ConvertFrom-Json $ResourceManagerOutput

Write-Host "##vso[task.setvariable variable=HubConnectionString;isOutput=true]$($rmJson.HubConnectionString.value)"
Write-Host "##vso[task.setvariable variable=FunctionAppName;isOutput=true]$($rmJson.functionAppName.value)"
Write-Host "##vso[task.setvariable variable=IoTHubName;isOutput=true]$($rmJson.IoTHubsName.value)"

$deploymentJson = Get-Content $(System.DefaultWorkingDirectory)/_aaronpowell.sunshine/drop/deployment.arm32v7.json | ConvertFrom-Json

Write-Host "##vso[task.setvariable variable=DEVOPS_IOTEDGE_REGISTRY_URL;isOutput=true]$($deploymentJson.modulesContent.'$edgeAgent'.'properties.desired'.runtime.settings.registryCredentials.YourACR.address.value)"