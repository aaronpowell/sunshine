module Device

open FSharp.Azure.Storage.Table
open System
open Payload

type Device =
     {[<PartitionKey>] DeviceId : string
      [<RowKey>] RowKey : string 
      DeviceType : string
      DeviceModel : string
      Timestamp : DateTimeOffset}

let payloadToDevice (deviceData : DevicePayload.Root) =
      { DeviceId = deviceData.DeviceId
        RowKey = Guid.NewGuid().ToString()
        DeviceType = deviceData.DeviceType
        DeviceModel = deviceData.DeviceModel
        Timestamp = deviceData.Timestamp }
