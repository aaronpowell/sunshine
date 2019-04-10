module IoTHubTrigger

open Microsoft.Extensions.Logging
open Microsoft.Azure.WebJobs

[<FunctionName("IoTHubTrigger")>]
let trigger
    ([<EventHubTrigger("iothub-ehub-sunshine-d-1491440-c129cbe415", ConsumerGroup = "functions", Connection = "IoTHubConnectionString")>] message: string)
    (logger: ILogger) =
        logger.LogInformation("There was a message " + message)