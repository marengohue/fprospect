module FProspect.Utils.Functional

let rec choose<'a>(opts : 'a option list) =
    match opts with
    | [] -> None
    | opt :: tail ->
        match opt with
        | Some _ -> opt
        | None -> choose tail
