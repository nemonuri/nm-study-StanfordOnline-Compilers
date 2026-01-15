module Nemonuri.Choice

// Reference: https://github.com/dotnet/fsharp/blob/170ee19820227897681a9fac6ae6f1190574bff7/src/FSharp.Core/prim-types.fsi#L1970

type choice2 (t1 t2: Type) =
| Choice1Of2 of t1
| Choice2Of2 of t2

type choice3 (t1 t2 t3: Type) =
| Choice1Of3 of t1
| Choice2Of3 of t2
| Choice3Of3 of t3
