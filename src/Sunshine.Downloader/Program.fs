open System
open InverterApi
open Specs
open LiveDataDownloader
open FeedDownloader
open Utils
open IoTWrapper

let gev s = Environment.GetEnvironmentVariable s

let baseUrl = gev "SUNSHINE_URL"
let username = gev "SUNSHINE_USERNAME"
let password = gev "SUNSHINE_PASSWORD"
let iotConnStr = gev "IOT_CONNSTR"

[<EntryPoint>]
let main _ =
    async {
    let! iotClient = getIotHubClient iotConnStr

    let token = getAuthToken username password
    let getData' = getData token (Uri baseUrl)

    let! specs = getSpec getData'
    let deviceId = specs.Device.DeviceId |> toS

    printfn "Up and running, we found an inverter with the ID %s" deviceId

    let mutable correlationId = Guid.NewGuid()

    match! getLiveList getData' deviceId with
    | Some liveList ->
        let rec listPoller() = async {
            do! int(TimeSpan.FromMinutes(5.).TotalMilliseconds) |> Async.Sleep
            correlationId <- Guid.NewGuid()

            match! getLiveList getData' deviceId with
            | Some liveList -> do! sendIoTMessage iotClient "liveList" correlationId liveList
            | None -> ignore()

            listPoller() |> Async.Start }
        do! sendIoTMessage iotClient "liveList" correlationId liveList
        listPoller() |> Async.Start

        let rec dataPoller() = async {
            match! getLiveData getData' deviceId with
            | Some liveData -> do! sendIoTMessage iotClient "liveData" correlationId liveData
            | None -> ignore()

            do! int(TimeSpan.FromSeconds(20.).TotalMilliseconds) |> Async.Sleep
            dataPoller() |> Async.Start }

        dataPoller() |> Async.Start

    | None ->
        printfn "Well that shouldn't have happened..."

    let rec feedPoller() = async {
        match! getPgridFeed getData' DateTime.Today deviceId with
        | Some feed -> do! sendIoTMessage iotClient "feed" correlationId feed
        | None -> ignore()

        do! int(TimeSpan.FromMinutes(5.).TotalMilliseconds) |> Async.Sleep
        feedPoller() |> Async.Start }

    feedPoller() |> Async.Start

    printfn "Background jobs running, now we're waiting... "
    if Console.IsInputRedirected then
        while true do
            do! Async.Sleep 300000
    else
        Console.ReadLine() |> ignore

    return 0 } |> Async.RunSynchronously
