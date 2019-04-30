module LiveListApi
open Ids
open Giraffe
open Newtonsoft.Json

type Point =
      { Name: string
        Unit: string
        Description: string
        Type: string
        Kind: string
        [<JsonProperty("decimal_precision")>]
        DecimalPrecision: int }

type Device(deviceId: string, ``type``: string, points: Point array) =
      [<JsonProperty("device_id")>]
      member __.DeviceId = deviceId
      member __.Type = ``type``
      member __.Points = points

type Inverter(deviceId, points, model: string) =
    inherit Device(deviceId, "http://power-one.com/device/inverter/v2/", points)

    [<JsonProperty("device_type")>]
    member __.DeviceType = "inverter_1phase"
    [<JsonProperty("device_model")>]
    member __.DeviceModel = model

type LiveList =
      { [<JsonProperty("Devices")>]Devices: Device array }

let getLiveListData next ctx =
    let logger = Device(loggerId, "http://power-one.com/device/inverter/v1/", Array.empty)

    let inverterPoints = [|
        { Name = "CountryStd"
          Unit = ""
          Description = "Country Std"
          Type = "out"
          Kind = ""
          DecimalPrecision = 2 };
        { Name =  "InputMode"
          Unit = ""
          Description = "Input Mode"
          Type = "in"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "NumOfMPPT"
          Unit = ""
          Description = "Number of DC inputs"
          Type = "in"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "WRtg"
          Unit = "W"
          Description = "Nameplate Maximum Power "
          Type = "out"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "Iin1"
          Unit = "A"
          Description = "Iin1"
          Type = "in"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "Vin1"
          Unit = "V"
          Description = "Vin1"
          Type = "in"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "Pin1"
          Unit = "W"
          Description = "Pin1"
          Type = "in"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "Iin2"
          Unit = "A"
          Description = "Iin2"
          Type = "in"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "Vin2"
          Unit = "V"
          Description = "Vin2"
          Type = "in"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "Pin2"
          Unit = "W"
          Description = "Pin2"
          Type = "in"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "Pin"
          Unit = "W"
          Description = "Pin"
          Type = "in"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "Igrid"
          Unit = "A"
          Description = "Iout"
          Type = "out"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "Pgrid"
          Unit = "W"
          Description = "Pout"
          Type = "out"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "Vgrid"
          Unit = "V"
          Description = "Vout"
          Type = "out"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "Fgrid"
          Unit = "Hz"
          Description = "Fout"
          Type = "out"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "Ppeak"
          Unit = "W"
          Description = "Pout Peak of the day "
          Type = "out"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "cosPhi"
          Unit = ""
          Description = "Cos phi"
          Type = "out"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "SplitPhase"
          Unit = ""
          Description = "Split Phase"
          Type = "out"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "VgridL1_N"
          Unit = "V"
          Description = "VoutL1_N"
          Type = "out"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "VgridL2_N"
          Unit = "V"
          Description = "VoutL2_N"
          Type = "out"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "PacTogrid"
          Unit = "W"
          Description = "PacTogrid"
          Type = "out"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "Fan1rpm"
          Unit = ""
          Description = "Fan 1 rpm"
          Type = "other"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "Temp1"
          Unit = "degC"
          Description = "Ambient stage temperature"
          Type = "other"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "TempInv"
          Unit = "degC"
          Description = "Inverter stage temperature"
          Type = "other"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "TempBst"
          Unit = "degC"
          Description = "Booster stage temperature"
          Type = "other"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "Riso"
          Unit = "MOhm"
          Description = "Riso"
          Type = "other"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "IleakInv"
          Unit = "uA"
          Description = "Ileak AC"
          Type = "other"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "IleakDC"
          Unit = "uA"
          Description = "Ileak DC"
          Type = "other"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "Vgnd"
          Unit = "V"
          Description = "Vgnd"
          Type = "other"
          Kind = ""
          DecimalPrecision = 1
        };
        { Name =  "SysTime"
          Unit = ""
          Description = "System Time"
          Type = "other"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "EDay_i"
          Unit = "Wh"
          Description = "Energy today"
          Type = "statistics"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "EWeek_i"
          Unit = "Wh"
          Description = "Energy current week"
          Type = "statistics"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "EMonth_i"
          Unit = "Wh"
          Description = "Energy current month"
          Type = "statistics"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "EYear_i"
          Unit = "Wh"
          Description = "Energy current year"
          Type = "statistics"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "ETotal"
          Unit = "Wh"
          Description = "Lifetime Energy"
          Type = "statistics"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "E0_7D"
          Unit = "Wh"
          Description = "Energy last 7 days"
          Type = "statistics"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "E0_30D"
          Unit = "Wh"
          Description = "Energy last 30 days"
          Type = "statistics"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "GlobState"
          Unit = ""
          Description = "Global State"
          Type = "other"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "AlarmState"
          Unit = ""
          Description = "Alarm State"
          Type = "other"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "DC1State"
          Unit = ""
          Description = "DC/DC_1 State"
          Type = "other"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "DC2State"
          Unit = ""
          Description = "DC/DC_2 State"
          Type = "other"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "InvState"
          Unit = ""
          Description = "Inverter State"
          Type = "other"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "WarningFlags"
          Unit = ""
          Description = "Warning flags"
          Type = "integer"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "PACDeratingFlags"
          Unit = ""
          Description = "PAC Derating flags"
          Type = "integer"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "QACDeratingFlags"
          Unit = ""
          Description = "QAC Derating flags"
          Type = "integer"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "SACDeratingFlags"
          Unit = ""
          Description = "SAC Derating flags"
          Type = "integer"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "FRT_state"
          Unit = ""
          Description = "FRT status"
          Type = "integer"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "ClockState"
          Unit = ""
          Description = "Clock State"
          Type = "integer"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "m126Mod_Ena"
          Unit = ""
          Description = "SunSpec Actual Curve Enable map 126"
          Type = "other"
          Kind = ""
          DecimalPrecision = 2
        };
        { Name =  "m132Mod_Ena"
          Unit = ""
          Description = "SunSpec Actual Curve Enable map 132"
          Type = "other"
          Kind = ""
          DecimalPrecision = 2
        }
    |]

    let inverter = Inverter(inverterId, inverterPoints, "")

    json { Devices = [| logger; inverter |] } next ctx