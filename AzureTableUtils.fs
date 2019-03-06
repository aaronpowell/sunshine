module AzureTableUtils

open Microsoft.WindowsAzure.Storage.Table
open FSharp.Azure.Storage.Table

let fromTableToClientAsync (table: CloudTable) q = fromTableAsync table.ServiceClient table.Name q
let inTableToClientAsync (table: CloudTable) o = inTableAsync table.ServiceClient table.Name o