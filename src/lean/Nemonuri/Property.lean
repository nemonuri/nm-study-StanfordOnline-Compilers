module

public import Mathlib.Tactic.Contrapose

@[expose] public section public_s

set_option autoImplicit false
universe u

namespace Nemonuri

structure Property (σ: Type u) where
  protected T: Type u
  protected v: σ → T

namespace Property

structure IsLawfulPartial {σ: Type u} (σNone: σ) (p: Property σ) (pNone: p.T) : Prop where
  none_eq: (p.v σNone = pNone)
  source_exists (v: p.T) : (v ≠ pNone) → ∃(s: σ), (p.v s = v)

variable {σ: Type u} {σNone: σ} {p: Property σ} {pNone: p.T}


/-
theorem IsLawfulPartial.contrapose_iff
  : IsLawfulPartial σNone p pNone ↔ ∀(s: σ), ¬(p.v s = pNone) → ¬(s = σNone) := by
  unfold IsLawfulPartial
  simp
  constructor
  · intro h1 s
    contrapose
    intro h2
    simp [h2] ; exact h1
-/

/-
theorem IsLawfulPartial._aux (h1: IsLawfulPartial σNone p pNone) (s: σ) (h2: p.v s ≠ pNone) : s ≠ σNone := by
  unfold IsLawfulPartial at h1
-/

end Property


class PartialProperty (σ: Type u) [Zero σ]
  extends
    toProperty: Property σ
  where
  protected zero : T
  is_lawful_partial : Property.IsLawfulPartial 0 toProperty zero

variable {σ: Type u} [Zero σ]

instance PartialProperty.instZeroT [inst: PartialProperty σ] : Zero inst.T where
  zero := inst.zero



end Nemonuri

end public_s
