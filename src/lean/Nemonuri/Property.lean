module

public import Mathlib.Tactic.MkIffOfInductiveProp

@[expose] public section public_s

set_option autoImplicit false
universe u

namespace Nemonuri

--structure Property (σ: Type u) (T: Type u) where
--  v: σ → T

--namespace Property

@[mk_iff]
class ZeroEq {σ T: Type u} [Zero σ] [Zero T] (f: σ → T) : Prop where
  zero_eq: (f 0 = 0)


variable {σ T: Type u} [Zero σ] [Zero T] (f: σ → T)

/-
@[simp]
theorem zeroEq_iff : (ZeroEq f) ↔ (f 0 = 0) := by
  constructor
  · intro h
    obtain ⟨h'⟩ := h ; exact h'
  · intro h
    constructor ; exact h
-/

@[mk_iff]
class SourceExists : Prop where
  source_exists (v: T) (h: v ≠ 0) : ∃(s: σ), (f s = v)

/-
omit [Zero σ] in
@[simp]
theorem sourceExists_iff : (SourceExists f) ↔ ((v: T) → (v ≠ 0) → ∃(s: σ), (f s = v)) := by
  constructor
  · intro h
    obtain ⟨h'⟩ := h ; exact h'
  · intro h
    constructor ; exact h
-/

@[mk_iff]
class abbrev IsProperty : Prop := ZeroEq f, SourceExists f



theorem IsProperty.imp_and (self: IsProperty f) : ZeroEq f ∧ SourceExists f := (isProperty_iff f).mp self

/-
@[simp]
theorem isProperty_iff : (IsProperty f) ↔ (ZeroEq f) ∧ (SourceExists f) := by
  constructor
  · intro h
    obtain ⟨_,_⟩ := h
    trivial
  · intro h
    have lm1 : IsProperty f := @IsProperty.mk _ _ _ _ f h.left h.right
    exact lm1
-/


end Nemonuri

end public_s
