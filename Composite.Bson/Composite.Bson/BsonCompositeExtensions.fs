namespace Composite.Bson

open System.IO
open System.Runtime.CompilerServices

open FSharp.Core
open Composite

[<Extension>]
type BsonCompositeExtensions () =

    [<Extension>]
    ///<summary>Writes the contents of a BSON composite to an output stream.</summary>
    ///<param name="source">The input composite.</param>
    ///<param name="outputStream">The output stream.</param>
    static member inline WriteToStream (source: Composite<BsonElementMark, obj>) (outputStream: Stream)=
        source |> BsonComp.writeToStream outputStream
