module YCSCS1.Introduction.HistoryOfCompiler.SpeedCoding

module Base = YCSCS1.Introduction.CompilerAndInterpreter.Compiler
open Nemonuri.Choice

/// <region:1>

//--- frame ---

type speedcoding_t = | SpeedCoding
type handwritten_t = | HandWritten

type interpreter_t =
| SpeedCodingInterpreter of speedcoding_t
| OtherInterpreter of Base.interpreter_t  // 이게 바로, 클래스 '상속'?

type program_develop_t = | Develop of (choice2 speedcoding_t handwritten_t)
type program_execution_t = | Execution of (choice2 speedcoding_t handwritten_t)
type today_program_execution_t = | TodayExecution of (choice2 interpreter_t Base.executable_t)

//---|

//--- private constants ---
private let c_SpeedCoding = 0
private let c_HandWritten = 1
//---|

private let _sorh_t = (choice2 speedcoding_t handwritten_t)

private let _is_any_slower_than 
  (#any_t: Type) (#sel_t: Type)
  (selector: any_t -> sel_t)
  (strict_comparer: sel_t -> sel_t -> bool)
  (any1 any2: any_t)
  : bool =
  let (sel1, sel2) = (selector any1, selector any2) in
  strict_comparer sel1 sel2

(*
private let _first_is_slower (sorh1 sorh2: _sorh_t) : bool =
  match encode_choice2 sorh1, encode_choice2 sorh2 with
  | c_HandWritten, c_SpeedCoding -> true
  | _ -> false

private let _speedcoding_is_slower sorh1 sorh2 : bool =
  not (_handwritten_is_slower sorh1 sorh2)
*)

let is_develop_slower_than (pd1 pd2: program_develop_t) : bool =
  _is_any_slower_than (fun (pd: program_develop_t) -> encode_choice2 pd._0) (>) pd1 pd2

let is_less_productive_than pd1 pd2 = is_develop_slower_than pd1 pd2

let is_execution_slower_than (pe1 pe2: program_execution_t) : bool =
  _is_any_slower_than (fun (pe: program_execution_t) -> encode_choice2 pe._0) (<) pe1 pe2

let is_today_execution_slower_than (pe1 pe2: today_program_execution_t) : bool =
  _is_any_slower_than (fun (pe: today_program_execution_t) -> encode_choice2 pe._0) (<) pe1 pe2

//--- test ---
private let c12 = Choice1Of2
private let c22 = Choice2Of2

let _ = assert ( Develop (c22 HandWritten) `is_develop_slower_than` Develop (c12 SpeedCoding) )
let _ = assert ( Develop (c22 HandWritten) `is_less_productive_than` Develop (c12 SpeedCoding) )
let _ = assert ( Execution (c12 SpeedCoding) `is_execution_slower_than` Execution (c22 HandWritten) )
let _ = assert ( TodayExecution (c12 (OtherInterpreter Base.Interpreter)) `is_today_execution_slower_than` TodayExecution (c22 Base.Executable) )
//---|

/// </region:1>

/// <region:2>
/// </region:2>

/// <region:3>
/// </region:3>