module Contract

open System.Runtime.Serialization

[<DataContract>]
type DocumentIndexRequest =
    {
        [<field: DataMember(Name = "name")>]
        Name : string
        
        [<field: DataMember(Name = "phrase")>]
        Phrase : string   }

[<DataContract>]
type SearchResponse =
    {
        [<field: DataMember(Name = "name")>]
        Name : string

        [<field: DataMember(Name = "phrase")>]
        Phrase : string

        [<field: DataMember(Name = "_score")>]
        Score: float32    }
