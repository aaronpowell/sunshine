module IoTWrapper
open Microsoft.Azure.Devices.Client
open System.Threading.Tasks

type IoTConnectionWrapper =
  { SendEventAsync : Message -> Task }

let getIotHubClient iotConnStr =
    async {
    return! match iotConnStr with
                              | null | "" ->
                                async {
                                let amqpSetting = AmqpTransportSettings(TransportType.Amqp_Tcp_Only) :> ITransportSettings;
                                let! client = [| amqpSetting |]
                                              |> ModuleClient.CreateFromEnvironmentAsync
                                              |> Async.AwaitTask
                                do! client.OpenAsync() |> Async.AwaitTask
                                return { SendEventAsync = client.SendEventAsync } }
                              | _ ->
                                async {
                                let client = DeviceClient.CreateFromConnectionString(iotConnStr)
                                return { SendEventAsync = client.SendEventAsync } } }
