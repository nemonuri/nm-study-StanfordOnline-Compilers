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

// <snippet:history-of-compiler>

type manufacture_t =
| Hardware
| Sortware

type time_t =
| Present
| Future

type cost_t =
| Cost: (time: time_t) -> (manufacture: manufacture_t) -> cost_t

let is_expensive (cost: cost_t) : option bool =
  let Cost t m = cost in
  match t, m with
  | Present, Hardware -> Some true
  | Present, Sortware -> Some true
  | Future, Hardware -> None
  | Future, Sortware -> Some false



//let is_current_state (manufacture: manufacture_t) (cost: cost_t) : bool =







// </snippet:history-of-compiler>