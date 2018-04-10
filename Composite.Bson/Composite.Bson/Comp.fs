namespace Composite.Bson

open FSharp.Core
open System.IO
open Newtonsoft.Json
open Newtonsoft.Json.Bson
open Composite

type KeyValuePair = { Key: string; Value: obj }

[<RequireQualifiedAccess>]
module Comp =
    let ofBson (bson: byte[]) =
        let rec getElement (reader: BsonDataReader) (propertyName: string Option) =
                if reader.Read() |> not
                then None
                else
                    match reader.TokenType with
                    | JsonToken.StartObject -> Some (Composite (seq {
                                                            let mutable elementOption = getElement reader propertyName 
                                                            while elementOption |> Option.isSome do
                                                                yield elementOption.Value
                                                                elementOption <- (getElement reader propertyName)
                                                    }))
                    | JsonToken.StartArray -> Some (Composite (seq {
                                                            let mutable i = 0;
                                                            let mutable elementOption = getElement reader (Some (if propertyName.IsSome then sprintf "%s[%i]" propertyName.Value i else  sprintf "[%i]" i)) 
                                                            while elementOption |> Option.isSome do
                                                                yield elementOption.Value
                                                                i <- i + 1
                                                                elementOption <- (getElement reader (Some (if propertyName.IsSome then sprintf "%s[%i]" propertyName.Value i else  sprintf "[%i]" i)))
                                                    }))
                    | JsonToken.PropertyName -> let extractedPropertyName = reader.Value.ToString()
                                                let newPropertyName = match propertyName with
                                                                      | Some name -> (name + "." + extractedPropertyName)
                                                                      | None -> extractedPropertyName
                                                getElement reader (newPropertyName|>Some)
                    | JsonToken.EndObject | JsonToken.EndArray -> None
                    | _ -> match propertyName with
                           | Some name -> Some (Value {Key=name; Value=reader.Value})
                           | None ->  Some (Value {Key="_"; Value=reader.Value})

        let getMainElement () =
            Composite (seq {
                use stream = new MemoryStream(bson)
                use reader = new BsonDataReader(stream)
                reader.Read() |> ignore

                let mutable elementOption = (getElement reader None)
                while elementOption |> Option.isSome do
                      yield elementOption.Value
                      elementOption <- (getElement reader None)
                
            })

        getMainElement ()


    // let toBson (source: KeyValuePair Composite) =
    //     let mutable buffer : byte[] = Array.zeroCreate 4000
    //     use stream = new MemoryStream(buffer, true)
    //     use writer = new BsonDataWriter(stream)

    //     let rec writeElement (pendingStartTokens: int) (pendingEndWritingFuncs: (writer -> unit)[]) (composite: KeyValuePair Composite) =
    //         match composite with
    //         | Composite kvp -> 

    //         writer.WriteStartObject()
    //     ()


