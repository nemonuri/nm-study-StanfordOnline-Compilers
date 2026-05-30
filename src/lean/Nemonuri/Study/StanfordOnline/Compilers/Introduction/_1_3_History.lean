module

public import Nemonuri.Study.StanfordOnline.Compilers.Introduction._1_2_Compiler
public import Mathlib.Order.Bounds.Basic
public import Cslib.Foundations.Semantics.LTS.Bisimulation

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

section Machine

structure Machine where
  beginIn: Std.Time.Year.Offset
  called: String
  builtBy: String
  commerciallySuccessful: Bool

variable (TM: Type u) [LE TM] [Std.IsPartialOrder TM]

class HasMachine where
  protected v: TM → Machine

variable [HasMachine TM]

instance (m: TM) : CoeDep TM m Machine where
  coe := HasMachine.v m


@[mk_iff]
structure IsTheMachine (m: TM) : Prop where
  beginIn: Machine.beginIn m / 10 = 195
  called: Machine.called m = "704"
  builtBy: Machine.builtBy m = "IBM"
  firstCommerciallySuccessful: IsLeast { x: TM | Machine.commerciallySuccessful x } m
  someEarierMachines: { x: TM | x < m }.Nontrivial

def TheMachine := { x: TM // IsTheMachine _ x }

def theMachine [ctx: Inhabited (TheMachine TM)] := ctx.default

--@[reducible]
--def theMachine (tm: TM) (req: IsTheMachine _ tm) := LeastOn.mk tm req.firstCommerciallySuccessful |>.toUnique

end Machine

/-!
3. they found that the software costs exceeded the hardware costs.
4. And not just by a little bit, but by a lot.
5. This is important because these, the hardware in these, those days was extremely expensive
6. And even then when hardware cost the most in absolute and relative terms, more than they would ever cost again, already the software was the dominant expense in, in making good use out of computers.
-/

structure CostContext where
  softwareCosts: Set Nat
  hardwareCosts: Set Nat

namespace CostContext

structure ThoseDays.Context where
  littleBit: Nat
  aLot: Nat
  extremelyExpensiveCost: Nat


@[mk_iff]
structure ThoseDays [ctx: Inhabited (ThoseDays.Context)] (c: CostContext) : Prop where
  exceeded: ∀sc ∈ c.softwareCosts, ∀hc ∈ c.hardwareCosts, hc < sc
  aLot_spec: ctx.default.aLot > ctx.default.littleBit
  by_a_lot: ∀sc ∈ c.softwareCosts, ∀hc ∈ c.hardwareCosts, ctx.default.aLot < sc - hc
  extremelyExpensive: ∀hc ∈ c.hardwareCosts, ctx.default.extremelyExpensiveCost < hc



end CostContext


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

variable [Inhabited (CostContext.ThoseDays.Context)]

structure Effort where
  run (pre: CostContext) (req: CostContext.ThoseDays pre) : { post: CostContext // MoreProductiveProgramming pre post }
  name: String
  year: Std.Time.Year.Offset
  developer: String

variable (T: Type u) [LE T] [Std.IsPartialOrder T]

class HasEffort where
  protected v: T → Effort

variable [HasEffort T]

instance (x: T) : CoeDep T x Effort where
  coe := HasEffort.v x



@[mk_iff]
structure IsSpeedCoding (x: T) : Prop where
  the_earliest: IsLeast Set.univ x
  name: Effort.name x = "speed coding"
  year: Effort.year x = 1953
  developer: Effort.developer x = "John Backus"

/-
@[reducible]
def speedCoding (x: T) (req: IsSpeedCoding _ x) := LeastOn.mk x req.the_earliest |>.toUnique
-/

@[reducible]
def SpeedCoding := { x: T // IsSpeedCoding _ x }

def speedCoding [ctx: Inhabited (SpeedCoding T)] := ctx.default


/-!
2. Now, speed coding is what we call today, an early example of an interpreter.
-/
class HasLts (α St La: Type*) where
  protected v: α → Cslib.LTS St La

instance {α St La: Type*} [HasLts α St La] (x: α) : CoeDep α x (Cslib.LTS St La) where
  coe := HasLts.v x


namespace SpeedCoding

open Interpreter
variable (St: Type*) [Zero St] [State St] [Ability St] [HasLts (SpeedCoding T) St (Label St)]

@[mk_iff]
protected class IsInterpreter (s: SpeedCoding T) : Prop where
  intro: (toLTS St).IsBisimulation s Eq

variable [Inhabited (SpeedCoding T)] [SpeedCoding.IsInterpreter T St (speedCoding T)]

/-!
3. The primary advantage was that it was much faster, to develop the programs.
4. So the, in that sense, the programmer was much more productive.
5. But among its disadvantages, code written, speed code programs were ten to twenty times slower.
6. Then handwritten programs and that's also true of interpreted programs today.
-/




end SpeedCoding



end spec_s


end Nemonuri.Study.StanfordOnline.Compilers.Introduction.History


end public_s
