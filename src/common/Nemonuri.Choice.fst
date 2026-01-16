module Nemonuri.Choice
module I = FStar.IntegerIntervals

// Reference: https://github.com/dotnet/fsharp/blob/170ee19820227897681a9fac6ae6f1190574bff7/src/FSharp.Core/prim-types.fsi#L1970

type choice2 (t1 t2: Type) =
| Choice1Of2 of t1
| Choice2Of2 of t2

let encode_choice2 #t1 #t2 (item: choice2 t1 t2) 
  : I.under 2 =
  match item with
  | Choice1Of2 _ -> 0
  | Choice2Of2 _ -> 1
  

type choice3 (t1 t2 t3: Type) =
| Choice1Of3 of t1
| Choice2Of3 of t2
| Choice3Of3 of t3
