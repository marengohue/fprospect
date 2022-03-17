module Index

open System.Collections.Generic

open Lucene.Net.Analysis.Standard
open Lucene.Net.Index
open Lucene.Net.Search
open Lucene.Net.Store
open Lucene.Net.Util
open Lucene.Net.Documents

let luceneVer = LuceneVersion.LUCENE_48

// Setup analyzer
let analyzer = new StandardAnalyzer(luceneVer)

// Setup index
let idxDir = new RAMDirectory()
let idxConfig = new IndexWriterConfig(luceneVer, analyzer)
let idxWriter = new IndexWriter(idxDir, idxConfig)

let indexDocument (data : Contract.DocumentIndexRequest) =
    let doc = new Document()
    doc.Add(new TextField("name", data.Name, Field.Store.YES))
    doc.Add(new TextField("favoritePhrase", data.Phrase, Field.Store.YES))

    idxWriter.AddDocument(doc)
    idxWriter.Flush(false, false)
    printf "Added document. Now %A docs total.\n" idxWriter.NumDocs

let search (hits : int) terms : Contract.SearchResponse[] =
    let idxReader = idxWriter.GetReader(true)
    let searcher = new IndexSearcher(idxReader)

    let query = new MultiPhraseQuery()
    terms
    |> String.split ' '
    |> List.map (fun word -> new Term("favoritePhrase", word))
    |> List.iter query.Add
    
    let searchResult = searcher.Search(query, hits)
    printf "Hits: %i\n" (searchResult.TotalHits)
    
    searchResult.ScoreDocs
    |> Array.map (fun hit ->
        let doc = searcher.Doc(hit.Doc)
        { Name = doc.Get("name")
          Phrase = doc.Get("favoritePhrase")
          Score = hit.Score }
    )
