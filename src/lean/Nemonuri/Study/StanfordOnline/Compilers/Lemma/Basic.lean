module

public import Cslib.Foundations.Semantics.LTS.Basic
public import Cslib.Foundations.Semantics.LTS.Execution
public import Mathlib.Data.List.Pairwise


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

--def List.rel_isTrans_getElem {α} (as: List α) (rel: α → α → Prop) [IsTrans α rel]
--  (h: as.length > 1) (k: Nat) (hk: k < as.length - 1)

theorem List.rel_isTrans_getElem_imp_pairwise {α} (rel: α → α → Prop) [IsTrans α rel]
  --(req1: as.length > 1) (k: Nat) (hk: k < as.length - 1)
  (as: List α)
  (h: (_: as.length > 1) → ∀(k: Nat), (hk: k < as.length - 1) → rel as[k] as[k+1])
  : as.Pairwise rel := by
  match as with
  | [] | [_] => simp
  | hd0::hd1::tl1 =>
    have lm1 := h (by simp) 0 (by simp)
    simp at lm1
    apply (List.Pairwise.cons_cons_of_trans lm1)
    match tl1 with
    | [] => simp
    | hd2::tl2 =>
      have lm2 := List.rel_isTrans_getElem_imp_pairwise rel (hd1 :: hd2 :: tl2) (by
        intro _ k hk
        have lm3 := h (by simp) (k+1) (by simpa using hk)
        simpa only [getElem_cons_succ] using lm3
      )
      exact lm2

--    simp
--    exact lm1
--  | hd::tl =>
--    simp [List.pairwise_cons]
/-
  | hd0 :: hd1 :: tl1 =>
    have lm1 := h (by simp) 0 (by simp)
    simp at lm1
    apply (List.Pairwise.cons_cons_of_trans lm1)
    --simp at h
    match tl1 with
    | [] => simp
    | hd2 :: tl2 =>
-/
/-
    have lm2 := List.rel_isTrans_getElem_imp_pairwise rel (hd1::tl1) (by
      intro _ k hk
      have lm3 := h (by simp) (k+1)
    )
-/
    --simp only [List.getElem_cons] at h

    --have := List.getElem_cons
    --have lm2 := List.rel_isTrans_getElem_imp_pairwise rel (hd1::tl1)


def List.rel_isTrans_pairwise_iff_getElem {α} (rel: α → α → Prop) [IsTrans α rel]
  (as: List α)
  : as.Pairwise rel ↔ ((_: as.length > 1) → ∀(k: Nat), (hk: k < as.length - 1) → rel as[k] as[k+1]) := by
  constructor
  · intro h _ k hk
    simp [List.pairwise_iff_getElem] at h
    specialize h k (k+1) (by omega) (by omega) (by omega)
    exact h
  · exact (List.rel_isTrans_getElem_imp_pairwise rel as)


/-
  match as with
  | [] => simp
  | [_] => simp
  | a0 :: a1 :: at =>
-/
/-
  by_cases h1: as.length ≤ 1
  · simp [h1]
    match as with
    | [] => exact Pairwise.nil
    | [_] => apply pairwise_singleton
    | _ :: _ :: _ => contradiction
  · simp at h1
    simp [h1]
    constructor
    · intro h2 k _
      simp [List.pairwise_iff_getElem] at h2
      specialize h2 k (k+1) (by omega) (by omega) (by omega)
      exact h2
    · intro h2
      simp [List.pairwise_iff_getElem]
-/

end public_s
