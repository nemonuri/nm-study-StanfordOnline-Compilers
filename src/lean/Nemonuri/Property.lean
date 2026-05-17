module


@[expose] public section public_s

set_option autoImplicit false
universe u

namespace Nemonuri

--structure Property (σ: Type u) (T: Type u) where
--  v: σ → T

--namespace Property

class ZeroEq {σ T: Type u} [Zero σ] [Zero T] (f: σ → T) : Prop where
  zero_eq: (f 0 = 0)

variable {σ T: Type u} [Zero σ] [Zero T] (f: σ → T)

theorem zeroEq_iff : (ZeroEq f) ↔ (f 0 = 0) := by
  constructor
  · intro h
    obtain ⟨h'⟩ := h ; exact h'
  · intro h
    constructor ; exact h

class SourceExists : Prop where
  source_exists (v: T) (h: v ≠ 0) : ∃(s: σ), (f s = v)

omit [Zero σ] in
theorem sourceExists_iff : (SourceExists f) ↔ ((v: T) → (v ≠ 0) → ∃(s: σ), (f s = v)) := by
  constructor
  · intro h
    obtain ⟨h'⟩ := h ; exact h'
  · intro h
    constructor ; exact h


class abbrev IsProperty : Prop := ZeroEq f, SourceExists f



end Nemonuri

end public_s
