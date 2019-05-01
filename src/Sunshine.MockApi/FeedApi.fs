module FeedApi

open System
open Giraffe
open Ids
open Newtonsoft.Json

type DataStreamData =
      { Value: float
        Timestamp: DateTimeOffset }

type PgridStream =
      { Data: DataStreamData array
        [<JsonProperty("decimal_precision")>]
        DecimalPrecision: int
        Title: string
        Units: string
        Class: string
        Priority: int }

type DataStream = { Pgrid: PgridStream }

type DeviceFeed =
      { FeedIntervals: string array
        Description: string
        Datastreams: DataStream option }

type Response = { Feeds: (string * DeviceFeed) list }

let getFeedData next ctx =
    let logger = { FeedIntervals = [|"raw"|]
                   Description = "http://power-one.com/device/aurora/universal_logger/v1/"
                   Datastreams = None }

    let pgrid =
        { Pgrid =
            { Data = [| { Value = 100.11; Timestamp = DateTimeOffset.Now } |]
              DecimalPrecision = 2
              Title = "Pgrid"
              Units = "W"
              Class = "out"
              Priority = 0 }
        }

    let inverter = { FeedIntervals = [|"raw"|]
                     Description = "http://power-one.com/device/inverter/v2/"
                     Datastreams = Some(pgrid)
    }

    json { Feeds = [(loggerId, logger); (inverterId, inverter)] } next ctx