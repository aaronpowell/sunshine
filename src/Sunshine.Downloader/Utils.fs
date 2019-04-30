module Utils
open Microsoft.Azure.Devices.Client
open System.Text
open System

let inline toS o = o.ToString()

let sendIoTMessage<'T> (client : DeviceClient) route  correlationId (obj : 'T) =
    let json = obj |> toS
    let msg = new Message(Encoding.ASCII.GetBytes json)
    msg.Properties.Add("__messageType", route)
    msg.Properties.Add("correlationId", correlationId.ToString())
    msg.Properties.Add("messageId", Guid.NewGuid().ToString())
    client.SendEventAsync(msg) |> Async.AwaitTask