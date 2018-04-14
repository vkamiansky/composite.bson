namespace Composite.Bson

open FSharp.Core
open System
open System.IO

open Newtonsoft.Json
open Newtonsoft.Json.Bson
open Composite

type KeyValuePair = { Key: string; Value: obj }

[<RequireQualifiedAccess>]
module BsonComp =

    ///<summary>Creates a new marked sequence composite that will use the given BSON stream as source.</summary>
    ///<param name="inputStream">A BSON stream.</param>
    let ofBsonStream (inputStream: Stream) =
        let toPath containerPath prop =
            if (String.IsNullOrWhiteSpace(containerPath) && String.IsNullOrWhiteSpace(prop)) then String.Empty else sprintf "%s.%s" containerPath prop
        let toSomeValueToken elementType containerPath prop value =
            Some(MarkedValue {Mark = ValueMark {Type = elementType; Property = prop; Path = toPath containerPath prop}; Value = value}) 
        let rec getElementOption (reader: BsonDataReader) (containerPath: string) (prop: string) =
                if reader.Read() |> not
                then None
                else
                    match reader.TokenType with
                    | JsonToken.StartObject -> let newPath = toPath containerPath prop
                                               Some (MarkedComposite { Mark = ObjectMark { Property = prop; Path = newPath }; Components = seq {
                                                            let mutable elementOption = getElementOption reader newPath String.Empty
                                                            while elementOption |> Option.isSome do
                                                                yield elementOption.Value
                                                                elementOption <- (getElementOption reader newPath String.Empty)
                                                    }})
                    | JsonToken.StartConstructor -> let newPath = toPath containerPath prop
                                                    Some (MarkedComposite { Mark = ConstructorMark { Name = reader.Value.ToString(); Property = prop; Path = newPath }; Components = seq {
                                                            let mutable elementOption = getElementOption reader newPath String.Empty
                                                            while elementOption |> Option.isSome do
                                                                yield elementOption.Value
                                                                elementOption <- (getElementOption reader newPath String.Empty)
                                                    }})
                    | JsonToken.StartArray -> let newPath = toPath containerPath prop
                                              Some (MarkedComposite { Mark = ArrayMark {Property = prop; Path = newPath}; Components = seq {
                                                            let mutable i = 0;
                                                            let mutable elementOption = getElementOption reader newPath (sprintf "[%i]" i)
                                                            while elementOption |> Option.isSome do
                                                                yield elementOption.Value
                                                                i <- i + 1
                                                                elementOption <- (getElementOption reader newPath (sprintf "[%i]" i))
                                                    }})
                    | JsonToken.PropertyName -> getElementOption reader containerPath (reader.Value.ToString())
                    | JsonToken.EndObject | JsonToken.EndArray | JsonToken.EndConstructor -> None
                    | JsonToken.Comment -> getElementOption reader containerPath prop
                    | JsonToken.String -> toSomeValueToken BsonString containerPath prop reader.Value
                    | JsonToken.Integer -> toSomeValueToken BsonInteger containerPath prop reader.Value
                    | JsonToken.Float -> toSomeValueToken BsonFloat containerPath prop reader.Value
                    | JsonToken.Boolean -> toSomeValueToken BsonBoolean containerPath prop reader.Value
                    | JsonToken.Bytes -> toSomeValueToken BsonBytes containerPath prop reader.Value
                    | JsonToken.Date -> toSomeValueToken BsonDate containerPath prop reader.Value
                    | JsonToken.Raw -> toSomeValueToken BsonRaw containerPath prop reader.Value
                    | JsonToken.None | JsonToken.Null | JsonToken.Undefined -> toSomeValueToken BsonEmpty containerPath prop null
                    | _ -> failwith "Unknown BSON token type."

        MarkedComposite { Mark = DocumentMark; Components = seq{
            use reader = new BsonDataReader(inputStream)
            let mutable tokenOption = (getElementOption reader String.Empty String.Empty)
            while tokenOption |> Option.isSome do
                  yield tokenOption.Value
                  tokenOption <- (getElementOption reader String.Empty String.Empty)
        }}

    ///<summary>Writes the contents of a BSON composite to an output stream.</summary>
    ///<param name="outputStream">The output stream.</param>
    ///<param name="source">The input composite.</param>
    let writeToStream (outputStream: Stream)  (inputComposite: Composite<BsonElementMark, obj>) =
        use writer = new BsonDataWriter(outputStream)

        let rec writeElement element isProperty =
            match element with
            | MarkedComposite { Mark = DocumentMark; Components = c } ->
                for inner in c do
                    writeElement inner false
            | MarkedComposite { Mark = ObjectMark { Property = prop }; Components = c } -> 
                if (prop |> String.IsNullOrWhiteSpace |> not) && (isProperty) then writer.WritePropertyName prop
                writer.WriteStartObject()
                for inner in c do
                    writeElement inner true
                writer.WriteEndObject()
            | MarkedComposite { Mark = ArrayMark { Property = prop }; Components = c } -> 
                if (prop |> String.IsNullOrWhiteSpace |> not) && (isProperty) then writer.WritePropertyName prop
                writer.WriteStartArray()
                for inner in c do
                    writeElement inner false
                writer.WriteEndArray()
            | MarkedComposite { Mark = ConstructorMark { Name = name; Property = prop }; Components = c } -> 
                if (prop |> String.IsNullOrWhiteSpace |> not) && (isProperty) then writer.WritePropertyName prop
                writer.WriteStartConstructor(name)
                for inner in c do
                    writeElement inner true
                writer.WriteEndConstructor()
            | MarkedValue { Mark = ValueMark { Type = valueType; Property = prop }; Value = v } -> 
                if (prop |> String.IsNullOrWhiteSpace |> not) && (isProperty) then writer.WritePropertyName prop
                match valueType with
                | BsonRaw -> writer.WriteRawValue(v.ToString())
                | BsonEmpty -> writer.WriteNull()
                | BsonUnknown -> writer.WriteUndefined()
                | BsonString -> writer.WriteValue(v.ToString())
                | BsonInteger -> writer.WriteValue(unbox<int>v)
                | BsonFloat -> writer.WriteValue(unbox<float>v)
                | BsonDate -> writer.WriteValue(unbox<DateTime>v)
                | BsonBytes -> writer.WriteValue(unbox<byte[]> v)
                | BsonBoolean -> writer.WriteValue(unbox<bool>v)
            | _ -> failwith (sprintf "Unable to serialize the element %A" element)

        writeElement inputComposite false
        writer.Flush()