module

public import Cslib.Foundations.Semantics.LTS.Basic
public import Cslib.Foundations.Semantics.LTS.Execution

@[expose] public section public_s

set_option autoImplicit false

variable {State Label : Type*}


namespace Cslib.LTS

theorem MTr.single_iff
  (lts : LTS State Label) (s1 : State) (μ : Label) (s2 : State)
  : lts.MTr s1 [μ] s2 ↔ lts.Tr s1 μ s2 :=
  Iff.intro (Cslib.LTS.MTr.single_invert lts s1 μ s2) (Cslib.LTS.MTr.single lts)


theorem MTr.split_iff
  (lts : LTS State Label) {s0 : State} {μs1 μs2 : List Label} {s2 : State}
  : (lts.MTr s0 (μs1 ++ μs2) s2) ↔ (∃ s1, lts.MTr s0 μs1 s1 ∧ lts.MTr s1 μs2 s2) := by
  open Classical in
  constructor
  · intro h ; exact MTr.split h
  · intro h
    obtain ⟨_, ⟨mtrL, mtrR⟩⟩ := h
    exact Cslib.LTS.MTr.comp lts mtrL mtrR


def IsImageNonempty (lts : LTS State Label) (s1: State) (μ : Label) : Prop :=
  (lts.image s1 μ).Nonempty

theorem isImageNonempty_iff {lts : LTS State Label} {s1: State} {μ : Label}
  : lts.IsImageNonempty s1 μ ↔ ∃s2, lts.Tr s1 μ s2 := by
  simp [IsImageNonempty, Set.Nonempty, Membership.mem, Set.Mem, Cslib.LTS.image, setOf]

end Cslib.LTS

end public_s
