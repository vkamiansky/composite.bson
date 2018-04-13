namespace Composite.Bson

    open System

    type BsonValueType =
    | BsonString
    | BsonInteger
    | BsonFloat
    | BsonDate
    | BsonBytes
    | BsonBoolean
    | BsonRaw
    | BsonEmpty
    | BsonUnknown

    type BsonConstructorMark = { Name: string; Property: string; Path: string }

    type BsonContainerMark = { Property: string; Path: string }

    type BsonValueMark = { Type: BsonValueType; Property: string; Path: string }

    type private BsonElementMarkStatic = BsonElementMark
    and BsonElementMark =
        | DocumentMark
        | ArrayMark of BsonContainerMark
        | ObjectMark of BsonContainerMark
        | ConstructorMark of BsonConstructorMark
        | ValueMark of BsonValueMark
        static member ToString x =
            match x with
            | DocumentMark -> "Document"
            | ObjectMark {Property=prop; Path=path } -> if String.IsNullOrWhiteSpace prop && String.IsNullOrWhiteSpace path
                                                        then sprintf "Object"
                                                        else sprintf "Object-Property: %s, Path: %s" prop path
            | ArrayMark {Property=prop; Path=path } -> if String.IsNullOrWhiteSpace prop && String.IsNullOrWhiteSpace path
                                                       then sprintf "Array"
                                                       else sprintf "Array-Property: %s, Path: %s" prop path
            | ConstructorMark {Name=name; Property=prop; Path=path } -> if String.IsNullOrWhiteSpace prop && String.IsNullOrWhiteSpace path
                                                                        then sprintf "Ctor Name: %s" name
                                                                        else sprintf "Ctor-Property: %s, Name: %s, Path: %s" prop name path
            | ValueMark {Type=valueType; Property=prop; Path=path } -> if String.IsNullOrWhiteSpace prop && String.IsNullOrWhiteSpace path
                                                                       then sprintf "%A" valueType
                                                                       else sprintf "%A-Property: %s, Path: %s" valueType prop path
        override x.ToString() = BsonElementMarkStatic.ToString x 
