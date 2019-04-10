module LiveDataApi
open System
open Ids
open Giraffe
open Newtonsoft.Json

type Point =
      { Name: string
        Value: float }

type Device(deviceId: string, timestamp: DateTimeOffset, points: Point array) =
    [<JsonProperty("device_id")>]
    member __.DeviceId = deviceId
    member __.Timestamp = timestamp
    member __.Points = points

type Inverter(deviceId, timestamp, points) =
    inherit Device(deviceId, timestamp, points)

    [<JsonProperty("device_type")>]
    member __.DeviceType = "inverter_1phase"
    [<JsonProperty("device_model")>]
    member __.DeviceModel = "MOCK-DEVICE"

let getLiveData next ctx =
    let now = DateTimeOffset.Now
    let logger = Device(loggerId, now, Array.empty)

    let inverterPoints = [|
        { Name = "CountryStd"; Value = 75.0 }
    |]

    let inverter = Inverter(inverterId, now, inverterPoints)

    let data = [(loggerId, logger); (inverterId, inverter :> Device)] |> Map.ofList

    json data next ctx