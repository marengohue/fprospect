module FProspect.Data.Index

type Index =
  { Name : string }

type FieldType =
| String
| Boolean
| Integer
| Long
| Double
| Collection of FieldType

type IndexField =
  { Name : string
    Type : FieldType }
    

