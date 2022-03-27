module FProspect.Data.Index

type PrimitiveType =
| String
| Boolean
| Integer
| Long
| Double    

type FieldType =
| Primitive of PrimitiveType
| Collection of PrimitiveType

type IndexField =
  { Name : string
    Type : FieldType }
    
type Index =
  { Name : string
    Fields : IndexField list }

