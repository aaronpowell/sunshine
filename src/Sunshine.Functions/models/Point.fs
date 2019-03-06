module Point

open System
open FSharp.Azure.Storage.Table
open Payload

type Point =
     { [<PartitionKey>] DeviceId : string
       [<RowKey>] RowKey : string
       Name : string
       Value : decimal }

let payloadToPoint deviceRecordId (source : DevicePayload.Point) =
      { DeviceId = deviceRecordId
        RowKey = Guid.NewGuid().ToString()
        Name = source.Name
        Value = source.Value }