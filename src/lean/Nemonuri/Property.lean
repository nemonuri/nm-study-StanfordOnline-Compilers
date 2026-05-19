module

public import Mathlib.Tactic.MkIffOfInductiveProp

@[expose] public section public_s

set_option autoImplicit false
universe u

namespace Nemonuri

--structure Property (σ: Type u) (T: Type u) where
--  v: σ → T

--namespace Property

variable {σ T: Type u}

@[mk_iff]
class ZeroEq [Zero σ] [Zero T] (f: σ → T) : Prop where
  zero_eq: (f 0 = 0)

def AppEq (f: σ → T) (s1 s2: σ) : Prop := (f s1) = (f s2)

theorem appEq_iff {f: σ → T} {s1 s2: σ} : (AppEq f s1 s2) ↔ ((f s1) = (f s2)) := by rfl

abbrev DecidableAppEq (f: σ → T) := (s1: σ) → (s2: σ) → Decidable (AppEq f s1 s2)

instance (priority := low) (f: σ → T) [DecidableEq T] : DecidableAppEq f :=
  fun s1 s2 => (inferInstanceAs (Decidable ((f s1) = (f s2))) : Decidable (AppEq f s1 s2))

class LawfulAppEqBEq (f: σ → T) extends BEq T where
  beq_iff_appEq (s1 s2: σ) : ((f s1) == (f s2)) ↔ AppEq f s1 s2

attribute [simp] LawfulAppEqBEq.beq_iff_appEq

instance LawfulAppEqBEq.instDecidableAppEq (f: σ → T) [LawfulAppEqBEq f] : DecidableAppEq f :=
  fun s1 s2 =>
    if h: (f s1) == (f s2) then
      .isTrue (by simp at h ; exact h)
    else
      .isFalse (by simp at h ; exact h)

instance (priority := low) (f: σ → T) [DecidableEq T] : LawfulAppEqBEq f where
  beq_iff_appEq s1 s2 := by simp [appEq_iff]


variable [Zero σ] [Zero T] (f: σ → T)

/-
@[simp]
theorem zeroEq_iff : (ZeroEq f) ↔ (f 0 = 0) := by
  constructor
  · intro h
    obtain ⟨h'⟩ := h ; exact h'
  · intro h
    constructor ; exact h
-/

/-
@[mk_iff]
class SourceExists : Prop where
  source_exists (v: T) (h: v ≠ 0) : ∃(s: σ), (f s = v)
-/

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

--@[mk_iff]
class abbrev Property := Zero T, ZeroEq f, LawfulAppEqBEq f--, SourceExists f



--theorem IsProperty.imp_and (self: IsProperty f) : ZeroEq f ∧ SourceExists f := (isProperty_iff f).mp self

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
