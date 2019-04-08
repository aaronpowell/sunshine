module SpecsApi

open Giraffe
open Ids

type Logger = { MacAddress: string }
type Device = {
    ModelId: string
    ModelIdDescr: string
    MeterCompatibility: bool
    DeviceId: string
    InputChannelNumber: int
    OutputPhaseNumber: int
    EthernetPresence: bool
}

type Spec = {
    Logger: Logger
    Device: Device
}

let getSpec next ctx =
    json { Device =
            { ModelId = "0x03DD"
              ModelIdDescr = "MOCK-DEVICE"
              MeterCompatibility = true
              DeviceId = inverterId
              InputChannelNumber = 2
              OutputPhaseNumber = 1
              EthernetPresence = false }
           Logger =
            { MacAddress = loggerId }
        }
        next
        ctx