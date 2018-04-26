namespace Composite.Bson

open System
open System.IO

[<RequireQualifiedAccess>]
module BsonComposite =

    ///<summary>Initializes a new BSON sequence composite with an object at the root.</summary>
    let New () =
        BsonComp.create ()

    ///<summary>Creates a new marked sequence composite that will use the BSON stream acquired through the use of the given function as source.</summary>
    ///<param name="getInputStream">A BSON stream producing function.</param>
    let FromStreamFunction (getInputStream: Func<Stream>) =
        BsonComp.ofBsonStreamFunction getInputStream.Invoke

    ///<summary>Creates a new marked sequence composite that will use the given BSON stream as source.</summary>
    ///<param name="inputStream">A BSON stream.</param>
    let FromStream (inputStream: Stream) =
        BsonComp.ofBsonStream inputStream