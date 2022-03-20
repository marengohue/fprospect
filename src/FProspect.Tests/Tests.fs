module FProspect.Tests

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
        ensureObjectName <| name =! Error (InvalidObjectName MustStartWithAlphabeticChar)

    [<Theory>]
    [<InlineData("invalid_char")>]
    [<InlineData("ca$h-money")>]
    [<InlineData("go@it")>]
    let ``IndexValidator.ensureObjectName only accepts alphanumerical names with dash`` name =
        ensureObjectName <| name =! Error (InvalidObjectName InvalidCharacters)

    [<Theory>]
    [<InlineData("")>]
    [<InlineData("x")>]
    let ``IndexValidator.ensureObjectName checks min length of name`` name =
        ensureObjectName <| name =! Error (InvalidObjectName TooShort)

    [<Fact>]
    let ``IndexValidator.ensureObjectName checks max length of name`` =
        ensureObjectName <| "x123456789012345678901234567890123" =! Error (InvalidObjectName TooLong)

    [<Theory>]
    [<InlineData("This-Is-Not-Okay")>]
    [<InlineData("this-also-isnt-Okay")>]
    let ``IndexValidator.ensureObjectName checks character case`` name =
        ensureObjectName <| name =! Error (InvalidObjectName OnlyLowerCaseAllowed)

    [<Theory>]
    [<InlineData("this--is-not-ok")>]
    [<InlineData("so-is--this-yea")>]
    let ``IndexValidator.ensureObjectName checks for no duplicate separator`` name =
        ensureObjectName <| name =! Error (InvalidObjectName DoubleSeparator)

    [<Theory>]
    [<InlineData("Edm.Boolean")>]
    [<InlineData("Edm.String")>]
    [<InlineData("Edm.Int32")>]
    [<InlineData("Edm.Int64")>]
    [<InlineData("Edm.Double")>]
    let ``IndexValidator.ensureFieldType supports basic types`` name =
        test
            <@ match ensureFieldType name with
               | Ok _ -> true
               | _ -> false @>

    [<Fact(Skip = "for now")>]
    let ``IndexValidator.ensureField returns proper field with valid data`` () =
        let input =
            { Name = "test-name"
              Type = "string" }

        let expected : Result<IndexField, unit> =
            Ok { Name = "test-name"
                 Type = FieldType.String }

        test <@ ensureField <| input = expected @>

