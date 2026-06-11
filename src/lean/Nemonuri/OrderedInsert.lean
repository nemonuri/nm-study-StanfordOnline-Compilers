module

public import Mathlib.Data.List.Sort

@[expose] public section public_s


theorem List.Nodup.orderedInsert_nodup
  {α: Type*} (r : α → α → Prop) [DecidableRel r] (a: α) (as: List α)
  (req_nodup: as.Nodup) (req_not_mem: a ∉ as)
  : (List.orderedInsert r a as).Nodup := by
  cases as with
  | nil => simp
  | cons a1 as1 =>
    by_cases h1: r a a1
    · simp [h1]
      revert req_nodup req_not_mem
      simp
      intro lm1 lm2 lm3 lm4
      exact ⟨⟨lm3, lm4⟩, lm1, lm2⟩
    · simp [h1]
      simp at req_nodup req_not_mem
      have lm1 := orderedInsert_nodup r a as1 req_nodup.2 req_not_mem.2
      exact ⟨⟨(fun h => req_not_mem.1 (Eq.symm h)), req_nodup.1⟩, lm1⟩


theorem List.SortedLE.orderedInsert_sortedLE
  {α: Type*} [ord: Preorder α] [DecidableLE α] [Std.Total ord.le] (a: α) (as: List α)
  (req_sortedLE: as.SortedLE)
  : (List.orderedInsert (· ≤ ·) a as).SortedLE := by
  cases as with
  | nil => simp [List.sortedLE_iff_pairwise]
  | cons a1 as1 =>
    by_cases h1: a ≤ a1
    · simp [h1]
      revert req_sortedLE
      simp [List.sortedLE_iff_isChain, h1]
    · simp [h1]
      have lm1 := orderedInsert_sortedLE a as1 (by
        simp [List.sortedLE_iff_pairwise] at req_sortedLE
        simp only [← List.sortedLE_iff_pairwise] at req_sortedLE
        exact req_sortedLE.2 )
      simp [List.sortedLE_iff_pairwise]
      simp only [← List.sortedLE_iff_pairwise]; simp [lm1]
      constructor
      · exact Std.Total.rel_of_not_rel_swap h1
      · simp [List.sortedLE_iff_pairwise] at req_sortedLE
        exact req_sortedLE.1


end public_s
