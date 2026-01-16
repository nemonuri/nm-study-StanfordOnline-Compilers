module YCSCS1.Introduction.HistoryOfCompiler.SpeedCoding

//--- frame ---

type program_language_t =
| SpeedCoding
| HandwrittenAssembly

type program_kind_t =
| InterpreterRequired
| Compiled

type time_t =
| Year1953
| Today

type program_t =
| SpeedCodingInterpreter
| IBM704Program: (lang: program_language_t) -> program_t
| TodayProgram: (kind: program_kind_t) -> program_t

type program_size_t =
| Byte300

//---|

let get_program_kind (program: program_t) : program_kind_t =
  match program with
  | SpeedCodingInterpreter -> Compiled
  | IBM704Program lang ->
  begin
    match lang with
    | SpeedCoding -> InterpreterRequired
    | HandwrittenAssembly -> Compiled
  end
  | TodayProgram kind -> kind

let get_time (program: program_t) : time_t =
  match program with
  | SpeedCodingInterpreter
  | IBM704Program _ -> Year1953
  | TodayProgram _ -> Today

private let is_less_productive_than_core (pk_l pk_r: program_kind_t) : bool =
  match pk_l, pk_r with
  | Compiled, InterpreterRequired -> true
  | _ -> false

let is_less_productive_than (program_l program_r: program_t) : 
  Pure bool (requires (get_time program_l) = (get_time program_r)) (ensures fun _ -> True)
  =
  is_less_productive_than_core (get_program_kind program_l) (get_program_kind program_r)

private let is_slower_than_core (pk_l pk_r: program_kind_t) : bool =
  match pk_l, pk_r with
  | InterpreterRequired, Compiled -> true
  | _ -> false

let is_slower_than (program_l program_r: program_t) : 
  Pure bool (requires (get_time program_l) = (get_time program_r)) (ensures fun _ -> True)
  =
  is_slower_than_core (get_program_kind program_l) (get_program_kind program_r)

let get_program_size (program: program_t{SpeedCodingInterpreter? program}) : program_size_t =
  match program with
  | SpeedCodingInterpreter -> Byte300

let is_size_trouble (size: program_size_t) (time: time_t) : bool =
  match size, time with
  | Byte300, Year1953 -> true
  | Byte300, Today -> false

let is_well_known_language 
  (lang: program_language_t{SpeedCoding? lang}) 
  (time: time_t{Today? time})
  : bool =
  match lang, time with
  | SpeedCoding, Today -> false