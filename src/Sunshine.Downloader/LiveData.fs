module LiveData

open FSharp.Data
open Utils

type LiveList = JsonProvider<"./sample-data/live-data-list.json">
type LiveData = JsonProvider<"./sample-data/live-data.json">

let liveUrlPath = "v1/livedata"
let listUrlPath = liveUrlPath + "/list"

let getLiveList getData deviceId =
    async {
        let! data = getData listUrlPath

        let parsedData = (LiveList.Parse data)

        printfn "Raw: %s" data
        printfn "Obj: %A" parsedData
        printfn "All: %A" parsedData.Devices
        printfn "First: %A" parsedData.Devices.[0]
        printfn "Second: %A" parsedData.Devices.[1]

        return parsedData.Devices
               |> Array.map(fun d ->
                        printfn "Device: %A" d
                        d)
               |> Array.filter (fun d -> d.DeviceId |> toS = deviceId)
               |> Array.tryExactlyOne
    }

let getLiveData getData (deviceId : string) =
    async {
        let! data = getData liveUrlPath

        let liveData = LiveData.Parse data

        return liveData.JsonValue.Properties()
               |> Array.filter (fun (p, _) -> p = deviceId.Replace("\"", ""))
               |> Array.map (fun (_, v) -> LiveData.DeviceId v)
               |> Array.tryExactlyOne
    }