module FProspect.IndexTests

open Xunit
open Swensen.Unquote

open FProspect.Data.Index
open FProspect.Data.Errors
open FProspect.ViewModels.Index
open FProspect.Validators.Index

module IndexTests =
    [<Theory>]
    [<InlineData("test-name")>]
    [<InlineData("okname")>]
    [<InlineData("numbers-not-at-the-start-31")>]
    let ``IndexValidator.ensureObjectName works with OK name`` name =
        ensureObjectName <| name =! Ok name

    [<Theory>]
    [<InlineData("123-starts-with-num")>]
    [<InlineData("000-starts-with-num")>]
    [<InlineData("-starts-with-weird")>]
    [<InlineData("$tarts-with-weird")>]
    let ``IndexValidator.ensureObjectName only accepts strings starting with alphanumerics`` name =
        ensureObjectName <| name =! Error MustStartWithAlphabeticChar

    [<Theory>]
    [<InlineData("invalid_char")>]
    [<InlineData("ca$h-money")>]
    [<InlineData("go@it")>]
    let ``IndexValidator.ensureObjectName only accepts alphanumerical names with dash`` name =
        ensureObjectName <| name =! Error InvalidCharacters

    [<Theory>]
    [<InlineData("")>]
    [<InlineData("x")>]
    let ``IndexValidator.ensureObjectName checks min length of name`` name =
        ensureObjectName <| name =! Error TooShort

    [<Fact>]
    let ``IndexValidator.ensureObjectName checks max length of name`` =
        ensureObjectName <| "x123456789012345678901234567890123" =! Error TooLong

    [<Theory>]
    [<InlineData("This-Is-Not-Okay")>]
    [<InlineData("this-also-isnt-Okay")>]
    let ``IndexValidator.ensureObjectName checks character case`` name =
        ensureObjectName <| name =! Error OnlyLowerCaseAllowed

    [<Theory>]
    [<InlineData("this--is-not-ok")>]
    [<InlineData("so-is--this-yea")>]
    let ``IndexValidator.ensureObjectName checks for no duplicate separator`` name =
        ensureObjectName <| name =! Error DoubleSeparator

    [<Theory>]
    [<InlineData("Edm.Boolean")>]
    [<InlineData("Edm.String")>]
    [<InlineData("Edm.Int32")>]
    [<InlineData("Edm.Int64")>]
    [<InlineData("Edm.Double")>]
    let ``IndexValidator.ensureFieldType supports basic types`` name =
        test
            <@ match ensureFieldType name with
               | Ok (Primitive _) -> true
               | _ -> false @>

    [<Theory>]
    [<InlineData("Edm.Boolean")>]
    [<InlineData("Edm.String")>]
    [<InlineData("Edm.Int32")>]
    [<InlineData("Edm.Int64")>]
    [<InlineData("Edm.Double")>]
    let ``IndexValidator.ensureFieldType supports collections`` name =
        test
            <@ match ensureFieldType (sprintf "Collection(%s)" name) with
                | Ok (Collection _) -> true
                | _ -> false @>                                  

    [<Fact>]
    let ``IndexValidator.ensureFieldType checks primitive data types`` () =
        ensureFieldType "Edm.Int128" =! Error (InvalidType UnknownTypeName)

    [<Fact>]
    let ``IndexValidator.ensureField returns proper field with valid data`` () =
        let input =
            { Name = "test-name"
              Type = "Edm.String" }

        let expected : Result<IndexField, FieldError> =
            Ok { Name = "test-name"
                 Type = Primitive String }

        test <@ ensureField <| input = expected @>

    [<Fact>]
    let ``IndexValidator.ensureField returns proper field with valid collection field`` () =
        let input =
            { Name = "test-name-2"
              Type = "Collection(Edm.Int64)" }

        let expected : Result<IndexField, FieldError> =
            Ok { Name = input.Name
                 Type = Collection Long }

        test <@ ensureField <| input = expected @>

    [<Fact>]
    let ``IndexValidator.ensureField returns error when given invalid type`` () =
        let input =
            { Name = "test-name-2"
              Type = "Collection(Edm.Int64?)" }

        let expected : Result<IndexField, FieldError> = Error (InvalidType InvalidCollectionType)

        test <@ ensureField <| input = expected @>

    [<Fact>]
    let ``IndexValidator.ensureField returns error when given invalid non-collection type`` () =
        let input =
            { Name = "test-name-2"
              Type = "Edm.Int128" }

        let expected : Result<IndexField, FieldError> = Error (InvalidType UnknownTypeName)

        test <@ ensureField <| input = expected @>

    [<Fact>]
    let ``IndexValidator.ensureIndex returns error when given invalid name`` () =
        let input : IndexCreateRequest =
            {
                Name = "yea this is invalid"
                Fields = []
            }

        ensureIndex input =! Error (InvalidIndexName InvalidCharacters)
