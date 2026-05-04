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

class HasSnapShot (tc: TypeContext) (T: Type u) where
  snapShot (target: T) : SnapShot tc

end State

open State in
class HasState (tc: TypeContext) (T: Type u)
  extends HasSnapShot tc T where
  setState (before: T) (arg: SnapShot tc) : { after: T // (snapShot after) = arg }

end Implementer


open Implementer State in
class Implementer (tc: TypeContext) (T : Type u)
  extends HasApproach T where
  init {label: Label} :
    State tc label →
    (prog: tc.TProgram) →
    { after: State tc .init // match after with | .init prog2 => prog = prog2 }
  takeInput :
    State tc .init →
    (data: tc.TData) →
    { after: State tc .takeInput // match after with | .takeInput _ data2 _ => data = data2 }
  produceOutput :
    State tc .takeInput →
    (output: tc.TOutput) →
    { after: State tc .produceOutput // match after with | .produceOutput _ output2 _ => output = output2 }


open Implementer State in
class ImplementerM (tc: TypeContext) (T : Type u)
  extends Implementer tc T, HasState tc T





end Program


/-
protected class abbrev Key (T: Type u) := BEq T, Hashable T, EquivBEq T, LawfulHashable T

namespace Programs

class HasProgramMap (T: Type u) (TKey: Type u) [Compilers.Key TKey] (toProgramType: TKey → Type u) where
  toProgramMap (self: T) : Std.ExtDHashMap TKey toProgramType

inductive Writer (TOther: Type u) [Compilers.Key TOther] where
  | you
  | other (x: TOther)
  deriving BEq, Hashable

namespace Writer


variable {TOther: Type u} [k: Compilers.Key TOther]

instance instEquivBEq : EquivBEq (Writer TOther) where
  symm := by
    unfold BEq.beq instBEqWriter instBEqWriter.beq at *
    intro a b h1
    match meq1: a, meq2: b with
    | .you, .you => simp
    | .other _, .other _ =>
      apply k.symm; trivial
  rfl := by
    unfold BEq.beq instBEqWriter instBEqWriter.beq at *
    intro a
    simp
    match meq1: a with
    | .you => simp
    | .other xa =>
      apply k.rfl
  trans := by
    unfold BEq.beq instBEqWriter instBEqWriter.beq at *
    intro a b c h1 h2
    match meq1: a, meq2: b, meq3: c with
    | .you, .you, .you => simp
    | .other _, .other _, .other _ =>
      simp_all
      exact k.trans h1 h2

instance instLawfulHashable : LawfulHashable (Writer TOther) where
  hash_eq a b h1 := by
    unfold Hashable.hash instHashableWriter instHashableWriter.hash at *
    simp
    match meq1: a, meq2: b with
    | .you, you => simp
    | .other xa, .other xb =>
      simp
      rw [k.hash_eq xa xb]
      unfold BEq.beq instBEqWriter instBEqWriter.beq at h1
      simp at h1
      exact h1

variable [k2: LawfulBEq TOther]

instance : LawfulBEq (Writer TOther) where
  eq_of_beq := by
    unfold BEq.beq instBEqWriter instBEqWriter.beq at *
    intro a b h1
    simp_all
    match meq1: a, meq2: b with
    | .you, .you => rfl
    | .other _, other _ =>
      simp at h1
      simp [h1]

end Writer

end Programs
-/


end Nemonuri.Study.StanfordOnline.Compilers


end expose_s
end public_s
