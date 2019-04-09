module Utils
open Microsoft.Azure.Devices.Client
open System.Text
open System

let inline toS o = o.ToString()

let sendIoTMessage (client : DeviceClient) (obj : Object) =
    let json = obj |> toS
    let msg = new Message(Encoding.ASCII.GetBytes json)
    client.SendEventAsync(msg) |> Async.AwaitTask