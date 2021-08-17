module Services.Bank.Database

open System
open Libs.MyMoney
open Services.Bank.Types


type AccDbError = 
    | AlreadyExist 
    | InternalError of String 
    | NotExist

type AccDb = {
    Create : AccCreation -> Result<AccId, AccDbError>;
    Get : AccId -> Result<Acc, AccDbError>
}

type TranDbErr = 
    | NotExist
    | InternalError of String

type TranDb = {
    GetAcc : AccId -> bool -> Result<Acc, TranDbErr>
    GetUpd : AccId -> Result<AccUpd, TranDbErr>
    UpdateBalance: AccId -> Amount -> Result<unit, TranDbErr>
    Create : TranCreation -> Result<TranId * DateTime, TranDbErr>
    Get : TranId -> Result<Tran, TranDbErr>
}