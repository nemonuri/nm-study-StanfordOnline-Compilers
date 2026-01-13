module YCSCS1.Introduction.Semantic.HistoryOfCompiler

// <snippet:history-of-compiler>

type frame_t =
| Product: product_t -> frame_t
| Time: time_t -> frame_t

and product_t =
| Hardware
| Software

and time_t =
| Current
| FutureGoal

type production_cost_t = { product: product_t; time: time_t }

let is_expensive (pc: production_cost_t) : bool = 
  match pc.product, pc.time with
  | Software, FutureGoal -> false
  | _ -> true

let is_more_expensive (pc_l pc_r: production_cost_t) : bool =
  match (pc_l.product, pc_l.time), (pc_r.product, pc_r.time) with
  | (Hardware, Current), (Software, Current) -> true
  | (Software, FutureGoal), (Software, Current) -> true
  | _ -> false

// </snippet:history-of-compiler>