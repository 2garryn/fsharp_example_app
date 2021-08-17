module Services.Bank.Service

open System

open Services.Bank.Types
open Services.Bank.Errors
open Libs.MyMoney
open Services.Bank.Database



type TranCeBuilder() = 
  member x.Bind(m, f) = Result.bind f m    
  member x.Return(v) = Ok v
  member x.ReturnFrom m = m
  member x.Zero() = Ok ()

let tranCE = TranCeBuilder()



type Accounts(db: AccDb) = 
    member this.Create cr = 
        match db.Create cr with 
        | Ok accId -> 
            Ok { Id = accId;
            Balance = Amount.Zero(cr.Currency); 
            Label = cr.Label; 
            Upd = cr.Upd}
        | Error e -> Error <| this.MapErr e

    member this.Get id = 
        match db.Get id with 
        | Ok acc -> Ok acc
        | Error e -> Error <| this.MapErr e

    member private _.MapErr e = 
        match e with
        | AccDbError.AlreadyExist -> AccErr.AlreadyExist  
        | AccDbError.InternalError reason -> AccErr.InternalError reason  
        | AccDbError.NotExist -> AccErr.NotExist  


type Transactions(db: TranDb) = 
    member this.Create (cr: TranCreation) = 
        let mapErr = fun(e) -> 
            match e with 
            | InternalError s ->  TranErr.InternalError s
            | _ -> failwith "Invalid Result"
        tranCE {
            do! if cr.Debit = cr.Credit then Error CanNotTranToItself
                else Ok ()
            do! this.UpdateBalanceMaybe cr.Credit (fun (b) -> b - cr.Amount)
            do! this.UpdateBalanceMaybe cr.Debit (fun (b) -> b + cr.Amount)
            let! (tranId, timestamp) = db.Create cr |> Result.mapError mapErr
            return {
                Id = tranId;
                Debit = cr.Debit;
                Credit = cr.Credit;
                Amount = cr.Amount;
                Timestamp = timestamp;
                SearchTags = cr.SearchTags;
                Metadata = cr.Metadata
            }
        }
        
    member private _.UpdateBalanceMaybe accId f =
        let mapErr = fun(e) -> 
            match e with 
            | InternalError s ->  TranErr.InternalError s
            | NotExist ->  TranErr.AccNotExist accId
        tranCE {
           let! upd = db.GetUpd accId |> Result.mapError mapErr
           do! match upd with 
               | Ignore -> Ok ()
               | DebitAndCredit -> 
                    tranCE {
                        let! acc = db.GetAcc accId true |> Result.mapError mapErr 
                        return! 
                            match f(acc.Balance) with
                            | Ok (balance) -> db.UpdateBalance accId balance |> Result.mapError mapErr
                            | Error (NegResult(_)) -> Error <| NotEnoughMoney accId
                            | Error (DifferentCurrency(_)) -> Error <| CurrencyNotMatched accId
                            | _ -> failwith "Invalid Result"
                    }
        }


    member this.Get (id: TranId) = 
        let mapErr = fun(e: TranDbErr) -> 
            match e with 
            | InternalError s ->  TranErr.InternalError s
            | NotExist -> TranErr.TranNotExist id 
        tranCE { return! db.Get id |> Result.mapError mapErr }
         

