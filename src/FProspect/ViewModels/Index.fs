module FProspect.ViewModels.Index

open System.Runtime.Serialization

[<DataContract>]
type IndexFieldRequest =
    { [<field: DataMember(Name = "name")>]
      Name : string

      [<field: DataMember(Name = "type")>]
      Type : string }
    
[<DataContract>]
type IndexCreateRequest =
    { [<field: DataMember(Name = "name")>]
      Name : string

      [<field: DataMember(Name = "fields")>]
      Fields : IndexFieldRequest list }

[<DataContract>]
type IndexCreateResponse =
    { [<field: DataMember(Name = "name")>]
      Name : string }

