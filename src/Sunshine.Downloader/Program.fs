// Learn more about F# at http://fsharp.org

open System
open InverterApi
open Specs

let baseUrl = Environment.GetEnvironmentVariable("SUNSHINE_URL")
let username = Environment.GetEnvironmentVariable("SUNSHINE_USERNAME")
let password = Environment.GetEnvironmentVariable("SUNSHINE_PASSWORD")

[<EntryPoint>]
let main argv =
    let token = getAuthToken username password
    let getData' = getData token (Uri baseUrl)

    let specs = (getSpec getData') |> Async.RunSynchronously

    sprintf "%A" specs
    |> Console.WriteLine

    0 // return an integer exit code
