module LiveDataTrigger

open Microsoft.Extensions.Logging
open Microsoft.Azure.WebJobs

open LiveData
open Microsoft.WindowsAzure.Storage.Table
open FSharp.Azure.Storage.Table
open AzureTableUtils
open System
open Microsoft.Azure.EventHubs
open System.Text

type LiveDataEntity =
     { [<PartitionKey>] DeviceId: string
       [<RowKey>] MessageId: string
       RawMessage: string
       MessageTimestamp: DateTime // SysTime
       CorrelationId: string }

[<FunctionName("LiveDataTrigger")>]
let trigger
    ([<EventHubTrigger("live-data", ConsumerGroup = "Raw", Connection = "IoTHubConnectionString")>] eventData: EventData)
    ([<Table("LiveData", Connection = "DataStorageConnectionString")>] liveDataTable: CloudTable)
    (logger: ILogger) =
    async {
       let message = Encoding.UTF8.GetString eventData.Body.Array
       let correlationId = eventData.Properties.["correlationId"].ToString()
       let messageId = eventData.Properties.["messageId"].ToString()

       let parsedData = LiveDataDevice.Parse message
       let findPoint' = findPoint parsedData.Points
       let deviceId = parsedData.DeviceId.ToString()
       let timestamp = epoch.AddSeconds(findPoint' "SysTime")

       let rawDataEntity =
           { DeviceId = deviceId
             MessageId = messageId
             RawMessage = message
             MessageTimestamp = timestamp
             CorrelationId = correlationId }
       let! _ = rawDataEntity |> Insert |> inTableToClientAsync liveDataTable

       logger.LogInformation(sprintf "%s: Stored raw message %s for device %s" correlationId messageId deviceId)
    } |> Async.StartAsTask