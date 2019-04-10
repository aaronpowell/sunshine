module LiveListApi
open Ids
open Giraffe
open Newtonsoft.Json

type Point =
      { Name: string
        Unit: string
        Description: string
        Type: string
        Kind: string
        [<JsonProperty("decimal_precision")>]
        DecimalPrecision: int }

type Device(deviceId: string, ``type``: string, points: Point array) =
      [<JsonProperty("device_id")>]
      member __.DeviceId = deviceId
      member __.Type = ``type``
      member __.Points = points

type Inverter(deviceId, points, model: string) =
    inherit Device(deviceId, "http://power-one.com/device/inverter/v2/", points)

    [<JsonProperty("device_type")>]
    member __.DeviceType = "inverter_1phase"
    [<JsonProperty("device_model")>]
    member __.DeviceModel = model

type LiveList =
      { [<JsonProperty("Devices")>]Devices: Device array }

let getLiveListData next ctx =
    let logger = Device(loggerId, "http://power-one.com/device/inverter/v1/", Array.empty)

    let inverterPoints = [|
        { Name = "CountryStd"
          Unit = ""
          Description = "Country Std"
          Type = "out"
          Kind = ""
          DecimalPrecision = 2 }
    |]

    let inverter = Inverter(inverterId, inverterPoints, "")

    json { Devices = [| logger; inverter |] } next ctx