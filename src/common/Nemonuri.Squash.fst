module Nemonuri.Squash

open FStar.Squash

let prove (p: prop) (x: squash p) : Pure unit (requires True) (ensures (fun _ -> p)) = ()

let prove_b (b: bool) (x: squash (b2t b)) = prove (b2t b) x

let prove_implies (pa: prop) (pc: prop) (x: squash (pa ==> pc)) = prove (pa ==> pc) x

let prove_step (p1: prop) (p2: prop) (x: squash (p1 ==> p2)) 
  : Pure unit (requires p1) (ensures (fun _ -> p2)) =
  ()

let prove_step_b (p1: bool) (p2: bool) (x: squash (p1 ==> p2))
  : Pure unit (requires p1) (ensures (fun _ -> p2)) =
  prove_step (b2t p1) (b2t p2) x


include FStar.Calc