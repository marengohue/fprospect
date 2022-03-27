module FProspect.Validators.Index

open System.Text.RegularExpressions

open FProspect.Utils
open FProspect.Utils.Functional
open FProspect.ViewModels.Index
open FProspect.Data.Errors
open FProspect.Data.Index

let testError regex error candidate =
    if Regex.IsMatch(candidate, regex)
    then Error error
    else Ok candidate

let validateStartCharUse =
    testError "^[^a-zA-z].+$" MustStartWithAlphabeticChar

let validateAllowedCharacters =
    testError "[^A-Za-z\d\-]" InvalidCharacters

let validateNameLength (candidate : string) =
    match candidate.Length with
    | x when x <= 3 -> Error TooShort
    | x when x > 32 -> Error TooLong
    | _ -> Ok candidate

let validateNameCase =
    testError "[A-Z]" OnlyLowerCaseAllowed

let validateSeparator sep =
    testError (sprintf "%s%s" sep sep) DoubleSeparator

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

let ensurePrimitiveType =
    ensureBasicType
    >> Option.map (fun it -> Ok (Primitive it))

let ensureCollectionType =
    Regex.tryMatch "Collection\((?<collType>.+)\)"
    >> Option.bind (Map.tryFind "collType")
    >> Option.map (fun x ->
        match ensureBasicType x with
        | Some basicType -> Ok (Collection basicType)
        | _ -> Error InvalidCollectionType)

let ensureFieldType typeName =
    match choose [
        ensureCollectionType typeName
        ensurePrimitiveType typeName ] with
    | Some result -> result // This can also yield error result
    | None -> Error UnknownTypeName
    |> Result.mapError (fun typeError -> InvalidType typeError)

let ensureFieldName =
    ensureObjectName >> Result.mapError InvalidFieldName

let ensureIndexName =
    ensureObjectName >> Result.mapError InvalidIndexName

let ensureField (fieldDef : IndexFieldRequest) =
    ensureFieldName fieldDef.Name
    |> Result.bind (fun validName ->
        ensureFieldType fieldDef.Type
        |> Result.map (fun validFieldType -> (validName, validFieldType)))
    |> Result.map (fun (name, fieldType) -> { Name = name; Type = fieldType })

let ensureIndex (indexDef : IndexCreateRequest) =
    ensureIndexName indexDef.Name
    |> Result.bind (fun validName ->
        indexDef.Fields
        |> List.mapi (fun index field ->
            field
            |> ensureField
            |> Result.mapError (fun err -> (index, err)))
        |> List.sequenceResultA
        |> Result.map2
            (fun fields -> (validName, fields))
            (fun err -> InvalidField err)
    )
    |> Result.map (fun (name, fields) -> { Name = name; Fields = fields })


