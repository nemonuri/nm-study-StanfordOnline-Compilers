module YCSCS1.Introduction.Semantic

type program_t = | Program
type data_t = | Data
type output_t = | Output
type interpreter_t = | Interpreter
type compiler_t = | Compiler
type executable_t = | Executable

// <snippet:interpreter>

assume val run_interpreter : interpreter_t -> program_t -> data_t -> output_t

noeq type online_runner_t = {
  run: program_t -> data_t -> output_t;
}

// </snippet:interpreter>


// <snippet:compiler>

assume val run_compiler : compiler_t -> program_t -> executable_t
assume val run_executable : executable_t -> data_t -> output_t

noeq type offline_runner_t = {
  compile: program_t -> executable_t;
  run: executable_t -> data_t -> output_t;
}

// </snippet:compiler>

