module LiveData

open FSharp.Data

type LiveList = JsonProvider<"./sample-data/live-data-list.json">

let liveUrlPath = "v1/livedata"
let listUrlPath = liveUrlPath + "/list"

let getLiveList getData =
    async {
        let! data = getData listUrlPath

        return LiveList.Parse data
    }