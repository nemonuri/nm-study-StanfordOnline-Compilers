module YCSCS1.Introduction.Semantic.HistoryOfCompiler.SpeedCoding

// <snippet:history-of-compiler-ibm704>

//--- frame ---

type program_language_t =
| SpeedCoding
| Assembly

type program_kind_t =
| Interpreted
| Compiled

type time_t =
| Year1953
| Today

type program_t =
| SpeedCodingInterpreter
| Product: lang: program_language_t -> program_t

//---|

let get_program_kind (program: program_t) : program_kind_t =
  match program with
  | SpeedCodingInterpreter -> Compiled
  | Product lang ->
    match lang with
    | SpeedCoding -> Interpreted
    | Assembly -> Compiled

let is_less_productive_than (pk_l pk_r: program_kind_t) : bool =
  match pk_l, pk_r with
  | Compiled, Interpreted -> true
  | _ -> false

// </snippet:history-of-compiler-ibm704>