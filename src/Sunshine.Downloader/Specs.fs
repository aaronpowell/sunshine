module Specs

open FSharp.Data

type Spec = JsonProvider<"./sample-data/specs.json">

let urlPath = "/v1/specs"

let getSpec getData =
    async {
        let! data = getData urlPath

        return Spec.Parse data
    }