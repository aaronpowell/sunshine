module HttpTrigger

open Microsoft.Azure.WebJobs
open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.AspNetCore.Http
open Microsoft.WindowsAzure.Storage.Table
open System
open System.Web.Http
open FSharp.Azure.Storage.Table

open Payload
open Device
open AzureTableUtils
open Point

let deviceId = Environment.GetEnvironmentVariable "DEVICE_ID"

[<FunctionName("ReciveData")>]
let httpTriggerFunction 
    ([<HttpTrigger(AuthorizationLevel.Function, "post", Route = null)>] req : HttpRequest)
    ([<Table("Devices")>] devicesSource: CloudTable)
    ([<Table("Points")>] pointsSource: CloudTable) =
    async {
        let! payload = parsePayload req.Body

        let deviceData = getDevicePayload deviceId payload

        let device = payloadToDevice deviceData

        let! result = device
                      |> Insert
                      |> inTableToClientAsync devicesSource

        match result.HttpStatusCode with
        | 204 ->
            let payloadToPoint' = payloadToPoint device.RowKey
            let points = deviceData.Points |> Array.map payloadToPoint'

            let! _ = (Array.map (Insert >> (fun op -> inTableToClientAsync pointsSource op)) points)
                          |> Async.Parallel

            return OkResult() :> IActionResult
        | _ ->
            return InternalServerErrorResult() :> IActionResult
    } |> Async.StartAsTask