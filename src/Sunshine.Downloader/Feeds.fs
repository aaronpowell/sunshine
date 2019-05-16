module FeedDownloader

open FeedData
open FSharp.Data
open System

let feedUrl = "/v1/feeds/"

let pgridFeedUrl (date : DateTime) =
    let day = date.ToString("yyyy-MM-dd")
    let query = sprintf "?client=query1D&end=%sT23:59:59&feedList%%5B%%5D=Pgrid&maxDataPointsPerPage=1440&start=%sT00:00:00" day day
    feedUrl + query

let getPgridFeed getData date (deviceId : string) =
    async {
        try
           let! data = pgridFeedUrl date
                       |> getData
           let parsed = PgridFeed.Parse data

           return parsed.Feeds.JsonValue.Properties()
                  |> Array.filter (fun (p, _) -> p.EndsWith(deviceId.Replace("\"", "")))
                  |> Array.map (fun (_, v) -> PgridFeed.DeviceId v)
                  |> Array.tryExactlyOne
        with
        | ex ->
            printfn "Failure getting feed"
            printfn "%A" <| ex
            return None
    }