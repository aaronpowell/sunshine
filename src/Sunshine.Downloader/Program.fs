// Learn more about F# at http://fsharp.org

open System
open InverterApi
open Specs
open LiveData
open Feeds

let gev s = Environment.GetEnvironmentVariable s

let baseUrl = gev "SUNSHINE_URL"
let username = gev "SUNSHINE_USERNAME"
let password = gev "SUNSHINE_PASSWORD"

let toS o = o.ToString()

[<EntryPoint>]
let main _ =
   async {
            let token = getAuthToken username password
            let getData' = getData token (Uri baseUrl)

            let! specs = getSpec getData'
            let deviceId = specs.Device.DeviceId |> toS

            let! liveListData = getLiveList getData'

            let deviceLive = liveListData.Devices
                             |> Array.filter (fun d -> d.DeviceId |> toS = deviceId)

            match deviceLive |> Array.tryExactlyOne with
            | Some device ->
                let! pgridFeed = getPgridFeed getData' DateTime.Today deviceId

                match pgridFeed with
                | Some feed ->
                    printfn "Found the data feed"
                | None ->
                    printfn "Didn't find feed"
            | None ->
                printfn "Well that shouldn't have happened..."

            return 0
    } |> Async.RunSynchronously
