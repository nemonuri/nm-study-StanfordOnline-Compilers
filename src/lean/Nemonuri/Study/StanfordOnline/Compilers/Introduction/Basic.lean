module


public import Mathlib.Data.Fintype.Defs

public section public_s
@[expose] section expose_s

set_option autoImplicit false

namespace Nemonuri.Study.StanfordOnline.Compilers

def Motive.{u1, u2} (T: Sort u1) : Sort (max u1 (u2 + 1)) := T → Sort u2

--def MotiveM {T: Type _} (mt: Motive T) (m: Type _ → Type _) : Type _ := (t: T) → m (mt t)


def Monadic.{u1, u2, u3} (T: Type u1) [DecidableEq T]
  : Type (max (max u1 (u2 + 1)) (u3 + 1)) :=
  T → Type u2 → Type u3


namespace Motive

protected def run {T} (mtv: Motive T) (t: T) := mtv t

--class Lift m [Monad m] {T} (mt1: Motive T) (mt2: Motive (m T)) where
--  lift {l: Label} {α: Type u1} : (m1 l α) → (m2 l α)





def IsEquiv S (m1 m2: Motive S) : Prop := (s: S) → (m1 s) = (m2 s)

attribute [scoped simp] IsEquiv

instance instEquivalenceIsEquiv {S} : Equivalence (IsEquiv S) where
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


instance instSetoid {S} : Setoid (Motive S) where
  r := IsEquiv S
  iseqv := instEquivalenceIsEquiv

end Motive


namespace Monadic

universe u
variable {T: Type u} [DecidableEq T]


def toMotive (m: Monadic T)
  : Motive T :=
  (fun (t: T) => (α: Type _) → (m t α))



def IsEquiv T [DecidableEq T] (m1 m2: Monadic T) : Prop := (t: T) → (α: Type _) → (m1 t α) = (m2 t α)

attribute [scoped simp] Motive.IsEquiv Monadic.IsEquiv Monadic.toMotive


theorem IsEquiv.imp_toMotive_isEquiv (m1 m2: Monadic T) (is_equiv: IsEquiv T m1 m2)
  : Motive.IsEquiv T m1.toMotive m2.toMotive := by
    simp at is_equiv
    simp
    intro l
    exact (is_equiv l |> pi_congr)

#print IsEquiv.imp_toMotive_isEquiv

/-
theorem IsEquiv.iff_toMotive_isEquiv.{u1, u2} (m1 m2: Monadic.{u1, u2})
  : (IsEquiv m1 m2) ↔ (Motive.IsEquiv m1.toMotive m2.toMotive) := by
  constructor
  case mp => exact (IsEquiv.imp_toMotive_isEquiv m1 m2)
  case mpr =>
    simp
    intro toMotive_isEquiv l α
    --replace toMotive_isEquiv := toMotive_isEquiv l
    --apply funext
    --cbv --at toMotive_isEquiv
    --skip
    --conv at toMotive_isEquiv =>
-/


instance instEquivalenceIsEquiv : Equivalence (IsEquiv T) where
  refl m := by simp
  symm := by simp_all
  trans := by simp_all


instance instSetoid : Setoid (Monadic T) where
  r := IsEquiv T
  iseqv := instEquivalenceIsEquiv



def constId T [DecidableEq T] : Monadic T := Function.const T Id

instance instInhabited : Inhabited (Monadic T) where
  default := constId T

--class LiftT.{u1, u2, u3} (m1: Monadic.{u1, u2}) (m2: Monadic.{u1, u3}) where
--  lift {l: Label} {α: Type u1} : (m1 l α) → (m2 l α)

end Monadic

class HasLabel (TTarget TLabel: Type _) [DecidableEq TLabel] where
  label (t: TTarget) : TLabel

class MaybeHasLabel (TTarget TLabel: Type _) [DecidableEq TLabel] where
  label? (t: TTarget) : Option TLabel

class HasLabelWhen (TTarget TLabel: Type _) (pred: TTarget → Prop) [DecidableEq TLabel]
  extends
    toMaybeHasLabel: MaybeHasLabel TTarget TLabel
  where
  label?_not_none (t: TTarget) (h: pred t) : (toMaybeHasLabel.label? t) ≠ .none

namespace HasLabelWhen

instance instHasLabelSubtype
  {TTarget TLabel: Type _} {pred: TTarget → Prop} [DecidableEq TLabel] [HasLabelWhen TTarget TLabel pred]
  : HasLabel (Subtype pred) TLabel where
  label t :=
    match t with
    | ⟨val, prop⟩ =>
    match meq: MaybeHasLabel.label? val with
    | Option.some v => v
    | Option.none => absurd meq (HasLabelWhen.label?_not_none val prop)

end HasLabelWhen




end Nemonuri.Study.StanfordOnline.Compilers

end expose_s
end public_s
