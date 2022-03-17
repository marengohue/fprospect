open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.Utils.Collections
open Suave.Json

let runSearch data =
    Option.ofChoice (data ^^ "q")
    |> Option.defaultValue "*"
    |> Index.search 20

let getRoutes =
    GET >=> choose
        [ path "/" >=> OK "FProspect is all green"
          path "/search"
              >=> request (fun r -> (runSearch r.query) |> toJson |> Successful.ok)
              >=> Writers.setMimeType "application/json" ]

let postRoutes =
    POST >=> choose
        [ path "/new" >=> request (fun r ->
            fromJson (r.rawForm) |> Index.indexDocument |> ignore
            OK "OK"
         )]
          
let allRoutes = choose [ getRoutes ; postRoutes ]
startWebServer defaultConfig allRoutes
