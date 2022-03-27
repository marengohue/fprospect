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

type FieldError =
| InvalidFieldName of ObjectNameError
| InvalidType of TypeError

type IndexError =
| InvalidIndexName of ObjectNameError
| InvalidField of (int*FieldError)
