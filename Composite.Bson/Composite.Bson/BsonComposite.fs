namespace Composite.Bson

open System.IO

[<RequireQualifiedAccess>]
module BsonComposite =

    ///<summary>Initializes a new BSON sequence composite with an object at the root.</summary>
    let New () =
        BsonComp.create ()

    ///<summary>Creates a new marked sequence composite that will use the given BSON stream as source.</summary>
    ///<param name="inputStream">A BSON stream.</param>
    let FromStream (inputStream: Stream) =
        BsonComp.ofBsonStream inputStream