namespace Composite.Bson

open Composite

[<RequireQualifiedAccess>]
module BsonComposite =

    ///<summary>Creates a new marked sequence composite based on the given BSON bytes.</summary>
    ///<param name="bson">A BSON presented as a byte array.</param>
    let FromBytes (bson: byte[]) =
        MComp.ofBsonBytes bson