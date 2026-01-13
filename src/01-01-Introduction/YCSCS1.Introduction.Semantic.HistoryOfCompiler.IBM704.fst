module YCSCS1.Introduction.Semantic.HistoryOfCompiler.IBM704

// <snippet:history-of-compiler-ibm704>

//--- frame ---

type product_t =
| Hardware
| Software

type time_t =
| Current
| FutureGoal

//---|

type production_cost_t = { product: product_t; time: time_t }

let is_expensive (pc: production_cost_t) : bool = 
  match pc.product, pc.time with
  | Software, FutureGoal -> false
  | _ -> true

let is_less_expensive_than (pc_l pc_r: production_cost_t) : bool =
  match (pc_l.product, pc_l.time), (pc_r.product, pc_r.time) with
  | (Hardware, Current), (Software, Current) -> true
  | (Software, FutureGoal), (Software, Current) -> true
  | _ -> false

// </snippet:history-of-compiler-ibm704>