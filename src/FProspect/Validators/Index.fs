module FProspect.Validators.Index

open System
open System.Text.RegularExpressions

open FProspect.Utils
open FProspect.Data.Errors
open FProspect.Data.Index

let testError regex error candidate =
    if Regex.IsMatch(candidate, regex)
    then Error error
    else Ok candidate

let validateStartCharUse =
    testError "^[^a-zA-z].+$" (InvalidObjectName MustStartWithAlphabeticChar)

let validateAllowedCharacters =
    testError "[^A-Za-z\d\-]" (InvalidObjectName InvalidCharacters)

let validateNameLength (candidate : string) =
    match candidate.Length with
    | x when x <= 3 -> Error (InvalidObjectName TooShort)
    | x when x > 32 -> Error (InvalidObjectName TooLong)
    | _ -> Ok candidate

let validateNameCase =
    testError "[A-Z]" (InvalidObjectName OnlyLowerCaseAllowed)

let validateSeparator sep =
    testError (sprintf "%s%s" sep sep) (InvalidObjectName DoubleSeparator)

let ensureObjectName =
    validateStartCharUse
    >> Result.bind validateAllowedCharacters
    >> Result.bind validateNameLength
    >> Result.bind validateNameCase
    >> Result.bind (validateSeparator "-")
    

let ensureBasicType =
    function
    | "Edm.String" -> Some String
    | "Edm.Boolean" -> Some Boolean
    | "Edm.Int32" -> Some Integer
    | "Edm.Int64" -> Some Long
    | "Edm.Double" -> Some Double
    | _ -> None

let ensureCollectionType =
    Regex.tryMatch "Collection(?<collType>.+)"
    >> Option.bind (Map.tryFind "collType")
    >> Option.map
        (fun x ->
         match ensureBasicType x with
         | Some basicType -> Ok (Collection basicType)
         | _ -> Error InvalidCollectionType)

let ensureFieldType _ =
    Ok String

let ensureField _ =
    new System.NotImplementedException() |> raise
