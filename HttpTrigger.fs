namespace Sunshine

open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Host
open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.AspNetCore.Http

module HttpTrigger =
    [<FunctionName("HttpTrigger")>]
    let httpTriggerFunction ([<HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)>] req : HttpRequest) =
        OkObjectResult("Hello!")
