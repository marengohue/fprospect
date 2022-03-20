module FProspect.Utils.Regex

open System.Text.RegularExpressions

let tryMatch pattern input =
    let m = Regex.Match(input, pattern) in
    if m.Success then Some (Map.ofSeq [ for g in m.Groups -> g.Name, g.Value ]) else None

let (|Match|_|) = tryMatch
