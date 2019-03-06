module Payload

open FSharp.Data
open System.IO
open System.Text

type Payload = JsonProvider<"./sample-data/payload.json">
type DevicePayload = JsonProvider<"./sample-data/device.json">

let parsePayload (stream : Stream) =
    async {
        use reader = new StreamReader(stream, Encoding.UTF8)
        let! body = reader.ReadToEndAsync() |> Async.AwaitTask
        return Payload.Parse body
    }

let getDevicePayload deviceId (payload : Payload.Root) =
    let deviceRawData = payload.JsonValue.GetProperty deviceId
    deviceRawData.ToString() |> DevicePayload.Parse

