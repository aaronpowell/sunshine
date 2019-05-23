#! /bin/sh

IoTHubName=$(cat ./.arm/release-output.json | jq '.iotHubName.value')

(az extension add --name azure-cli-iot-ext && az iot hub device-identity show --device-id $DEVICE_ID --hub-name $IoTHubName) || (az iot hub device-identity create --hub-name $IoTHubName --device-id $DEVICE_ID --edge-enabled && TMP_OUTPUT="$(az iot hub device-identity show-connection-string --device-id $DEVICE_ID --hub-name $IoTHubName)" && RE="\"cs\":\s?\"(.*)\"" && if [[ $TMP_OUTPUT =~ $RE ]]; then CS_OUTPUT=${BASH_REMATCH[1]}; fi && echo "##vso[task.setvariable variable=CS_OUTPUT]${CS_OUTPUT}")