module


public section public_s
@[expose] section expose_s


namespace Nemonuri.Study.StanfordOnline.Compilers

universe u u1 u2
set_option autoImplicit false


namespace Program


namespace Runtime

structure TypeContext : Type (u+1) where
  TProgram : Type u
  TData : Type u
  TOutput : Type u

inductive Label where
  | load
  | takeInput
  | produceOutput
  deriving DecidableEq


namespace Label

instance instLawfulBEq : LawfulBEq Label := inferInstance

-- Note: 대체 이건 무슨 패턴이지...?
-- https://github.com/leanprover/lean4/blob/v4.30.0-rc2/src/Init/Control/State.lean

/-
protected def ToSort (toSort: Label → Sort u) : Sort u := (label: Label) → toSort label

protected def ToSort.mk {toSort: Label → Sort u} {label: Label} (x: toSort label) := x

protected def OfSort (_: Sort u) : Type := Label

protected def OfSort.mk {_: Sort u} (x: Label) := x
-/

def Motive : Type u := Label → Sort u

def Motive.mk (self: Label → Sort u) : Motive := self


class Motive.LiftM (TPre: semiOutParam Motive.{u}) (TPost: Motive.{u}) where
  liftM (label) : TPre label → TPost label


end Label

/-
class OfLabelM (T: Type u1) (m: Type u1 → Type u2) where
  ofLabel: Label → m T

class ToLabelM (T: Type u1) (m: Type u1 → Type u2) where
  toLabel: m T → Label

abbrev OfLabel T := OfLabelM T Id

abbrev ToLabel T := ToLabelM T Id
-/

/-
open Label Motive in
class StepM TPre TPost [inst: LiftM.{u} TPre TPost] where
  step (label) : (TPre label) → (LiftM.liftM label (TPre label))
-/


--class LogM (m1 m2: (label: Label) → Sort u) where
--  log {}


inductive State (tc: TypeContext) : Label → Type u where
  | load : tc.TProgram → State tc .load
  | takeInput : tc.TData → tc.TProgram → State tc .takeInput
  | produceOutput : tc.TOutput → tc.TProgram → State tc .produceOutput

def Label.Motive.ofState (tc: TypeContext) : Label.Motive := State tc

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



inductive Trace (tc: TypeContext) : Label → Type u where
  | load :
      State tc .load →
      Trace tc .load
  | takeInput :
      (label: Label) →
      Trace tc label →
      State tc .takeInput →
      Trace tc .takeInput
  | produceOutput :
      Trace tc .takeInput →
      State tc .produceOutput →
      Trace tc .produceOutput

def Label.Motive.ofTrace (tc: TypeContext) : Label.Motive := Trace tc

instance Trace.instTraceM {tc} : TraceM (Trace tc) where
  TMotivePre label :=
    match label with
    | .load => State tc .load
    | .takeInput => (label: Label) × (Trace tc label) × (State tc .takeInput)
    | .produceOutput =>



--inductive StateM (tc: TypeContext) (m: (label: State.Label) → State tc label → (Type u))

namespace History

protected inductive Raw (tc: TypeContext) : State.Label → Type u where
  | load :
      State tc .load →
      History.Raw tc .load
  | takeInput :
      (label: State.Label) →
      History.Raw tc label →
      State tc .takeInput →
      History.Raw tc .takeInput
  | produceOutput :
      History.Raw tc .takeInput →
      State tc .produceOutput →
      History.Raw tc .produceOutput

end History

structure History (tc: TypeContext) where
  label: State.Label
  raw: History.Raw tc label

abbrev History.Raw.toHistory {tc label} (raw: History.Raw tc label) : History tc := .mk label raw


namespace History

open Raw

variable {tc: TypeContext}

@[match_pattern]
def load (prog: tc.TProgram) : History tc :=
  State.load prog |> Raw.load |> toHistory

@[match_pattern]
def takeInput (label) (pre: History.Raw tc label) (data: tc.TData) (prog: tc.TProgram) : History tc :=
  State.takeInput data prog |> Raw.takeInput label pre |> toHistory

@[match_pattern]
def produceOutput (pre: History.Raw tc .takeInput) (output: tc.TOutput) (prog: tc.TProgram) : History tc :=
  State.produceOutput output prog |> Raw.produceOutput pre |> toHistory

@[simp, spec]
theorem load_label_eq (prog: tc.TProgram) : (load prog).label = .load := by
  unfold load
  simp

@[simp, spec]
theorem takeInput_label_eq label pre (data: tc.TData) (prog: tc.TProgram)
  : (takeInput label pre data prog).label = .takeInput := by
  unfold takeInput
  simp

@[simp, spec]
theorem produceOutput_label_eq pre (output: tc.TOutput) (prog: tc.TProgram)
  : (produceOutput pre output prog).label = .produceOutput := by
  unfold produceOutput
  simp

end History


/-
def toProgram {tc label} (state: State tc label) : tc.TProgram :=
  match state with
  | .load prog => prog
  | .takeInput _ _ prog => prog
  | .produceOutput _ _ prog => prog
-/

namespace State


structure SnapShot (tc: TypeContext) where
  label : State.Label
  state : State tc label

abbrev toSnapShot {tc: TypeContext} {label: Label} (state: State tc label) := SnapShot.mk label state

class HasSnapShot (tc: TypeContext) (T: Type u) where
  snapShot (target: T) : SnapShot tc

end State

open State

namespace Step

class Load (tc: TypeContext) where
  load: tc.TProgram → State tc .load

class TakeInput (tc: TypeContext) (req: Label.Req) where
  takeInput (label: Label) : (req label) → (State tc label) → (State tc .takeInput)

class ProduceOutput (tc: TypeContext) where
  produceOutput :
    State tc .takeInput →
    State tc .produceOutput

end Step


protected class abbrev Raw (tc: TypeContext) (req: Label.Req) :=
  Step.Load tc, Step.TakeInput tc req, Step.ProduceOutput tc

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
