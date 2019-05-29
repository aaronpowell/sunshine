module InverterApi

open FSharp.Data
open System
open FSharp.Data.HttpRequestHeaders
open System.Text

let getAuthToken username password =
    sprintf "%s:%s" username password
    |> ASCIIEncoding.ASCII.GetBytes
    |> Convert.ToBase64String

let getData authToken baseUri (path : string) =
    let url = Uri(baseUri, path)
    printfn "Requesting: %s" (url.ToString())
    Http.AsyncRequestString
        ( url.ToString(),
          httpMethod = "GET",
          headers = [ Accept HttpContentTypes.Json
                      Authorization (sprintf "Basic %s" authToken) ] )