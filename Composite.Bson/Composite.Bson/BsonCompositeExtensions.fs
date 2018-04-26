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

    [<Extension>]
    ///<summary>Ensures that an array will be present in the container marked with the given path and will be marked with the following name.</summary>
    ///<param name="source">A BSON composite.</param>
    ///<param name="containerPath">The path of the container.</param>
    ///<param name="name">The name of the array.</param>
    static member inline EnsureHasArray (source: Composite<BsonElementMark, obj>) (containerPath: string) (name: string) =
        source |> BsonComp.ensureHasArray containerPath name

    [<Extension>]
    ///<summary>Ensures that an object will be present in the container marked with the given path and will be marked with the following name.</summary>
    ///<param name="source">A BSON composite.</param>
    ///<param name="containerPath">The path of the container.</param>
    ///<param name="name">The name of the object.</param>
    static member inline EnsureHasObject (source: Composite<BsonElementMark, obj>) (containerPath: string) (name: string) =
        source |> BsonComp.ensureHasObject containerPath name

    [<Extension>]
    ///<summary>Set the value of the given name in the container marked with the given path.</summary>
    ///<param name="source">A BSON composite.</param>
    ///<param name="containerPath">The path of the container.</param>
    ///<param name="name">The name of the value.</param>
    ///<param name="name">The value.</param>
    static member inline SetValue (source: Composite<BsonElementMark, obj>) (containerPath: string) (name: string) (value: obj) =
        source |> BsonComp.setValue containerPath name value