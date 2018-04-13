namespace Composite.Bson

open System.Runtime.CompilerServices

open FSharp.Core
open Composite

[<Extension>]
type CompositeExtensions () =

    [<Extension>]
    ///<summary>Converts a BSON composite to BSON.</summary>
    ///<param name="source">The input composite.</param>
    static member inline ToBson (source: Composite<BsonElementMark, obj>)=
        source |> MComp.toBson
