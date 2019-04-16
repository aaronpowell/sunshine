module LiveData

open FSharp.Data

type LiveList = JsonProvider<"../Shared/sample-data/live-data-list.json">
type LiveData = JsonProvider<"../Shared/sample-data/live-data.json">
