module

public import Nemonuri.Study.StanfordOnline.Compilers.Introduction._1_2_Compiler
public import Mathlib.Order.Bounds.Basic

@[expose] public section public_s

namespace Nemonuri.Study.StanfordOnline.Compilers.Introduction.History

universe u
set_option autoImplicit false

/-!

#### 1.3. History

I think it's helpful to give a little bit of history about how compilers and interpreters were first developed.

1. So the story begins in the 1950s and in particular with a machine called the 704 built by IBM
2. This was their first commercially successful machine, although there had been some earlier machines that they had tried out

-/

section spec_s


instance : Div Std.Time.Year.Offset := inferInstanceAs (Div Int)

structure Machine where
  beginIn: Std.Time.Year.Offset
  called: String
  builtBy: String
  commerciallySuccessful: Bool

variable (TM: Type u) [LE TM] [Std.IsPartialOrder TM]

class HasMachine where
  getMachine: TM → Machine

instance [HasMachine TM] (m: TM) : CoeDep TM m Machine where
  coe := HasMachine.getMachine m


variable [HasMachine TM]

@[mk_iff]
structure IsTheMachine (m: TM) : Prop where
  beginIn: Machine.beginIn m / 10 = 195
  called: Machine.called m = "704"
  builtBy: Machine.builtBy m = "IBM"
  firstCommerciallySuccessful: IsLeast { x: TM | Machine.commerciallySuccessful x } m
  someEarierMachines: { x: TM | x < m }.Nontrivial

@[reducible]
def theMachine (tm: TM) (req: IsTheMachine _ tm) := LeastOn.mk tm req.firstCommerciallySuccessful


/-!
3. they found that the software costs exceeded the hardware costs.
4. And not just by a little bit, but by a lot.
5. This is important because these, the hardware in these, those days was extremely expensive
6. And even then when hardware cost the most in absolute and relative terms, more than they would ever cost again, already the software was the dominant expense in, in making good use out of computers.
-/

structure CostContext where
  softwareCosts: Set Nat
  hardwareCosts: Set Nat

structure CostContext.ThoseDays where
  littleBit: Nat
  aLot: Nat
  extremelyExpensiveCost: Nat

@[mk_iff]
structure CostContext.ThoseDays.IsLawful (c: CostContext) (td: CostContext.ThoseDays) : Prop where
  exceeded: ∀sc ∈ c.softwareCosts, ∀hc ∈ c.hardwareCosts, hc < sc
  aLot_spec: td.aLot > td.littleBit
  by_a_lot: ∀sc ∈ c.softwareCosts, ∀hc ∈ c.hardwareCosts, td.aLot < sc - hc
  extremelyExpensive: ∀hc ∈ c.hardwareCosts, td.extremelyExpensiveCost < hc


/-!
7. And this led a number of people to think about how they could do a better job of writing software.
8. How they could make programming more productive.
-/

def MoreProductiveProgramming (pre: CostContext) (post: CostContext) : Prop :=
  ∀preSc ∈ pre.softwareCosts, ∀postSc ∈ post.softwareCosts, preSc > postSc

/-!
##### 1.3.1. SpeedCoding

1. the earliest efforts to improve the productivity of programming was called speed coding, developed in 1953 by John Backus
-/





end spec_s


end Nemonuri.Study.StanfordOnline.Compilers.Introduction.History


end public_s
