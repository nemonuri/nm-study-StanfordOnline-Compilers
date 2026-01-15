module YCSCS1.Introduction.CompilerAndInterpreter.Interpreter

//--- frame ---

type program_t = | Program
type data_t = | Data
type interpreter_t = | Interpreter
type interpreter_input_t = { program: program_t; data: data_t }
type output_t = | Output
type online_t = { input: interpreter_input_t; interpreter: interpreter_t } // 범주가 명확하게 한정적이면, 'Predicate' 없이, 'Set' 으로만 나타내고 보자.

//---|

let run_program 
    (input: interpreter_input_t) 
    (interpreter: interpreter_t)
    : output_t =
    Output
