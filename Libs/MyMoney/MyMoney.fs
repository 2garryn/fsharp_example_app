
module Libs.MyMoney

open System

open Microsoft.FSharp.Core.Operators.Checked

type AmountErr = 
    | InvalidAmount of string
    | InvalidCurrency of string
    | SumOverloading of decimal * decimal * string
    | NegResult of decimal * decimal * string
    | DifferentCurrency of decimal * string * decimal * string



type Currency = 
    | COP
    | USD
    | MXN
    static member Parse (s: string) c = 
        match s with 
            | "COP" -> Ok COP
            | "MXN" -> Ok MXN
            | "USD" -> Ok USD
            | _ -> Error <| InvalidCurrency s
    override this.ToString() =
        match this with 
            | COP -> "COP"
            | MXN -> "MXN"
            | USD -> "USD"

type MoneyCalcBuilder() = 
  member x.Bind(m, f) = Result.bind f m    
  member x.Return(v) = Ok v
  member this.ReturnFrom m = m

let moneyExpr = MoneyCalcBuilder()


type Amount = 
    {Am : decimal; Cur : Currency} 
    static member (+) (a1, a2) = 
        Amount.CheckCurrency a1 a2 <| fun() -> 
            try Ok {Am = Checked.(+) a1.Am a2.Am; Cur = a1.Cur}
            with _ -> Error <| SumOverloading (a1.Am, a2.Am, a1.Cur.ToString())

    static member (-) (a1, a2) = 
        Amount.CheckCurrency a1 a2 <| fun() -> 
            Ok {Am = a1.Am - a2.Am; Cur = a1.Cur}


    static member CheckCurrency (a1: Amount) (a2: Amount) f = 
        if a1.Cur = a2.Cur then f()
        else Error <| DifferentCurrency (a1.Am, a1.Cur.ToString(), a2.Am, a2.Cur.ToString())

    static member Parse (s: string) c = 
        match Decimal.TryParse(s) with
        | true, v -> Ok {Am = v; Cur = c}
        | false, _ -> Error <| InvalidAmount s

