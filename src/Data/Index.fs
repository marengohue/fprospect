module Data.Index

type Index =
  { Name : string }

type FieldType =
| Text
| Integer
| Float
| Collection of FieldType

type IndexField =
  { Name : string
    Type : FieldType }
    
