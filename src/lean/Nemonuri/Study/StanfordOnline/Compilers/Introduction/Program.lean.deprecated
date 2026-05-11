module


public import Mathlib.Data.Fintype.Basic
public import Nemonuri.Study.StanfordOnline.Compilers.Introduction.Basic

public section public_s
@[expose] section expose_s


namespace Nemonuri.Study.StanfordOnline.Compilers

universe u --u1 u2
set_option autoImplicit false


/-
structure Program (TProgram: Type u) [InitialState TProgram] where
  program?: Option TProgram
  deriving DecidableEq
-/

namespace Runtime


protected structure Config where
  Cell: Type u
  cell_deq: DecidableEq Cell
  initialState: Finset Cell
  --filter? {α: Type u} (x: α) : Option ((p: Cell → Prop) × (DecidablePred p))

namespace Config

def Filter (cfg: Runtime.Config) : Type u := (p: cfg.Cell → Prop) × (DecidablePred p)

def Filter.mk (cfg: Runtime.Config) (p: cfg.Cell → Prop) [dec: DecidablePred p] : Filter cfg := ⟨p, dec⟩

class MaybeHasFilter (cfg: Runtime.Config) (α: Type u) where
  filter? (x: α) : Option cfg.Filter

protected def filter? (cfg: Runtime.Config) {α: Type u} := @MaybeHasFilter.filter? cfg α

instance (priority := low) {cfg: Runtime.Config} {α: Type u} : MaybeHasFilter cfg α where
  filter? _ := .none

abbrev TryRunAt (cfg: Runtime.Config) (α: Type u) [MaybeHasFilter cfg α] := TryRun α cfg.Filter

namespace MaybeHasFilter

instance (priority := high) {cfg: Runtime.Config} {α: Type u} [mhf: MaybeHasFilter cfg α] : TryRunAt cfg α where
  run? x := mhf.filter? x

end MaybeHasFilter


abbrev HasFilterReq (cfg: Runtime.Config) (α: Type u) [MaybeHasFilter cfg α] [TryRunAt cfg α] := HasReq α cfg.Filter

protected def filter
  (cfg: Runtime.Config) {α: Type u} [MaybeHasFilter cfg α] [TryRunAt cfg α] [hfr: HasFilterReq cfg α] (x: α) (req: hfr.Req x)
  : cfg.Filter := RunReq.run x cfg.Filter req



instance instDecidableEqFinsetCell (cfg: Runtime.Config) : DecidableEq (Finset cfg.Cell) := @Finset.decidableEq cfg.Cell cfg.cell_deq

end Config


structure Environment (cfg: Runtime.Config) where
  environment: Finset cfg.Cell
  deriving DecidableEq

#check instDecidableEqEnvironment.decEq

namespace Environment

open Config

instance instMaybeHasFilter {cfg: Runtime.Config} : MaybeHasFilter cfg (Environment cfg) where
  filter? _ := .some (Filter.mk cfg (fun _ => True))

instance instHasFilterReq {cfg: Runtime.Config} : HasFilterReq cfg (Environment cfg) where
  Req _ := True
  run?_not_none x h := by
    simp [TryRun.run?]


end Environment

def Config.initialEnvironment (cfg: Runtime.Config) : Environment cfg := cfg.initialState |> Environment.mk

protected class Program.Config (cfg: Runtime.Config) where
  protected filter : cfg.Filter

class Program (cfg: Runtime.Config) [pcfg: Program.Config cfg] (env: Environment cfg) where
  protected val : Finset cfg.Cell
  protected property :
    let ⟨fst, _⟩ := pcfg.filter
    (Finset.filter fst env.environment) = val

namespace Program





end Program


end Runtime



namespace Program




section program_1_s
variable {TProgram} [InitialState TProgram]

instance instCoeTryRun : Coe (Program TProgram) (TryRun (Program TProgram) TProgram) where
  coe _ := .mk (Program.program?)


def IsProcessed (prog: Program TProgram) : Prop := InitialState.IsProcessedM Option prog.program?

--instance [DecidableEq TProgram] : DecidablePred (IsProcessed : Program TProgram → Prop) := (inferInstance : (DecidablePred (InitialState.IsProcessedM Option)))


def State.IsError (s: Program TProgram) : Prop := s.program?.isNone

instance : DecidablePred (State.IsError : Program TProgram → Prop) :=
  fun s => dite (s.program?.isNone) .isTrue .isFalse

end program_1_s



namespace Runtime

/-
structure Load TProgram [Program TProgram] where
  load? (s: Program.State TProgram) : Option TProgram

variable {TProgram} [Program TProgram] in
instance Load.instCoeTryRun : Coe (Load TProgram) (TryRun (Program.State TProgram) TProgram) where coe x := .mk (x.load?)
-/

