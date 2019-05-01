module BasicAuth

open Saturn
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Authentication
open System.Security.Claims
open FSharp.Control.Tasks.V2.ContextSensitive
open System.Net.Http.Headers
open System
open System.Text
open System.Security.Claims

type IUserService =
    abstract member AuthenticateAsync : string -> string -> Async<bool>

type UserService() =
    let users = [("aaron", "password")] |> Map.ofList

    interface IUserService with

        member __.AuthenticateAsync username password =
            async {
                return match users.TryGetValue username with
                       | (true, user) when user = password -> true
                       | _ -> false
            }

type Credentials =
     { Username: string
       Password: string }

let getCreds headerValue =
    let value = AuthenticationHeaderValue.Parse headerValue
    let bytes = Convert.FromBase64String value.Parameter
    let creds = (Encoding.UTF8.GetString bytes).Split([|':'|])

    { Username = creds.[0]; Password = creds.[1] }

type BasicAuthHandler(options, logger, encoder, clock, userService : IUserService) =
    inherit AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)

    override this.HandleAuthenticateAsync() =
        let request = this.Request
        match request.Headers.TryGetValue "Authorization" with
        | (true, headerValue) ->
            async {
                let creds = getCreds headerValue.[0]

                let! userFound = userService.AuthenticateAsync creds.Username creds.Password

                return match userFound with
                       | true ->
                            let claims = [| Claim(ClaimTypes.NameIdentifier, creds.Username); Claim(ClaimTypes.Name, creds.Username) |]
                            let identity = ClaimsIdentity(claims, this.Scheme.Name)
                            let principal = ClaimsPrincipal identity
                            let ticket = AuthenticationTicket(principal, this.Scheme.Name)
                            AuthenticateResult.Success ticket
                       | false ->
                            AuthenticateResult.Fail("Invalid Username or Password")
            } |> Async.StartAsTask
        | (false, _) ->
            task { return AuthenticateResult.Fail("Missing Authorization Header") }

type ApplicationBuilder with
    [<CustomOperationAttribute("use_basic_auth")>]
    member __.UseBasicAuth(state : ApplicationState) =
        let middleware (app : IApplicationBuilder) =
            app.UseAuthentication()

        let service (s : IServiceCollection) =
            s.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("BasicAuthentication", null)
                |> ignore

            s.AddTransient<IUserService, UserService>() |> ignore
            s

        { state with
            ServicesConfig = service::state.ServicesConfig
            AppConfigs = middleware::state.AppConfigs }