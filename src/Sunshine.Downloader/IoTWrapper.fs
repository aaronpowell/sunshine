module IoTWrapper
open Microsoft.Azure.Devices.Client
open Microsoft.Azure.Devices.Shared
open System.Threading.Tasks

type IoTConnectionWrapper =
  { SendEventAsync : Message -> Task
    GetTwinAsync : unit -> Async<Twin> }

let getIoTHubClient iotConnStr =
    async {
    return! match iotConnStr with
                              | null | "" ->
                                async {
                                let amqpSetting = AmqpTransportSettings(TransportType.Amqp_Tcp_Only) :> ITransportSettings;
                                let! client = [| amqpSetting |]
                                              |> ModuleClient.CreateFromEnvironmentAsync
                                              |> Async.AwaitTask
                                do! client.OpenAsync() |> Async.AwaitTask
                                return { SendEventAsync = client.SendEventAsync
                                         GetTwinAsync = fun () -> client.GetTwinAsync() |> Async.AwaitTask } }
                              | _ ->
                                async {
                                let client = DeviceClient.CreateFromConnectionString iotConnStr
                                return { SendEventAsync = client.SendEventAsync
                                         GetTwinAsync = fun () -> client.GetTwinAsync() |> Async.AwaitTask } } }

open FSharp.Data
type DesiredProperties = JsonProvider<""" {"inverter":{"username":"user","password":"pass","url":"http://localhost/"},"$version":4} """>

let parseDesiredProperties (data : TwinCollection) =
  data.ToJson() |> DesiredProperties.Parse