inductive Label : Type 0 where
  | load
  | takeInput
  | produceOutput
  deriving DecidableEq

namespace Label

instance instLawfulBEq : LawfulBEq Label := inferInstance


inductive Transition : Type 0 where
  | init
  | step (pre: Label) (post: Label)
  deriving DecidableEq


namespace Transition

instance instLawfulBEq : LawfulBEq Transition := inferInstance

def post (tr: Transition) : Label :=
  match tr with
  | .init => .load
  | .step _ post => post

def pre? (tr: Transition) : Option Label :=
  match tr with
  | .init => .none
  | .step pre _ => .some pre

theorem pre?_not_init (tr: Transition) (not_init: tr ≠ .init) : (pre? tr) ≠ .none := by
  simp [pre?]
  match tr with
  | .init => contradiction
  | .step _ _ => simp

def pre (tr: Transition) (not_init: tr ≠ .init) : Label :=
  match meq1: pre? tr with
  | .none => absurd meq1 (pre?_not_init tr not_init)
  | .some v => v

end Transition



abbrev Literal (l: Label) : Type := { x: Label // x = l }

instance Literal.instSubsingleton {l} : Subsingleton (Literal l) where
  allEq := by simp

end Label

protected structure Config where
  innerMotive : Motive.{1,u} Label
  outerMotive : Motive.{1,u} Label.Transition



namespace Config

open Label
variable (cfg: Runtime.Config) (tr: Transition)

def PreInner : Type _ := if h: tr = .init then PUnit else tr.pre h |> cfg.innerMotive

def PreOuter : Type _ := cfg.outerMotive tr

def PostInner : Type _ := cfg.innerMotive tr.post

end Config
open Config

def StepSpec (cfg: Runtime.Config) : Sort _ := (tr: Label.Transition) → (PreInner cfg tr) → (PreOuter cfg tr) → (PostInner cfg tr) → Prop

namespace StepSpec

namespace Default

protected def isValidTransition (tr: Label.Transition) : Bool :=
  match tr with
  | .init => .true
  | .step pre post =>
  match pre, post with
  | _, .load => .false
  | .load, .takeInput | .produceOutput, .takeInput => .true
  | .takeInput, .produceOutput => .false
  | _, _ => .false

abbrev IsValidTransition (tr: Label.Transition) : Prop := Default.isValidTransition tr

instance {tr} : Decidable (IsValidTransition tr) := if h: Default.isValidTransition tr then .isTrue h else .isFalse h

end Default

def defaultSpec (cfg: Runtime.Config) : StepSpec cfg := fun tr _ _ _ => StepSpec.Default.IsValidTransition tr

end StepSpec



end Runtime

open Runtime Label Config in
structure Runtime
  extends
    toConfig: Runtime.Config
  where
  stepSpec: StepSpec toConfig := StepSpec.defaultSpec toConfig
  step
    (tr: Label.Transition)
    (preInner: toConfig.PreInner tr)
    (preOuter: toConfig.PreOuter tr)
    : toConfig.PostInner tr --{ postInner: toConfig.PostInner tr // stepSpec tr preInner preOuter postInner }
  step_meets_spec: ∀tr preInner preOuter, stepSpec tr preInner preOuter (step tr preInner preOuter)


namespace Runtime



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

structure Memory.Config : Type (u+1) where
  protected Program : Type u
  protected Data : Type u
  protected Output : Type u

inductive Memory (sc: Memory.Config) : Label → Type _ where
  | load :
      sc.Program → Memory sc .load
  | takeInput :
      sc.Data → sc.Program → Memory sc .takeInput
  | produceOutput :
      sc.Output → sc.Program → Memory sc .produceOutput


def MemoryM (sc: Memory.Config) (m: Monadic) (l: Label) : Type _ := m l (Memory sc l)

--def LogT.mk {sc m l} (x: m l (State sc l)) : StepT sc m l := x

inductive HistoryM (sc: Memory.Config) (m: Monadic) : Label → Type _ where
  | load :
      MemoryM sc m .load →
      HistoryM sc m .load
  | takeInput :
      (l: Label) →
      (HistoryM sc m l) →
      MemoryM sc m .takeInput →
      HistoryM sc m .takeInput
  | produceOutput :
      (HistoryM sc m .produceOutput) →
      MemoryM sc m .produceOutput →
      HistoryM sc m .produceOutput


namespace HistoryM

--def Spec (sc: State.Config) (m: Monadic) : Type _ := (l: Label) → (HistoryM sc m l) → Prop

/-
class Step (sc: State.Config) (m: Monadic) where
  spec: Spec sc m
  step
-/

end HistoryM


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
