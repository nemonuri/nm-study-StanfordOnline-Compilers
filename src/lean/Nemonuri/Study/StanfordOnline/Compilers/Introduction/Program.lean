module


public section public_s
@[expose] section expose_s


namespace Nemonuri.Study.StanfordOnline.Compilers

universe u --u1 u2
set_option autoImplicit false


namespace Program


namespace Runtime


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

def Motive.mk (x: Label → Sort u) : Motive := x

namespace Motive


def IsEquiv (m1 m2: Motive) : Prop := (l: Label) → (m1 l) = (m2 l)

attribute [scoped simp] IsEquiv

instance instEquivalenceIsEquiv : Equivalence IsEquiv where
  refl m := by simp
  symm := by
    intro m1 m2 is_eq
    simp at is_eq
    simp [is_eq]
  trans := by
    intro m1 m2 m3 is_eq_12 is_eq_23
    simp at is_eq_12
    simp at is_eq_23
    simp [is_eq_12, is_eq_23]


instance instSetoid : Setoid Motive where
  r := IsEquiv
  iseqv := instEquivalenceIsEquiv


end Motive

def Monadic.{u1, u2} : Type _ := Label → Type u1 → Type u2


namespace Monadic

def toMotive (m: Monadic) : Motive := Motive.mk (fun (l: Label) => (α: Type _) → (m l α))

#print toMotive

--protected def bindLabel (m: Monadic) (l: Label) := m l

def constId : Monadic := Function.const Label Id

instance instInhabited : Inhabited Monadic where
  default := constId

class LiftT.{u1, u2, u3} (m1: Monadic.{u1, u2}) (m2: Monadic.{u1, u3}) where
  lift {l: Label} {α: Type u1} : (m1 l α) → (m2 l α)

end Monadic


/-
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
-/

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
