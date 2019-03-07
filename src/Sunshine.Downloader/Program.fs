// Learn more about F# at http://fsharp.org

open System
open InverterApi
open Specs
open LiveData

let gev s = Environment.GetEnvironmentVariable s

let baseUrl = gev "SUNSHINE_URL"
let username = gev "SUNSHINE_USERNAME"
let password = gev "SUNSHINE_PASSWORD"

let toS o = o.ToString()

[<EntryPoint>]
let main _ =
    let token = getAuthToken username password
    let getData' = getData token (Uri baseUrl)

    let specs = (getSpec getData') |> Async.RunSynchronously
    let deviceId = specs.Device.DeviceId |> toS

    let liveListData = (getLiveList getData') |> Async.RunSynchronously

    let deviceLive = liveListData.Devices
                     |> Array.filter (fun d -> d.DeviceId |> toS = deviceId)

    match deviceLive |> Array.tryExactlyOne with
    | Some device ->
        printfn "Found it!"
    | None ->
        printfn "Well that shouldn't have happened..."

    0 // return an integer exit code
