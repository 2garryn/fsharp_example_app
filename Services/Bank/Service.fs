module Services.Bank.Service

open System
open Npgsql.FSharp

open Services.Bank.Types

type Accounts(tx: Sql.SqlProps) = 
    member this.Create (cr: AccCreation) = Ok ()
    member this.Get (id: AccId) = Ok ()
    member this.UpdateRange (id: AccId) (low: RangeLow) (high: RangeHigh) = Ok ()


type Transactions(tx: Sql.SqlProps) = 
    member this.Create (cr: TranCreation) = Ok ()
    member this.Get (id: TranId) = Ok ()
    member this.List (id: AccId) (startTs: DateTime) (endTs: DateTime) = Ok () 

