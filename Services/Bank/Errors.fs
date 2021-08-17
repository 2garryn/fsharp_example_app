module Services.Bank.Errors 

open System
open Services.Bank.Types
type AccErr = 
    | AlreadyExist 
    | InternalError of String
    | NotExist 

type TranErr = 
    | CanNotTranToItself
    | AccNotExist of AccId
    | TranNotExist of TranId
    | CurrencyNotMatched of AccId
    | NotEnoughMoney of AccId
    | InternalError of String

