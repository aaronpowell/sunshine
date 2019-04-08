// Learn more about F# at http://fsharp.org

open System
open InverterApi
open Specs
open LiveData
open Feeds
open Utils

let gev s = Environment.GetEnvironmentVariable s

let baseUrl = gev "SUNSHINE_URL"
let username = gev "SUNSHINE_USERNAME"
let password = gev "SUNSHINE_PASSWORD"

[<EntryPoint>]
let main _ =
   async {
            // do! System.Threading.Tasks.Task.Delay(5000) |> Async.AwaitTask
            let token = getAuthToken username password
            let getData' = getData token (Uri baseUrl)

            let! specs = getSpec getData'
            let deviceId = specs.Device.DeviceId |> toS

            match! getLiveList getData' deviceId with
            | Some liveList ->
                printfn "Found the live list"
            | None ->
                printfn "Well that shouldn't have happened..."

            match! getPgridFeed getData' DateTime.Today deviceId with
            | Some feed ->
                printfn "Found the data feed"
            | None ->
                printfn "Didn't find feed"

            match! getLiveData getData' deviceId with
            | Some liveData ->
                printfn "Found the live data"
            | None ->
                printfn "Didn't find live data"

            return 0
    } |> Async.RunSynchronously
