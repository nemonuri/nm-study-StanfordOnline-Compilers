module YCSCS1.Introduction.HistoryOfCompiler.SpeedCoding

module Base = YCSCS1.Introduction.CompilerAndInterpreter.Compiler
open Nemonuri.Choice

//--- frame ---

type speedcoding_t = | SpeedCoding
type handwritten_t = | HandWritten

type interpreter_t =
| SpeedCodingInterpreter of speedcoding_t
| OtherInterpreter of Base.interpreter_t  // 이게 바로, 클래스 '상속'?

type program_develop_t = | Develop of (choice2 speedcoding_t handwritten_t)
type program_execution_t = | Execution of (choice2 speedcoding_t handwritten_t)
type today_program_execution_t = | TodayExecution of (choice2 speedcoding_t handwritten_t)

//---|

//--- private constants ---
private let c_SpeedCoding = 0
private let c_HandWritten = 1
//---|

let is_develop_slower_than (pd1 pd2: program_develop_t) : bool =
  match (encode_choice2 pd1._0), (encode_choice2 pd2._0) with
  | c_HandWritten, c_SpeedCoding -> true
  | _ -> false

let is_less_productive_than pd1 pd2 = is_develop_slower_than pd1 pd2

let is_execution_slower_than (pe1 pe2: program_execution_t) : bool =
  match (encode_choice2 pe1._0), (encode_choice2 pe2._0) with
  | c_HandWritten, c_SpeedCoding -> true
  | _ -> false

let is_today_execution_slower_than (pe1 pe2: today_program_execution_t) : bool =
  match (encode_choice2 pe1._0), (encode_choice2 pe2._0) with
  | c_HandWritten, c_SpeedCoding -> true
  | _ -> false

