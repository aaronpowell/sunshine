open System
open InverterApi
open Specs
open LiveDataDownloader
open FeedDownloader
open Utils
open Microsoft.Azure.Devices.Client

let gev s = Environment.GetEnvironmentVariable s

let baseUrl = gev "SUNSHINE_URL"
let username = gev "SUNSHINE_USERNAME"
let password = gev "SUNSHINE_PASSWORD"
let iotHubConnectionString = gev "IOT_CONNSTR"

[<EntryPoint>]
let main _ =
    let iotHubClient = DeviceClient.CreateFromConnectionString(iotHubConnectionString, TransportType.Mqtt)

    async {
            let token = getAuthToken username password
            let getData' = getData token (Uri baseUrl)

            let! specs = getSpec getData'
            let deviceId = specs.Device.DeviceId |> toS

            printfn "Up and running, we found an inverter with the ID %s" deviceId

            match! getLiveList getData' deviceId with
            | Some liveList ->
                do! sendIoTMessage iotHubClient "liveList" liveList

                let rec dataPoller() = async {
                    match! getLiveData getData' deviceId with
                    | Some liveData ->
                        do! sendIoTMessage iotHubClient "liveData" liveData

                    | None ->
                        printfn "Didn't find live data"

                    do! Async.Sleep 20000
                    dataPoller() |> Async.Start
                }

                dataPoller() |> Async.Start

            | None ->
                printfn "Well that shouldn't have happened..."

            let rec feedPoller() = async {
                match! getPgridFeed getData' DateTime.Today deviceId with
                | Some feed ->
                    do! sendIoTMessage iotHubClient "feed" feed
                | None ->
                    printfn "Didn't find feed"

                do! Async.Sleep 300000
                feedPoller() |> Async.Start
            }

            feedPoller() |> Async.Start

            printfn "Background jobs running, now we're waiting... "
            if Console.IsInputRedirected then
                while true do
                    do! Async.Sleep 300000
            else
                Console.ReadLine() |> ignore

            return 0
    } |> Async.RunSynchronously
