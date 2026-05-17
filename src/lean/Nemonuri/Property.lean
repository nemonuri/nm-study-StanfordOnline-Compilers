module

public import Mathlib.Tactic.Contrapose
public import Mathlib.Logic.Basic

@[expose] public section public_s

set_option autoImplicit false
universe u

namespace Nemonuri

structure Property (σ: Type u) where
  protected T: Type u
  protected v: σ → T

namespace Property

class ZeroEq {σ: Type u} [Zero σ] (p: Property σ) [Zero p.T] : Prop where
  zero_eq: (p.v 0 = 0)

variable {σ: Type u} [Zero σ] (p: Property σ) [Zero p.T]

theorem zeroEq_iff : (ZeroEq p) ↔ (p.v 0 = 0) := by
  constructor
  · intro h
    obtain ⟨h'⟩ := h ; exact h'
  · intro h
    constructor ; exact h

class SourceExists : Prop where
  source_exists (v: p.T) (h: v ≠ 0) : ∃(s: σ), (p.v s = v)

omit [Zero σ] in
theorem sourceExists_iff : (SourceExists p) ↔ ((v: p.T) → (v ≠ 0) → ∃(s: σ), (p.v s = v)) := by
  constructor
  · intro h
    obtain ⟨h'⟩ := h ; exact h'
  · intro h
    constructor ; exact h

class abbrev IsLawfulPartial : Prop := ZeroEq p, SourceExists p


end Property


class PartialProperty (σ: Type u) [Zero σ]
  extends
    toProperty: Property σ
  where
  protected zero : T
  is_lawful_partial : @Property.IsLawfulPartial _ _ toProperty ⟨zero⟩

variable {σ: Type u} [Zero σ] [pp: PartialProperty σ]

instance PartialProperty.instZeroT : Zero pp.T where
  zero := pp.zero

open Property in
instance PartialProperty.instIsLawfulPartial : IsLawfulPartial pp.toProperty where
  zero_eq := pp.is_lawful_partial.zero_eq
  source_exists := pp.is_lawful_partial.source_exists


end Nemonuri

end public_s
