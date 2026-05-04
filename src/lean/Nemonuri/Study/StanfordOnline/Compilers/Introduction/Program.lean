module


public section public_s
@[expose] section expose_s


namespace Nemonuri.Study.StanfordOnline.Compilers

universe u
set_option autoImplicit false


namespace Program

namespace Implementer

inductive Approach where
  | compiler
  | interpreter
  deriving DecidableEq

instance Approach.instLawfulBEq : LawfulBEq Approach := inferInstance

class HasApproach (T: Type u) where
  approach (target: T) : Approach



namespace State

inductive Label where
  | init
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
  | init :
      tc.TProgram →
      State tc .init
  | takeInput :
      State tc .init →
      tc.TData →
      tc.TProgram →
      State tc .takeInput
  | produceOutput :
      State tc .takeInput →
      tc.TOutput →
      tc.TProgram →
      State tc .produceOutput


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


class CanInit (tc: TypeContext) (T : Type u) where
  init {label: Label} :
    State tc label →
    tc.TProgram →
    State tc .init
  init_spec {label: Label} (pre: State tc label) : ∀prog, match init pre prog with | .init prog2 => prog = prog2

class CanTakeInput (tc: TypeContext) (T : Type u) where
  takeInput :
    State tc .init →
    tc.TData →
    State tc .takeInput
  takeInput_spec : ∀pre data, match takeInput pre data with | .takeInput _ data2 _ => data = data2

class CanProduceOutput (tc: TypeContext) (T : Type u) where
  produceOutput :
    State tc .takeInput →
    tc.TOutput →
    State tc .produceOutput
  produceOutput_spec : ∀pre output, match produceOutput pre output with | .produceOutput _ output2 _ => output = output2

end Implementer

open Implementer State

class Implementer (tc: TypeContext) (T : Type u)
  extends HasApproach T, CanInit tc T, CanTakeInput tc T, CanProduceOutput tc T


class HasSourceProgram (TProgram T : Type u) where
  sourceProgram (target: T) : TProgram


class Runnable (tc: TypeContext) (T : Type u)
  extends Implementer tc T, HasState tc T, HasSourceProgram tc.TProgram T where
  sourceProgram_spec {label: Label} (pre: State tc label) :
    ∀target, match init pre (sourceProgram target) with | .init prog2 => (sourceProgram target) = prog2
  run (pre: T) (data: tc.TData) : T
  run_spec : ∀pre data, (snapShot (run pre data)).label = .produceOutput


end Program




end Nemonuri.Study.StanfordOnline.Compilers


end expose_s
end public_s
