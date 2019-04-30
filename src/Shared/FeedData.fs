module FeedData

open FSharp.Data

type PgridFeed = JsonProvider<"../Shared/sample-data/pgrid-sum.json">
type PgridFeedDevice = JsonProvider<"../Shared/sample-data/pgrid-sum-device.json">
