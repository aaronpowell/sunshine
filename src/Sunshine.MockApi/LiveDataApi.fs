module LiveDataApi
open System
open Ids
open Giraffe
open Newtonsoft.Json
open DataGen

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
        { Name = "CountryStd"; Value = 75. }
        { Name = "InputMode"; Value = 0. }
        { Name = "NumOfMPPT"; Value = 2. }
        { Name = "WRtg"; Value = 5050. }
        // Panel group 1
        { Name = "Iin1"; Value = rand 1. 2. }
        { Name = "Vin1"; Value = rand 0. 400. }
        { Name = "Pin1"; Value = rand 0. 500. }
        // Panel group 2
        { Name = "Iin2"; Value = rand 1. 2. }
        { Name = "Vin2"; Value = rand 0. 400. }
        { Name = "Pin2"; Value = rand 0. 500. }
        // Totals
        { Name = "Pin"; Value = rand 0. 5000. }
        // Outputs to grid
        { Name = "Igrid"; Value = rand 0. 4. }
        { Name = "Pgrid"; Value = rand 0. 500. }
        { Name = "Vgrid"; Value = rand 0. 250. }
        { Name = "Fgrid"; Value = rand 0. 100. }
        // Random useful ones
        { Name = "SysTime"; Value = DateTime.Now.Subtract(epoch).TotalSeconds }
    |]

    let inverter = Inverter(inverterId, now, inverterPoints)

    let data = [(loggerId, logger); (inverterId, inverter :> Device)] |> Map.ofList

    json data next ctx