#! /bin/sh

echo $0

$iotHubName = jq '.IoTHubsName.value' <<< $0

(az extension add --name azure-cli-iot-ext && az iot hub device-identity show --device-id $deviceId --hub-name $iotHubName) || (az iot hub device-identity create --hub-name $iotHubName --device-id $deviceId --edge-enabled && TMP_OUTPUT="$(az iot hub device-identity show-connection-string --device-id $deviceId --hub-name $iotHubName)" && RE="\"cs\":\s?\"(.*)\"" && if [[ $TMP_OUTPUT =~ $RE ]]; then CS_OUTPUT=${BASH_REMATCH[1]}; fi && echo "##vso[task.setvariable variable=CS_OUTPUT]${CS_OUTPUT}")