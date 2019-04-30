module LiveListTrigger

open Microsoft.Extensions.Logging
open Microsoft.Azure.WebJobs

open LiveData
open Microsoft.Azure.EventHubs
open System.Text
open FSharp.Azure.Storage.Table
open Microsoft.WindowsAzure.Storage.Table
open AzureTableUtils

type LiveListPoint =
     { [<PartitionKey>] CorrelationId: string
       [<RowKey>] FieldName: string
       MessageId: string
       DeviceId: string
       Unit: string
       Description: string
       Type: string
       Kind: string
       DecimalPrecision: int }

type LiveListRaw =
     { [<PartitionKey>] CorrelationId: string
       [<RowKey>] Id: string
       DeviceId: string
       RawMessage: string }

[<FunctionName("LiveListTrigger")>]
let trigger
    ([<EventHubTrigger("live-list", Connection = "IoTHubConnectionString")>] eventData: EventData)
    ([<Table("LiveListData", Connection = "DataStorageConnectionString")>] liveListTable: CloudTable)
    ([<Table("LiveListPointsData", Connection = "DataStorageConnectionString")>] liveListPointsTable: CloudTable)
    (logger: ILogger) =
    async {
        let message = Encoding.UTF8.GetString eventData.Body.Array
        let correlationId = string eventData.Properties.["correlationId"]
        let messageId = string eventData.Properties.["messageId"]
        let device = LiveListDevice.Parse message
        let deviceId = string device.DeviceId
        
        let raw =
            { CorrelationId = correlationId
              Id = messageId
              DeviceId = deviceId
              RawMessage = message }

        let! _ = raw |> Insert |> inTableToClientAsync liveListTable

        let tableBatch points = inTableAsBatchAsync liveListPointsTable.ServiceClient liveListPointsTable.Name points
        let ops = device.Points
                     |> Array.map ((fun p -> { CorrelationId = correlationId
                                               DeviceId = deviceId
                                               MessageId = messageId
                                               FieldName = string p.Name
                                               Unit = string p.Unit
                                               Description = string p.Description
                                               Type = string p.Type
                                               Kind = string p.Kind
                                               DecimalPrecision = p.DecimalPrecision }) >> Insert)
                     |> Array.toSeq
                     |> autobatch
                     |> Seq.map tableBatch
        let! _ = Async.Parallel ops

        logger.LogInformation(sprintf "%s: Processed list for %s" correlationId deviceId)
    } |> Async.StartAsTask