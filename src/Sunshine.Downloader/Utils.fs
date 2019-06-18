module Utils
open Microsoft.Azure.Devices.Client
open System.Text
open System
open IoTWrapper

let inline toS o = o.ToString()

let sendIoTMessage<'T> client route  correlationId (obj : 'T) =
    let json = obj |> toS
    let msg = new Message(Encoding.ASCII.GetBytes json)
    msg.Properties.Add("__messageType", route)
    msg.Properties.Add("correlationId", correlationId.ToString())
    msg.Properties.Add("messageId", Guid.NewGuid().ToString())
    printfn "Submitting %s with correlationId %A" route correlationId
    client.SendEventAsync msg

let errorLogger msg ex =
    let now = DateTimeOffset.Now
    let dateFormat = now.ToString "dd-MM-yy hh:mm:ss"

    printfn "[%s] %s" dateFormat msg
    printfn "[%s] %A" dateFormat ex