module Services.Bank.Types 

open System
open Libs.MyMoney


type AccId = AccId of uint64

type TranId = TranId of uint64

type AccLabel = AccLabel of string

type RangeLow = RangeLow of Amount option

type RangeHigh = RangeHigh of Amount option


type AccUpd = 
    | DebitAndCredit
    | Ignore 

type MetadataValue = 
    | Int of int 
    | String of string
    | Amount of Amount



type TranCreation = {
    Debit: AccId;
    Credit: AccId; 
    Amount: Amount;
    SearchTags: Map<string, string>;
    Metadata: Map<string, MetadataValue>
}

type AccCreation = {
    Currency: Currency;
    Label: AccLabel;
    Upd: AccUpd
}

type Acc = {
    Id: AccId;
    Balance: Amount;
    Label: AccLabel;
    Upd: AccUpd
}

type Tran = {
    Id: TranId;
    Debit: AccId;
    Credit: AccId; 
    Amount: Amount;
    Timestamp: DateTime;
    SearchTags: Map<string, string>;
    Metadata: Map<string, MetadataValue>
}

