module


public section public_s
@[expose] section expose_s


namespace Nemonuri.Study.StanfordOnline.Compilers

universe u --u1 u2
set_option autoImplicit false


namespace Program


namespace Runtime

structure TypeContext : Type (u+1) where
  TProgram : Type u
  TData : Type u
  TOutput : Type u

inductive Label : Type 0 where
  | load
  | takeInput
  | produceOutput
  deriving DecidableEq

namespace Label

instance instLawfulBEq : LawfulBEq Label := inferInstance

abbrev Literal (l: Label) : Type := { x: Label // x = l }

instance Literal.instSubsingleton {l} : Subsingleton (Literal l) where
  allEq := by simp


-- Note: 대체 이건 무슨 패턴이지...?
-- https://github.com/leanprover/lean4/blob/v4.30.0-rc2/src/Init/Control/State.lean

def LabelT.{u1, u2}
  (α: Label → Type u1) (m: Label → Type u1 → Type u2) (β: Label → Type u1)
  : Type (max u1 u2) :=
  (l: Label) → (α l) → (m l (β l))

def LabelT.mk.{u1, u2}
  {α: Label → Type u1} {m: Label → Type u1 → Type u2} {β: Label → Type u1}
  (x: (l: Label) → (α l) → (m l (β l)))
  : LabelT α m β :=
  x

def LabelM α β := LabelT α (Function.const Label Id) β


namespace LabelT

--def bindLabel (l: Label) (α m β) (labelT: LabelT α m β) := labelT l

--def bindLabelAndFlip α m β l labelT := bindLabel l α m β labelT


end LabelT

--def LabelM.{u1, u2} (α: Type u1) : Type (max u1 (u2 + 1)) := Label → α → Type u2

--def LabelM.mk.{u1, u2} {α: Type u1} (x: Label → α → Type u2) : LabelM α := x

/-
def LabelT (m: Label → Type u) : Type u := (l: Label) → (m l)

def LabelT.mk {m: Label → Type u} (x: (l: Label) → (m l)) : LabelT m := x

instance instLabelT : LabelT (Function.const Label Label) := id
-/

namespace LabelT


protected def pure {α: Type u} (a: α) : LabelT (Function.const Label α) := Function.const Label a

/-
instance : Pure (λ (α: Type u) ↦ LabelT (Function.const Label α)) where
  pure := LabelT.pure


class Lift.{u1, u2} (m1: Label → Type u1) (m2: Label → Type u2) where
  lift (l: Label) : (m1 l) → (m2 l)
-/







end LabelT




--class LabelM.{u1, u2} (m1: Label → Type u1) (m2: Label → Type u2) where
--  labelM {l: Label} : (m1 l) → (m2 l)







--def CtorT {lm tm} (lt: LabelT lm tm) := (l: Label) → (lm l) → (lt l)



def Motive : Sort (u+1) := Label → Sort u

namespace Motive

instance instInhabited : Inhabited Motive where
  default := Function.const Label (default: Sort u)

--abbrev const (T: Sort u) : Motive := Function.const Label T

structure LiftM (TMotive : Motive) where
  liftM (label: Label) : (TMotive label) → Sort u

namespace LiftM

instance instInhabited (TMotive: Motive) : Inhabited (LiftM TMotive) where
  default := { liftM label := Function.const _ (TMotive label) }

end LiftM

end Motive


class CtorM (TPre : Motive) (TLift: Motive.LiftM TPre) where
  ctorM (label: Label) (pre: TPre label) : (TLift.liftM label pre)


namespace CtorM

open Motive LiftM


instance instInhabited (TPre : Motive) : Inhabited (CtorM TPre (default)) where
  default := { ctorM _ := id }
  --toSpecM := default

--instance instInhabited (target: TTarget) [Subsingleton TTarget] : Inhabited (CtorM target) where
--  default := CtorM.mk (toSpec := (default: Spec target)) (Function.const Label id)

section match_pattern_s
variable {TPre : Motive} {TLift: LiftM TPre} [ctor: CtorM TPre TLift]

def load (pre: TPre .load) := ctor.ctorM .load pre
def takeInput (pre: TPre .takeInput) := ctor.ctorM .takeInput pre
def produceOutput (pre: TPre .produceOutput) := ctor.ctorM .produceOutput pre

end match_pattern_s

attribute [match_pattern, reducible] load takeInput produceOutput



--protected class Dep (target: TTarget) (TPre TPost : MotiveM target) where
--  ctorM (label: Label) : (TPre.motiveM label) → (TPost.motiveM label)

/-
instance Dep.instCtorM
  (target: TTarget)
  (TPre TPost : MotiveM target)
  (ctor: CtorM.Dep target TPre TPost) : CtorM target where
  TPre := { motiveM := TPre.motiveM }
  TPost := { motiveM := TPre.motiveM }
  ctorM label pre := inst.ctorM label pre
-/

/-
abbrev recM (TPre: Sort u) (inst: CtorM TPre) (t: Label) : inst.toMotiveM.motiveM t :=
  match t with
  | .load => inst.load
  | .takeInput => inst.takeInput
  | .produceOutput => inst.produceOutput

abbrev casesOnM (TPost: Motive) (t: Label) (inst: CtorM (TPost t)) : inst.toMotiveM.motiveM t :=
  match t with
  | .load => inst.load
  | .takeInput => inst.takeInput
  | .produceOutput => inst.produceOutput
-/

end CtorM

@[implicit_reducible]
def Motive.toDefaultCtorM (TMotive: Motive) : CtorM TMotive (default) := default


end Label

open Label


/--
info:
  Nemonuri.Study.StanfordOnline.Compilers.Program.Runtime.Label.rec.{u}
  {motive : Label → Sort u} (load : motive load)
  (takeInput : motive takeInput) (produceOutput : motive produceOutput) (t : Label) : motive t
-/
#guard_msgs (info, whitespace := lax) in
#check Label.rec


/--
info: Nemonuri.Study.StanfordOnline.Compilers.Program.Runtime.Label.casesOn.{u} {motive : Label → Sort u} (t : Label)
  (load : motive load) (takeInput : motive takeInput) (produceOutput : motive produceOutput) : motive t
-/
#guard_msgs (info, whitespace := lax) in
#check Label.casesOn


inductive State (tc: TypeContext) : Label → Type u where
  | load : tc.TProgram → State tc .load
  | takeInput : tc.TData → tc.TProgram → State tc .takeInput
  | produceOutput : tc.TOutput → tc.TProgram → State tc .produceOutput


--def Label.Motive.ofState (tc: TypeContext) : Label.Motive := State tc


--instance State.instStepM {tc} : StepM id (State tc) where
--  step _ := id
  --TMotivePost := State tc
/-
    match label with
    | .load => tc.TProgram
    | .takeInput => tc.TData × tc.TProgram
    | .produceOutput => tc.TOutput × tc.TProgram
-/
  --trace _ := id
/-
    match label, pre with
    | .load, prog => State.load prog
    | .takeInput, ⟨data, prog⟩ => State.takeInput data prog
    | .produceOutput, ⟨output, prog⟩ => State.produceOutput output prog
-/



inductive History (tc: TypeContext) : Label → Type u where
  | load :
      State tc .load →
      History tc .load
  | takeInput :
      (label: Label) →
      History tc label →
      State tc .takeInput →
      History tc .takeInput
  | produceOutput :
      History tc .takeInput →
      State tc .produceOutput →
      History tc .produceOutput


def State.ToHistory (tc: TypeContext) : Label.Motive.LiftM (State tc) where
  liftM label _ :=
    match label with
    | .load => History tc .load
    | .takeInput => (label: Label) → (History tc label) → (History tc .takeInput)
    | .produceOutput => (History tc .takeInput) → (History tc .produceOutput)


instance State.ToHistory.instCtorM {tc: TypeContext} : CtorM (State tc) (State.ToHistory tc) where
  ctorM label state :=
    match label with
    | .load => History.load state
    | .takeInput => fun label pre => History.takeInput label pre state
    | .produceOutput => fun pre => History.produceOutput pre state


namespace Step

protected class Load (tc: TypeContext) where
  load: tc.TProgram → State tc .load

protected class TakeInput (tc: TypeContext) where
  takeInput (label: Label) : (State tc label) → (State tc .takeInput)

protected class ProduceOutput (tc: TypeContext) where
  produceOutput :
    State tc .takeInput →
    State tc .produceOutput

end Step

end Runtime

open Runtime in
class Runtime (tc: TypeContext) extends
  toLoad: Step.Load tc,
  toTakeInput: Step.TakeInput tc,
  toProduceOutput: Step.ProduceOutput tc

namespace Runtime

open Label Motive
variable (tc: TypeContext)

structure Step [Runtime tc] where
  protected label: Label


namespace Step

structure Log [Runtime tc] where
  step: Step tc
  state: State tc step.label

protected abbrev Log.label [Runtime tc] (log: Log tc) := log.step.label

protected abbrev Log.LabelM [Runtime tc] (label: Label) := {log: Log tc // log.label = label}

end Step

open Step

def State.ToStepLog [Runtime tc] : LiftM  where
  liftM label _ :=
    match label with
    | .load => Step.Load tc
    | .takeInput => Step.TakeInput tc
    | .produceOutput => Step.ProduceOutput tc




instance ToSum.instCtorM {tc: TypeContext} : CtorM (const (Step tc)) (ToSum tc) where
  ctorM label step :=
    match label with
    | .load => step.toLoad
    | .takeInput => step.toTakeInput
    | .produceOutput => step.toProduceOutput





def ToLog (tc: TypeContext) [Step tc] : LiftM (const (Step tc)) where
  liftM label _ :=
    match label with
    | .load =>

--inductive Log (tc: TypeContext) [Step tc] : Label → Type u where
--  | load (prog: tc.TProgram) (toLoad: motiveOf inst) : Log tc .load



end Step




def Spec (motive: Label.Motive) : Sort (max 1 u) := (label: Label) → (motive label) → Prop

def Spec.mk
  {motive: Label.Motive}
  (self: (label: Label) → (motive label) → Prop)
  : Spec motive := self

end Runtime

open Runtime State in
class Runtime (tc: TypeContext) where
  req: Label.Req
  raw: Runtime.Raw tc req

namespace Runtime


abbrev Raw.toRuntime {tc req} (self: Runtime.Raw tc req) : Runtime tc := Runtime.mk req self

abbrev Step.Spec tc := Step tc → Prop

abbrev PlausibleState {tc} (spec: Step.Spec tc) := Subtype spec


namespace PlausibleStep

variable {tc: TypeContext} {spec: Step.Spec tc}

def load



end PlausibleStep





class HasState (tc: TypeContext) (T: Type u)
  extends HasSnapShot tc T where
  setState (before: T) (arg: SnapShot tc) : T--{ after: T // (snapShot after) = arg }
  setState_spec : ∀before arg, (setState before arg |> snapShot) = arg

/-
class Init (tc: TypeContext) where
  init {label: Label} :
    State tc label →
    tc.TProgram →
    State tc .init
  init_spec {label: Label} (pre: State tc label) : ∀prog, match init pre prog with | .init prog2 => prog = prog2
-/

/-
class TakeInput (tc: TypeContext) where
  takeInput_req {label: Label} (state: State tc label) : Prop
  takeInput {label: Label} :
    ({state: State tc label // takeInput_req state}) →
    tc.TData →
    State tc .takeInput
  takeInput_spec {label} (pre: Subtype (@takeInput_req label)) data :
    match takeInput pre data with
    | .takeInput pre2 data2 _ => pre ≍ pre2 ∧ data = data2
-/

--abbrev TakeInput.ReqState {tc} (self: TakeInput tc) label := Subtype (@self.takeInput_req label)

/-
class ProduceOutput (tc: TypeContext) where
  produceOutput :
    State tc .takeInput →
    State tc .produceOutput
  produceOutput_spec : ∀pre, match produceOutput pre with | .produceOutput pre2 _ _ => pre = pre2
-/

end Runtime

open Runtime State

class Runtime (tc: TypeContext)
  extends TakeInput tc, ProduceOutput tc

namespace Runtime
namespace Implementer

inductive Approach where
  | compiler
  | interpreter
  deriving DecidableEq

instance Approach.instLawfulBEq : LawfulBEq Approach := inferInstance

class HasApproach (T: Type u) where
  approach (target: T) : Approach

end Implementer

class Implementer (tc: TypeContext)
  extends Runtime tc where
  approach : Implementer.Approach

namespace Implementer

instance instHasApproach {tc: TypeContext} : HasApproach (Implementer tc) where
  approach target := target.approach

end Implementer

end Runtime


class Runnable (tc: TypeContext) (T : Type u)
  extends Runtime tc, HasState tc T where
  run (pre: T) (data: tc.TData) : T
  run_spec : ∀pre data, (snapShot (run pre data)).label = .produceOutput


namespace Runnable

variable (tc: TypeContext) {T : Type u} [inst: Runnable tc T]

@[reducible]
def label (target: T) : Label := (inst.snapShot target).label

@[reducible]
def state (target: T) : State tc (Runnable.label tc target) := (inst.snapShot target).state





--abbrev is_init (target: T) : Prop := inst.toHasState.toHasSnapShot.snapShot target |> (·.label = .init)

end Runnable


--def Runnable.reset (tc: TypeContext) (T : Type u) [Runnable tc T] (target: T) : T :=
--  Runnable.toHasSourceProgram.sourceProgram target
--  |>


end Program

class Program (TProgram: Type u) where
  TData: Type u
  TOutput: Type u
  runtime: Program.Runtime (.mk TData TOutput TProgram)


end Nemonuri.Study.StanfordOnline.Compilers


end expose_s
end public_s
