module IoTHubTrigger

open Microsoft.Extensions.Logging
open Microsoft.Azure.WebJobs

open LiveData

[<FunctionName("LiveDataTrigger")>]
let trigger
    ([<EventHubTrigger("live-data", Connection = "IoTHubConnectionString")>] message: string)
    (logger: ILogger) =
        let l = LiveData.Parse message
        logger.LogInformation(sprintf "Live Data: %A" l)