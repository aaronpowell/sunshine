module LiveListTrigger

open Microsoft.Extensions.Logging
open Microsoft.Azure.WebJobs

open LiveData

[<FunctionName("LiveListTrigger")>]
let trigger
    ([<EventHubTrigger("live-list", Connection = "IoTHubConnectionString")>] message: string)
    (logger: ILogger) =
        let ll = LiveList.Parse message
        logger.LogInformation(sprintf "Live List: %A" ll)