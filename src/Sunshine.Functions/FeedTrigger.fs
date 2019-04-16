module FeedTrigger

open Microsoft.Extensions.Logging
open Microsoft.Azure.WebJobs

open FeedData

[<FunctionName("FeedTrigger")>]
let trigger
    ([<EventHubTrigger("feed", Connection = "IoTHubConnectionString")>] message: string)
    (logger: ILogger) =
        let feed = PgridFeed.Parse message
        logger.LogInformation(sprintf "Found a feed: %A" feed)