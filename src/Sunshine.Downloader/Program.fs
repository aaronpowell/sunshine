open System
open InverterApi
open Specs
open LiveDataDownloader
open FeedDownloader
open Utils
open IoTWrapper
open Logo

let gev s = Environment.GetEnvironmentVariable s

let iotConnStr = gev "IOT_CONNSTR"

[<EntryPoint>]
let main _ =
    printLogo()
    infoLogger <| sprintf "Welcome to Sunshine"
    async {
    let! iotClient = getIoTHubClient iotConnStr

    let! twin = iotClient.GetTwinAsync()

    let iotProperties = parseDesiredProperties twin.Properties.Desired

    let token = getAuthToken iotProperties.Inverter.Username iotProperties.Inverter.Password
    let getData' = getData token (Uri iotProperties.Inverter.Url)

    let! specs = getSpec getData'
    let deviceId = specs.Device.DeviceId |> toS

    infoLogger <| sprintf "Up and running, we found an inverter with the ID %s" deviceId

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
        infoLogger <| sprintf "Well that shouldn't have happened..."

    let rec feedPoller() = async {
        match! getPgridFeed getData' DateTime.Today deviceId with
        | Some feed -> do! sendIoTMessage iotClient "feed" correlationId feed
        | None -> ignore()

        do! int(TimeSpan.FromMinutes(5.).TotalMilliseconds) |> Async.Sleep
        feedPoller() |> Async.Start }

    feedPoller() |> Async.Start

    infoLogger <| sprintf "Background jobs running, now we're waiting... "
    if Console.IsInputRedirected then
        while true do
            do! Async.Sleep 300000
    else
        Console.ReadLine() |> ignore

    return 0 } |> Async.RunSynchronously
