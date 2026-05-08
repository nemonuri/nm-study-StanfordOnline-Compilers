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

def Motive : Sort (u+1) := Label → Sort u

def MotiveM.{u1, u2} : Sort ((max u1 u2) + 1) := Label → Sort u1 → Sort u2


namespace MotiveM

def constId : MotiveM := Function.const Label Id

instance instInhabited : Inhabited MotiveM where
  default := constId

class LiftT.{u1, u2, u3} (m1: MotiveM.{u1, u2}) (m2: MotiveM.{u1, u3}) where
  lift {l: Label} {α: Sort u1} : (m1 l α) → (m2 l α)

end MotiveM



def LabelT.{u1, u2}
  (α: Motive.{u1}) (m: MotiveM.{u1, u2}) (β: Motive.{u1})
  : Sort (imax u1 u2) :=
  (l: Label) → (α l) → (m l (β l))

def LabelT.mk.{u1, u2}
  {α: Motive.{u1}} {m: MotiveM.{u1, u2}} {β: Motive.{u1}}
  (x: (l: Label) → (α l) → (m l (β l)))
  : LabelT α m β :=
  x

def LabelM α β := LabelT α (default) β


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


inductive StepT (tc: TypeContext) (m: MotiveM) : Label → Type _ where
  | load :
      (m .load tc.TProgram) → StepT tc m .load
  | takeInput :
      (m .takeInput tc.TData × tc.TProgram) → StepT tc m .takeInput
  | produceOutput :
      (m .produceOutput tc.TOutput × tc.TProgram) →
      StepT tc m .produceOutput


def State tc := StepT tc (Label.MotiveM.constId)

/--
info: State :
  TypeContext → Label → Type u
-/
#guard_msgs (info, whitespace := lax) in
#check State.{u}

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



inductive TraceT (tc: TypeContext) (m: MotiveM) : Label → Sort _ where
  | load :
      (StepT tc m .load) →
      TraceT tc m .load
  | takeInput :
      (label: Label) →
      (TraceT tc m label) →
      (StepT tc m .takeInput) →
      TraceT tc m .takeInput
  | produceOutput :
      (TraceT tc m produceOutput) →
      (StepT tc m .produceOutput) →
      TraceT tc m .produceOutput
/-
  | takeInput :
      (label: Label) →
      History tc label →
      State tc .takeInput →
      History tc .takeInput
  | produceOutput :
      History tc .takeInput →
      State tc .produceOutput →
      History tc .produceOutput
-/

/-
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
-/

namespace Step

protected class Load tc m where
  load: TraceT tc m .load
--load: tc.TProgram → State tc .load

protected class TakeInput tc m where
  takeInput: TraceT tc m .takeInput
  --takeInput (label: Label) : (State tc label) → (State tc .takeInput)

protected class ProduceOutput tc m where
  produceOutput: TraceT tc m .produceOutput
/-
  produceOutput :
    State tc .takeInput →
    State tc .produceOutput
-/

end Step

end Runtime

/-
open Runtime Label in
class Runtime (tc: TypeContext) (m: MotiveM) extends
  toLoad: Step.Load tc m,
  toTakeInput: Step.TakeInput tc m,
  toProduceOutput: Step.ProduceOutput tc m
-/

open Runtime Label in
def Runtime (tc: TypeContext) (m: MotiveM) : Type _ := (l: Label) → (TraceT tc m l)

namespace Runtime

open Label
variable (tc: TypeContext) (m: MotiveM)

def Spec (l: Label) : Type _ := (TraceT tc m l) → Prop


--def Spec (l: Label)


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
