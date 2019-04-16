module FeedData

open FSharp.Data

type PgridFeed = JsonProvider<"../Shared/sample-data/pgrid-sum.json">
