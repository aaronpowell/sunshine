module LiveDataDownloader

open LiveData
open FSharp.Data
open Utils

let liveUrlPath = "v1/livedata"
let listUrlPath = liveUrlPath + "/list"

let getLiveList getData deviceId =
    async {
      try
        let! data = getData listUrlPath

        let parsedData = (LiveList.Parse data)

        return parsedData.Devices
               |> Array.filter (fun d -> d.DeviceId |> toS = deviceId)
               |> Array.tryExactlyOne
      with
      | ex ->
        errorLogger "Failure getting live list" ex
        return None }

let getLiveData getData (deviceId : string) =
    async {
        try
          let! data = getData liveUrlPath

          let liveData = LiveData.Parse data

          return liveData.JsonValue.Properties()
                 |> Array.filter (fun (p, _) -> p = deviceId.Replace("\"", ""))
                 |> Array.map (fun (_, v) -> LiveData.DeviceId v)
                 |> Array.tryExactlyOne
        with
        | ex ->
          errorLogger "Failure getting live data" ex
          return None }