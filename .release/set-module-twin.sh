#! /bin/sh

az extension add --name azure-cli-iot-ex

IoTHubName=$(cat $SYSTEM_DEFAULTWORKINGDIRECTORY/_aaronpowell.sunshine/arm/release-output.json | jq '.iotHubName.value' | sed s/\"//g)

az iot hub module-twin update \ 
--device-id $DEVICE_ID \ 
--hub-name $IoTHubName \ 
--module-id SunshineDownloader \ 
--set properties.desired='{ "inverter": { "username": "$SunshineUser", "password": "$SunshinePwd", "url": "$SunshineUrl" } }'