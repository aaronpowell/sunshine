module IoTHubTrigger

open Microsoft.Extensions.Logging
open Microsoft.Azure.WebJobs

open LiveData
open Microsoft.WindowsAzure.Storage.Table

type LiveDataEntity(partitionKey, rowKey) =
        inherit TableEntity(partitionKey, rowKey)

[<FunctionName("LiveDataTrigger")>]
let trigger
    ([<EventHubTrigger("live-data", Connection = "IoTHubConnectionString")>] message: string)
    ([<Table("LiveData", Connection = "DataStorageConnectionString")>] liveDataTable: CloudTable)
    (logger: ILogger) =
        let l = LiveData.Parse message

        let id = l.DeviceId.DeviceId.ToString()

        // let op = TableOperation.Insert(LiveDataEntity("", ""))

        // liveDataTable.ExecuteAsync op |> Async.AwaitTask |> Async.RunSynchronously |> ignore

        logger.LogInformation(sprintf "Live Data: %A" l)
        logger.LogInformation(sprintf "Id: %s" id)