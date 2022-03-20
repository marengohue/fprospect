module FProspect.Data.Errors

type ObjectNameError =
| MustStartWithAlphabeticChar
| InvalidCharacters
| TooShort
| TooLong
| OnlyLowerCaseAllowed
| DoubleSeparator

type TypeError =
| InvalidCollectionType
| UnknownTypeName

type Error =
| InvalidObjectName of ObjectNameError
| InvalidType of TypeError
