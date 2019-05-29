# Sunshine - My Home Grown IoT Project

![Logo](noun_solar_72144.svg)

<cite>solar by iconsmind.com from the Noun Project</cite>

Sunshine is a project I started to learn how someone would go about building an IoT project. My use case was to connect to my solar inverter and export the data to store where I could produce my own reporting against it.

## How it works

The project is made up of 3 components:

* Downloader
  * .NET Core application written in F#
  * Deployed as a Docker image
  * Pulls the data from the inverter and pushes it to [Azure IoT Hub](https://azure.microsoft.com/en-us/services/iot-hub/?WT.mc_id=sunshine-github-aapowell)
* Functions
  * [Azure Functions](https://azure.microsoft.com/en-us/services/functions/?WT.mc_id=sunshine-github-aapowell) for processing the data, written in F# against the v2 runtime
  * Listens to a set of [Event Hubs](https://azure.microsoft.com/en-au/services/event-hubs/?WT.mc_id=sunshine-github-aapowell) for incoming messages
  * Writes to [Table Storage](https://azure.microsoft.com/en-au/services/storage/tables/?WT.mc_id=sunshine-github-aapowell)
* Mock API
  * A fake implementation of my inverter API so I can dev when I'm not at home
  * Implemented using Saturn, a F# web framework

The Downloader runs in a Docker container and will either talk to the Mock API (local dev) or the inverter (production) and push the data up to [Azure IoT Hub](https://azure.microsoft.com/en-us/services/iot-hub/?WT.mc_id=sunshine-github-aapowell). IoT Hub will then route the data to one of several [Event Hubs](https://azure.microsoft.com/en-au/services/event-hubs/?WT.mc_id=sunshine-github-aapowell) which the Functions are subscribed to for processing and storage in [Table Storage](https://azure.microsoft.com/en-au/services/storage/tables/?WT.mc_id=sunshine-github-aapowell).

Deployments are orchestrated using [Azure Pipelines](https://azure.microsoft.com/en-us/services/devops/pipelines/?WT.mc_id=sunshine-github-aapowell) to build the images, push to [Azure Container Registry](https://azure.microsoft.com/en-us/services/container-registry/?WT.mc_id=sunshine-github-aapowell) and then use [Azure IoT Edge](https://azure.microsoft.com/en-us/services/iot-edge/?WT.mc_id=sunshine-github-aapowell) to deploy onto either my test device or my production device.

You'll find more information on the process on my blog through the series [Home Grown IoT](https://www.aaron-powell.com/posts/2019-05-29-home-grown-iot-prologue).