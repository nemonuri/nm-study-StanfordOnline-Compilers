module YCSCS1.Introduction.CompilerAndInterpreter.Compiler

open Nemonuri.Choice

//--- frame ---

include YCSCS1.Introduction.CompilerAndInterpreter.Interpreter

type compiler_t = | Compiler
type executable_t = | Executable

//---|

type program2_t = (choice2 program_t executable_t) // 뭔가 이름 붙이기 어려운 개념의 확장에 대해, 임시방편적인 이름과 Ad-Hoc Sum 타입을 도입.
type offline_t = { input: data_t; executable: executable_t }

let run_compiler
  (input: program_t)
  (compiler: compiler_t)
  : executable_t =
  Executable

let run_executable
  (input: data_t)
  (executable: executable_t)
  : output_t =
  Output
