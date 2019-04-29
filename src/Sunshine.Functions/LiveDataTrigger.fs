module IoTHubTrigger

open Microsoft.Extensions.Logging
open Microsoft.Azure.WebJobs

open LiveData
open Microsoft.WindowsAzure.Storage.Table
open FSharp.Azure.Storage.Table
open AzureTableUtils
open System

type LiveDataEntity =
     { [<PartitionKey>] DeviceId: string
       [<RowKey>] MessageId: string }

[<FunctionName("LiveDataTrigger")>]
let trigger
    ([<EventHubTrigger("live-data", Connection = "IoTHubConnectionString")>] message: string)
    ([<Table("LiveData", Connection = "DataStorageConnectionString")>] liveDataTable: CloudTable)
    (logger: ILogger) =
        let l = LiveDataDevice.Parse message

        let id = l.DeviceId.ToString()

        let e = { DeviceId = id; MessageId = Guid.NewGuid().ToString() }

        let result = e |> Insert |> inTableToClientAsync liveDataTable |> Async.RunSynchronously

        logger.LogInformation(sprintf "Result: %A" result)
        logger.LogInformation(sprintf "Live Data: %A" l)
        logger.LogInformation(sprintf "Id: %s" id)