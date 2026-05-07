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

-- Note: 대체 이건 무슨 패턴이지...?
-- https://github.com/leanprover/lean4/blob/v4.30.0-rc2/src/Init/Control/State.lean

def Motive : Sort (u+1) := Label → Sort u

instance Motive.instInhabited : Inhabited Motive where
  default := Function.const Label (default: Sort u)


--def Motive.pulift.{u1, u2} (m1: Motive.{u1}) : Motive.{max u1 u2} :=

--variable {TTarget: Sort u}

class MotiveM (TTarget: Sort u) : Sort (u+1) where
  motiveM : Motive.{u}

namespace MotiveM


instance instInhabited (TTarget: Sort u) : Inhabited (MotiveM TTarget) where
  default := { motiveM := Function.const Label TTarget }



class SpecM (TPre : Motive) : Sort (u+1) where --M target --(target: TTarget)
  TPost (label: Label) : MotiveM (TPre label) --M target

instance SpecM.instInhabited (TPre : Motive) : Inhabited (SpecM TPre) where
  default := { TPost := fun label => (default : MotiveM (TPre label)) }

#print SpecM.instInhabited

--instance Spec.instInhabited (target: TTarget) [Subsingleton TTarget] : Inhabited (Spec target) where
--  default := { TPre := default, TPost := default }

end MotiveM


class CtorM (TPre : Motive)
  extends
    toSpecM: MotiveM.SpecM TPre
  where
  ctorM (label: Label) : (TPre label) → (toSpecM.TPost label).motiveM label


namespace CtorM

open MotiveM


instance instInhabited (TPre : Motive) : Inhabited (CtorM TPre) where
  default := {
    toSpecM := default
    ctorM _ pre := pre
  }
  --toSpecM := default

--instance instInhabited (target: TTarget) [Subsingleton TTarget] : Inhabited (CtorM target) where
--  default := CtorM.mk (toSpec := (default: Spec target)) (Function.const Label id)

section match_pattern_s
variable (TPre : Motive)

def load (ctor: CtorM TPre) := ctor.ctorM .load
def takeInput (ctor: CtorM TPre) := ctor.ctorM .takeInput
def produceOutput (ctor: CtorM TPre) := ctor.ctorM .produceOutput

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
def Motive.toDefaultCtorM (TMotive: Motive) : CtorM TMotive := default

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



inductive History.Raw (tc: TypeContext) : Label → Type u where
  | load :
      State tc .load →
      History.Raw tc .load
  | takeInput :
      (label: Label) →
      History.Raw tc label →
      State tc .takeInput →
      History.Raw tc .takeInput
  | produceOutput :
      History.Raw tc .takeInput →
      State tc .produceOutput →
      History.Raw tc .produceOutput


structure History (tc: TypeContext) where
  label: Label
  raw: History.Raw tc label



namespace History

abbrev Raw.toHistory {tc label} (raw: History.Raw tc label) : History tc := .mk label raw

instance instMotiveOf {tc} : Label.MotiveOf (History tc) where
  motiveOf label := History.Raw tc label


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

class Step (tc: TypeContext) extends
  toLoad: Step.Load tc,
  toTakeInput: Step.TakeInput tc,
  toProduceOutput: Step.ProduceOutput tc


namespace Step

open Label

instance instMotiveOf {tc} : MotiveOf (Step tc) where
  motiveOf label :=
    match label with
    | .load => Step.Load tc
    | .takeInput => Step.TakeInput tc
    | .produceOutput => Step.ProduceOutput tc

#print instMotiveOf

/-
inductive State (tc: TypeContext) : Label → Type u where
  | load : tc.TProgram → State tc .load
  | takeInput : tc.TData → tc.TProgram → State tc .takeInput
  | produceOutput : tc.TOutput → tc.TProgram → State tc .produceOutput
-/

inductive Log (tc: TypeContext) [Step tc] : Label → Type u where
  | load (prog: tc.TProgram) (toLoad: motiveOf inst) : Log tc .load



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
