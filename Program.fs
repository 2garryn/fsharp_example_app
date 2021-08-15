// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open Libs.MyMoney

// Define a function to construct a message to print
let from whom =
    sprintf "from %s" whom


let isPos a = 
    if a > 0 then Some a
    else None

let isEmpty s = 
    if s = "" then None
    else Some s

let logerr e = printfn "Error: %A" e
let loginfo a = printfn "Info: %A" a

type MaybeBuilder() =
    member this.Bind(m, f) =
        match m with
        | None -> None
        | Some a -> f a
    member this.Return(x) =
        Some x
    member this.ReturnFrom m = m

let maybe = MaybeBuilder()

let checkPos a b c = 
    maybe {
        let! _ = isPos a 
        let! _ = isPos b 
        return! isPos c
    }

let checkEmpty a b c =
    maybe {
        let! _ = isEmpty a
        let! _ = isEmpty b
        return! isEmpty c 
    }



[<EntryPoint>]
let main argv =
    let r = 
        moneyExpr {
            let c = COP
            let! a1 = Amount.Parse "10.00" c
            let! a2 = Amount.Parse "20.00" c
            let! a3 = Amount.Parse "5.00" c
            let! a4 = a1 + a2
            return! a4 - a3
        }

    printfn "Hello world %A" r
    0 // return an integer exit code