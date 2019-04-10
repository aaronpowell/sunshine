module Specs

open FSharp.Data

type Spec = JsonProvider<"./sample-data/specs.json">

let urlPath = "/v1/specs"

let getSpec getData =
    async {
        try
            let! data = getData urlPath
            return Spec.Parse data

        with
        | _ ->
            printfn "hmm looks like the API isn't available yet, let's sleep a bit"

            do! Async.Sleep 30000

            let! data = getData urlPath
            return Spec.Parse data
    }