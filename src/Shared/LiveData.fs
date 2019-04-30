module LiveData

open FSharp.Data
open System

type LiveList = JsonProvider<"../Shared/sample-data/live-data-list.json">
type LiveData = JsonProvider<"../Shared/sample-data/live-data.json">
type LiveDataDevice = JsonProvider<"../Shared/sample-data/live-data-device.json">

let epoch = DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)

let findPoint (points: LiveDataDevice.Point[]) name =
       let point = points |> Array.find(fun p -> p.Name = name)
       float point.Value
