#! /bin/sh

IoTHubName=$(cat $SYSTEM_DEFAULTWORKINGDIRECTORY/_aaronpowell.sunshine/arm/release-output.json | jq '.iotHubName.value' | sed s/\"// | sed s/\"//)

# install iot hub extension
az extension add --name azure-cli-iot-ext

# check if device exists
# note: redirect to /dev/null so the keys aren't printed to logs, and we don't need the output anyway
az iot hub device-identity show \
--device-id $DEVICE_ID \
--hub-name $IoTHubName >> /dev/null

if [ $? -ne 0 ]; then
    az iot hub device-identity create --hub-name $IoTHubName --device-id $DEVICE_ID --edge-enabled
    TMP_OUTPUT="$(az iot hub device-identity show-connection-string --device-id $DEVICE_ID --hub-name $IoTHubName)"
    RE="\"cs\":\s?\"(.*)\""
    if [[ $TMP_OUTPUT =~ $RE ]]; then
        CS_OUTPUT=${BASH_REMATCH[1]};
    fi
    echo "##vso[task.setvariable variable=CS_OUTPUT]${CS_OUTPUT}"
fi

# (az extension add --name azure-cli-iot-ext && az iot hub device-identity show --device-id $DEVICE_ID --hub-name $IoTHubName) || (az iot hub device-identity create --hub-name $IoTHubName --device-id $DEVICE_ID --edge-enabled && TMP_OUTPUT="$(az iot hub device-identity show-connection-string --device-id $DEVICE_ID --hub-name $IoTHubName)" && RE="\"cs\":\s?\"(.*)\"" && if [[ $TMP_OUTPUT =~ $RE ]]; then CS_OUTPUT=${BASH_REMATCH[1]}; fi && echo "##vso[task.setvariable variable=CS_OUTPUT]${CS_OUTPUT}")