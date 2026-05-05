module


public section public_s
@[expose] section expose_s


namespace Nemonuri.Study.StanfordOnline.Compilers

universe u
set_option autoImplicit false


namespace Program



namespace Runtime

namespace State

inductive Label where
  | load
  | takeInput
  | produceOutput
  deriving DecidableEq

instance Label.instLawfulBEq : LawfulBEq Label := inferInstance


structure TypeContext : Type (u+1) where
  TProgram : Type u
  TData : Type u
  TOutput : Type u


end State


inductive State (tc: State.TypeContext) : State.Label → Type u where
  | load :
      tc.TProgram →
      State tc .load
  | takeInput {label: State.Label} :
      State tc label →
      tc.TData →
      tc.TProgram →
      State tc .takeInput
  | produceOutput :
      State tc .takeInput →
      tc.TOutput →
      tc.TProgram →
      State tc .produceOutput


def toProgram {tc label} (state: State tc label) : tc.TProgram :=
  match state with
  | .load prog => prog
  | .takeInput _ _ prog => prog
  | .produceOutput _ _ prog => prog


namespace State


structure SnapShot (tc: TypeContext) where
  label : State.Label
  state : State tc label

abbrev toSnapShot {tc: TypeContext} {label: Label} (state: State tc label) := SnapShot.mk label state

class HasSnapShot (tc: TypeContext) (T: Type u) where
  snapShot (target: T) : SnapShot tc

end State

open State

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

class TakeInput (tc: TypeContext) where
  takeInput_req {label: Label} (state: State tc label) : Prop
  takeInput {label: Label} :
    ({state: State tc label // takeInput_req state}) →
    tc.TData →
    State tc .takeInput
  takeInput_spec {label} (pre: Subtype (@takeInput_req label)) data :
    match takeInput pre data with
    | .takeInput pre2 data2 _ => pre ≍ pre2 ∧ data = data2

abbrev TakeInput.ReqState {tc} (self: TakeInput tc) label := Subtype (@self.takeInput_req label)

class ProduceOutput (tc: TypeContext) where
  produceOutput :
    State tc .takeInput →
    State tc .produceOutput
  produceOutput_spec : ∀pre, match produceOutput pre with | .produceOutput pre2 _ _ => pre = pre2

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




end Nemonuri.Study.StanfordOnline.Compilers


end expose_s
end public_s
