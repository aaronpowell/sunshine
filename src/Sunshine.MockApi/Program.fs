open Saturn
open SpecsApi
open LiveListApi
open LiveDataApi
open FeedApi
open Microsoft.Extensions.DependencyInjection
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Giraffe.Serialization
open BasicAuth
open Giraffe

let port = 80

let endpointPipe = pipeline {
    plug head
    plug requestId
}

let matchUpUsers : HttpHandler = fun next ctx ->
    next ctx

let authPipeline = pipeline {
    requires_authentication (Giraffe.Auth.challenge "BasicAuthentication")
    plug matchUpUsers
}

[<EntryPoint>]
let main argv =
    let serviceConfig (services: IServiceCollection) =
        let cr = CamelCasePropertyNamesContractResolver()
        cr.NamingStrategy.OverrideSpecifiedNames <- false
        let customSettings = JsonSerializerSettings(
                                ContractResolver = cr)

        services.AddSingleton<IJsonSerializer>(
            NewtonsoftJsonSerializer(customSettings))

    let webApp = router {
        pipe_through authPipeline
        get "/v1/specs" getSpec
        get "/v1/livedata/list" getLiveListData
        get "/v1/livedata" getLiveData
        get "/v1/feeds" getFeedData
    }
    let app = application {
        pipe_through endpointPipe
        url ("http://0.0.0.0:" + port.ToString() + "/")
        use_router webApp
        memory_cache
        use_gzip
        service_config serviceConfig
        use_basic_auth
    }

    run app
    0 // return an integer exit code
