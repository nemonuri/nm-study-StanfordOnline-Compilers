module YCSCS1.Introduction.CompilerAndInterpreter.Interpreter

//--- frame ---

type program_t = | Program
type data_t = | Data
type interpreter_t = | Interpreter
type output_t = | Output

//---|

type interpreter_input_t = { program: program_t; data: data_t } // 단순한 Tuple 타입은, frame 으로 느껴지지 않는다...흐음
type online_t = { input: interpreter_input_t; interpreter: interpreter_t } // 범주가 명확하게 한정적이면, 'Predicate' 없이, 'Set' 으로만 나타내고 보자.

let run_program 
    (input: interpreter_input_t) 
    (interpreter: interpreter_t)
    : output_t =
    Output
