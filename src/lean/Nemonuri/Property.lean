module

public import Mathlib.Tactic.MkIffOfInductiveProp
public import Mathlib.Logic.Basic
public import Mathlib.Logic.Relation

@[expose] public section public_s

set_option autoImplicit false
universe u

namespace Nemonuri

--structure Property (σ: Type u) (T: Type u) where
--  v: σ → T

--namespace Property

variable {σ T: Type u}

@[mk_iff]
class ZeroEq [Zero σ] [Zero T] (v: σ → T) : Prop where
  zero_eq: (v 0 = 0)

def AppEq (v: σ → T) (s1 s2: σ) : Prop := (v s1) = (v s2)

theorem appEq_iff {v: σ → T} {s1 s2: σ} : (AppEq v s1 s2) ↔ ((v s1) = (v s2)) := by rfl

open Function in
theorem appEq_iff_onFun {v: σ → T} {s1 s2: σ}
  : (AppEq v s1 s2) ↔ ((Eq on v) s1 s2) := by
  simp only [Function.onFun]
  exact appEq_iff

theorem appEq_equivalence (v: σ → T) : Equivalence (AppEq v) := eq_equivalence.comap v

namespace AppEq

@[implicit_reducible]
protected def ker (v: σ → T) : Setoid σ := ⟨AppEq v, appEq_equivalence v⟩

@[implicit_reducible]
protected def hasEquiv (v: σ → T) : HasEquiv σ := @instHasEquivOfSetoid σ (AppEq.ker v)

protected abbrev Equiv (v: σ → T) (s1 s2: σ) : Prop := (AppEq.hasEquiv v).Equiv s1 s2

protected abbrev Quotient (v: σ → T) := Quotient (AppEq.ker v)

protected def quot (v: σ → T) (s: σ) : AppEq.Quotient v := Quotient.mk (AppEq.ker v) s

end AppEq



abbrev DecidableAppEq (v: σ → T) := (s1: σ) → (s2: σ) → Decidable (AppEq v s1 s2)

instance (priority := low) (v: σ → T) [DecidableEq T] : DecidableAppEq v :=
  fun s1 s2 => (inferInstanceAs (Decidable ((v s1) = (v s2))) : Decidable (AppEq v s1 s2))

instance (priority := low) (v: σ → T) [DecidableAppEq v] : DecidableRel (AppEq v) :=
  fun s1 s2 => (inferInstance : DecidableAppEq v) s1 s2

instance (priority := low) (v: σ → T) [DecidableAppEq v] : DecidableRel (AppEq.Equiv v) :=
  inferInstanceAs (DecidableRel (AppEq v))

instance DecidableAppEq.instDecidableEqQuotient (v: σ → T) [DecidableAppEq v] : DecidableEq (AppEq.Quotient v) :=
  @Quotient.decidableEq σ (AppEq.ker v) (inferInstance : DecidableRel (AppEq.Equiv v))




class LawfulAppEqBEq (v: σ → T) extends BEq T where
  beq_iff_appEq (s1 s2: σ) : ((v s1) == (v s2)) ↔ AppEq v s1 s2

attribute [simp] LawfulAppEqBEq.beq_iff_appEq

instance LawfulAppEqBEq.instDecidableAppEq (v: σ → T) [LawfulAppEqBEq v] : DecidableAppEq v :=
  fun s1 s2 =>
    if h: (v s1) == (v s2) then
      .isTrue (by simp at h ; exact h)
    else
      .isFalse (by simp at h ; exact h)

instance (priority := low) (v: σ → T) [DecidableEq T] : LawfulAppEqBEq v where
  beq_iff_appEq s1 s2 := by simp [appEq_iff]




variable [Zero σ] [Zero T] (v: σ → T)

/-
@[simp]
theorem zeroEq_iff : (ZeroEq f) ↔ (v 0 = 0) := by
  constructor
  · intro h
    obtain ⟨h'⟩ := h ; exact h'
  · intro h
    constructor ; exact h
-/

/-
@[mk_iff]
class SourceExists : Prop where
  source_exists (v: T) (h: v ≠ 0) : ∃(s: σ), (v s = v)
-/

/-
omit [Zero σ] in
@[simp]
theorem sourceExists_iff : (SourceExists f) ↔ ((v: T) → (v ≠ 0) → ∃(s: σ), (v s = v)) := by
  constructor
  · intro h
    obtain ⟨h'⟩ := h ; exact h'
  · intro h
    constructor ; exact h
-/

--@[mk_iff]
class Property extends Zero T, ZeroEq v, LawfulAppEqBEq v where--, SourceExists f
  protected sur : Function.Surjective v

abbrev PropertyOf (σ: Type u) [Zero σ] {T : Type u} (v: σ → T) := Property v

abbrev PropertyAt (σ: Type u) [Zero σ] (T : Type u) (v: σ → T) := Property v

namespace Property

structure Context (σ: Type u) [Zero σ] where
  protected T: Type u
  protected v: σ → T
  protected zero: Zero T
  protected zeroEq: @ZeroEq σ T _ zero v
  protected lawfulAppEqBEq: LawfulAppEqBEq v
  protected sur : Function.Surjective v

instance Context.instProperty {σ: Type u} [Zero σ] {ctx: Context σ} : Property ctx.v :=
  @Property.mk σ ctx.T _ ctx.v ctx.zero ctx.zeroEq ctx.lawfulAppEqBEq ctx.sur

instance instLawfulBEq [Property v] : LawfulBEq T where
  rfl := by
    rename (Property v) => pty
    intro t
    obtain ⟨w1, w1_p⟩ := @pty.sur t
    have lm1 := pty.beq_iff_appEq ; simp only [appEq_iff] at lm1
    rw [← w1_p]
    specialize lm1 w1 w1
    simp only [lm1]
  eq_of_beq := by
    rename (Property v) => pty
    intro t1 t2 h
    obtain ⟨w1, w1_p⟩ := @pty.sur t1
    obtain ⟨w2, w2_p⟩ := @pty.sur t2
    have lm1 := pty.beq_iff_appEq ; simp only [appEq_iff] at lm1
    specialize lm1 w1 w2
    subst_eqs
    exact (lm1.mp h)


end Property


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
