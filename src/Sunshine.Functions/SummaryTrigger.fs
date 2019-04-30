module SummaryTrigger

open Microsoft.Extensions.Logging
open Microsoft.Azure.WebJobs

open LiveData
open Microsoft.WindowsAzure.Storage.Table
open FSharp.Azure.Storage.Table
open AzureTableUtils
open System
open Microsoft.Azure.EventHubs
open System.Text

type Summary =
     { [<PartitionKey>] DeviceId: string
       [<RowKey>] MessageId: string
       CurrentIn: float // Sum Iin for panels
       VoltsIn: float // Sum Vin for panels
       WattsInAggregate: float // Sum Pin for panels
       WattsIn: float // Pin
       CurrentOut: float // Igrid
       VoltsOut: float // Vgrid
       WattsOut: float // Pgrid
       HertzOut: float // Fgrid
       MessageTimestamp: DateTime // SysTime
       CorrelationId: string }

[<FunctionName("SummaryTrigger")>]
let trigger
    ([<EventHubTrigger("live-data", ConsumerGroup = "Summary", Connection = "IoTHubConnectionString")>] eventData: EventData)
    ([<Table("SummaryData", Connection = "DataStorageConnectionString")>] summaryTable: CloudTable)
    (logger: ILogger) =
    async {
       let message = Encoding.UTF8.GetString eventData.Body.Array
       let correlationId = eventData.Properties.["correlationId"].ToString()
       let messageId = eventData.Properties.["messageId"].ToString()

       let parsedData = LiveDataDevice.Parse message
       let findPoint' = findPoint parsedData.Points
       let deviceId = parsedData.DeviceId.ToString()
       let timestamp = epoch.AddSeconds(findPoint' "SysTime")
       
       let summary =
           { DeviceId = deviceId
             MessageId = messageId
             CurrentIn = (findPoint' "Iin1") + (findPoint' "Iin2")
             VoltsIn = (findPoint' "Vin1") + (findPoint' "Vin2")
             WattsInAggregate = (findPoint' "Pin1") + (findPoint' "Pin2")
             WattsIn = findPoint' "Pin"
             CurrentOut = findPoint' "Igrid"
             VoltsOut = findPoint' "Vgrid"
             WattsOut = findPoint' "Pgrid"
             HertzOut = findPoint' "Fgrid"
             MessageTimestamp = timestamp
             CorrelationId = correlationId }

       let! _ = summary |> Insert |> inTableToClientAsync summaryTable

       logger.LogInformation(sprintf "%s: Stored summary %s for device %s" correlationId messageId deviceId)
    } |> Async.StartAsTask