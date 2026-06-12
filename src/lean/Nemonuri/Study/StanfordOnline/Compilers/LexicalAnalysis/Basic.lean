module

--public import Nemonuri.Study.StanfordOnline.Compilers.StructureOfCompiler.LexicalAnalysis
public import Nemonuri.Study.StanfordOnline.Compilers.Lemma
public import Mathlib.Logic.Equiv.Defs
--public import Mathlib.Logic.Encodable.Basic
public import Mathlib.Order.Lattice
--public import Mathlib.Logic.Basic
public import Mathlib.Data.Fintype.Defs
public import Mathlib.Data.Finset.Sort
public import Mathlib.Data.Finset.Union
public import Nemonuri.OrderedInsert

@[expose] public section public_s

namespace Nemonuri.OptionNat

set_option trace.Meta.synthInstance true

def equiv : Equiv (Option Nat) Nat where
  toFun n? :=
    match n? with
    | .none => 0
    | .some n => n + 1
  invFun n :=
    match n with
    | 0 => .none
    | n + 1 => .some n
  left_inv := by
    intro n?
    cases n? <;> simp
  right_inv := by
    intro n
    cases n <;> simp

instance : LinearOrder (Option Nat) := Equiv.linearOrder equiv

end Nemonuri.OptionNat

namespace Nemonuri.Study.StanfordOnline.Compilers

set_option autoImplicit false


/-!
#### 3.1. Lexical Analysis Part 1

1. Let's start by looking at a small code fragment.
2. This is a linear string, you can think of this as bytes in the file that the lexical analyzer has to work and it's going to mark through, placing dividers between the different units.

-/

class Eof (α: Type*) where
  eof: α

namespace LexicalAnalysis

structure CodeFragment (α: Type*) where
  ofList :: toList : List α
  deriving Repr, DecidableEq

namespace CodeFragment

variable {α}

instance : Coe (List α) (CodeFragment α) where coe a := ⟨a⟩

instance : EmptyCollection (CodeFragment α) := ⟨([]: List α)⟩

protected def length (cf: CodeFragment α) : Nat := cf.toList.length --List.length (equiv _ cf)

section Index

variable (cf: CodeFragment α) (i: Nat)

def IsTextIndex : Prop := i < cf.length

instance : Decidable (cf.IsTextIndex i) := inferInstanceAs (Decidable (_ < _))

@[simp]
theorem isTextIndex_def : cf.IsTextIndex i ↔ i < cf.length := by rfl

def IsEofIndex : Prop := i = cf.length

instance : Decidable (cf.IsEofIndex i) := inferInstanceAs (Decidable (_ = _))

@[simp]
theorem isEofIndex_def : cf.IsEofIndex i ↔ i = cf.length := by rfl


/-
theorem isTextIndex_xor_isEofIndex
  : Xor' (cf.IsTextIndex i) (cf.IsEofIndex i) := by
  simp [xor_def] --; omega
  constructor
-/

def IsValidIndex : Prop := cf.IsTextIndex i ∨ cf.IsEofIndex i

instance : Decidable (cf.IsValidIndex i) := inferInstanceAs (Decidable (_ ∨ _))

theorem isValidIndex_def
  : cf.IsValidIndex i ↔ (cf.IsTextIndex i ∨ cf.IsEofIndex i) := by
  rfl

theorem isValidIndex_iff_le_length
  : cf.IsValidIndex i ↔ i ≤ cf.length := by
  simp [IsValidIndex]
  omega

def ValidIndex := { i:Nat // cf.IsValidIndex i }

/-
theorem isValidIndex_iff_xor
  : cf.IsValidIndex i ↔ Xor' (cf.IsTextIndex i) (cf.IsEofIndex i) := by
  simp [isValidIndex_def, xor_iff_or_and_not_and]
  intro h1 h2
  omega
-/

inductive IsValidIndex.TextOrEof (h: IsValidIndex cf i) where
  | isText (h: cf.IsTextIndex i)
  | isEof (h: cf.IsEofIndex i)

def IsBorderIndex : Prop := cf.IsEofIndex i ∨ i = 0

instance : Decidable (cf.IsBorderIndex i) := inferInstanceAs (Decidable (_ ∨ _))

theorem isBorderIndex_def : cf.IsBorderIndex i ↔ cf.IsEofIndex i ∨ i = 0 := by rfl

def BorderIndex := { i:Nat // cf.IsBorderIndex i }

instance : DecidableEq cf.BorderIndex := inferInstanceAs (DecidableEq (Subtype _))

instance : LinearOrder cf.BorderIndex := inferInstanceAs (LinearOrder (Subtype _))


def IsInteriorIndex : Prop := cf.IsValidIndex i ∧ (¬cf.IsBorderIndex i)


theorem IsInteriorIndex_def : cf.IsInteriorIndex i ↔ (cf.IsValidIndex i ∧ (¬cf.IsBorderIndex i)) := by rfl

theorem IsInteriorIndex_iff_ioo : cf.IsInteriorIndex i ↔ (0 < i ∧ i < cf.length) := by
  simp [IsInteriorIndex_def, isValidIndex_iff_le_length, isBorderIndex_def]
  omega

theorem length_le_one_imp_not_isInteriorIndex : (cf.length ≤ 1) → (¬cf.IsInteriorIndex i) := by
  simp [IsInteriorIndex_iff_ioo]
  omega

instance : Decidable (cf.IsInteriorIndex i) := decidable_of_iff' (0 < i ∧ i < cf.length) (cf.IsInteriorIndex_iff_ioo i)

theorem isValidIndex_iff_border_or_interior : cf.IsValidIndex i ↔ (cf.IsBorderIndex i ∨ cf.IsInteriorIndex i) := by
  simp [IsInteriorIndex_def]
  by_cases h1: cf.IsBorderIndex i <;> simp [h1]
  · simp [isBorderIndex_def] at h1
    simp [isValidIndex_iff_le_length]
    omega

theorem isBorder_isInterior_disjoint : ¬(cf.IsBorderIndex i ∧ cf.IsInteriorIndex i) := by
  simp [IsInteriorIndex_def]
  exact (fun x _ => x)


inductive IsValidIndex.BorderOrInterior (h: IsValidIndex cf i) where
  | isBorder (h: cf.IsBorderIndex i)
  | isInterior (h: cf.IsInteriorIndex i)

def InteriorIndex := { i:Nat // cf.IsInteriorIndex i }

instance : DecidableEq cf.InteriorIndex := inferInstanceAs (DecidableEq (Subtype _))

instance : LinearOrder cf.InteriorIndex := inferInstanceAs (LinearOrder (Subtype _))

end Index


def borderIndexUniv (cf: CodeFragment α) : Finset cf.BorderIndex :=
  let val0: Multiset cf.BorderIndex := {⟨0, Or.inr (Eq.refl _)⟩}
  if h: cf.length = 0 then
    val0.toFinset
  else
    { val := (⟨cf.length, Or.inl ((cf.isEofIndex_def cf.length).mp (Eq.refl _))⟩ ::ₘ val0),
      nodup := by
        subst val0
        simp
        intro cont
        have lm1 := Subtype.ext_iff.mp cont
        simp at lm1
        exact h lm1 }

instance (cf: CodeFragment α) : Fintype cf.BorderIndex where
  elems := cf.borderIndexUniv
  complete x := by
    obtain ⟨xv, xv_p⟩ := x
    revert xv_p
    simp [isBorderIndex_def]
    intro xv_p
    unfold borderIndexUniv
    simp
    by_cases h1: cf.length = 0
    · revert xv_p
      simp [h1]
      intro xv_p
      simp [xv_p]
    · simp [h1]
      cases xv_p <;> rename_i h2 <;> simp [h2]

def interiorIndexUniv (cf: CodeFragment α) : Finset cf.InteriorIndex where
  val := (List.range' 1 (cf.length - 1) 1).attachWith cf.IsInteriorIndex (by
    simp [IsInteriorIndex_iff_ioo]
    intro x h1 h2
    omega)
    |> Multiset.ofList
  nodup := by
    simp [List.attachWith]
    apply List.Nodup.pmap (by
      intro _ _ _ _
      exact Subtype.ext_iff.mp
    )
    exact List.nodup_range'

instance (cf: CodeFragment α) : Fintype cf.InteriorIndex where
  elems := cf.interiorIndexUniv
  complete := by
    rintro ⟨xv, xv_p⟩
    unfold interiorIndexUniv
    simp
    apply (List.mem_attachWith _ _).mpr
    simp
    simp [IsInteriorIndex_iff_ioo] at xv_p
    omega

def validIndexUniv (cf: CodeFragment α) : Finset cf.ValidIndex where
    val := List.range (cf.length+1) |>.attachWith cf.IsValidIndex (by
        simp [isValidIndex_iff_le_length]; omega
      ) |> Multiset.ofList
    nodup := by
      simp [List.attachWith]
      apply List.Nodup.pmap (by
        intro _ _ _ _
        exact Subtype.ext_iff.mp
      )
      exact List.nodup_range

instance (cf: CodeFragment α) : Fintype cf.ValidIndex where
  elems := cf.validIndexUniv
  complete := by
    rintro ⟨xv, xv_p⟩
    unfold validIndexUniv
    simp
    apply (List.mem_attachWith _ _).mpr
    simp
    simp [isValidIndex_iff_le_length] at xv_p
    omega

namespace BorderIndex

variable (cf: CodeFragment α)

protected def card : Nat := Fintype.card cf.BorderIndex

theorem card_def : BorderIndex.card cf = Fintype.card cf.BorderIndex := by rfl

theorem card_spec : BorderIndex.card cf = if cf.length = 0 then 1 else 2 := by
  simp only [BorderIndex.card, Fintype.card]
  have lm1: Finset.univ = cf.borderIndexUniv := by rfl
  simp [lm1]
  unfold borderIndexUniv
  simp
  by_cases h1: cf.length = 0
  · simp [h1]
  · simp [h1, Finset.card_insert_eq_ite]
    intro cont
    exact h1 (Subtype.ext_iff.mp cont)

theorem card_gt_zero : BorderIndex.card cf > 0 := by
  simp
  have lm1 := card_spec cf
  by_cases h1: cf.length = 0 <;> simp [h1] at lm1 <;> omega

def sortedUniv : List cf.BorderIndex := Finset.univ.sort LE.le

theorem sortedUniv_ne_nil : BorderIndex.sortedUniv cf ≠ [] := by
  simp [BorderIndex.sortedUniv]
  intro cont
  rewrite [← List.length_eq_zero_iff] at cont
  simp at cont
  have lm1 := BorderIndex.card_spec cf
  simp [BorderIndex.card, cont] at lm1
  by_cases h1: cf.length = 0 <;> simp [h1] at lm1

protected def first : cf.BorderIndex := BorderIndex.sortedUniv cf |>.head (sortedUniv_ne_nil cf)

protected def last : cf.BorderIndex := BorderIndex.sortedUniv cf |>.getLast (sortedUniv_ne_nil cf)

instance : Nonempty cf.BorderIndex := .intro (BorderIndex.first cf)

theorem first_eq_min' : (BorderIndex.first cf) = Finset.univ.min' (by simp only [Finset.univ_nonempty]) := by
  simp [Finset.min'_eq_sorted_zero]
  unfold BorderIndex.first
  conv =>
    rhs
    rw [List.getElem_zero_eq_head (by
      simp [← card_def]
      exact card_gt_zero cf
    )]
  congr

/-
theorem min_congr (i1 i2: BorderIndex cf) : (min i1 i2).val = min i1.val i2.val := by
  simp [LinearOrder.min_def]
  by_cases h1: i1 ≤ i2
  · simp [h1]
    intro h2
    rewrite [lt_iff_not_ge] at h2
    contradiction
  · simp [h1]
    intro h2
    contradiction
-/

theorem val_monotone : Monotone (fun i: BorderIndex cf => i.val) := Subtype.mono_coe (IsBorderIndex cf)

def valEmbedding : Function.Embedding (BorderIndex cf) Nat where
  toFun i := i.val
  inj' _ _ := Subtype.ext


theorem first_val_eq : (BorderIndex.first cf).val = 0 := by
  simp [first_eq_min']
  have lm1 : Finset.univ = cf.borderIndexUniv := by rfl
  simp [lm1]
  unfold borderIndexUniv
  simp
  by_cases h1: cf.length = 0
  · simp [h1]
  · simp [h1]
    have lm2 (i1 i2: BorderIndex cf) : (min i1 i2).val = min _ _ := (val_monotone cf).map_min
    simp [lm2, h1]
    clear lm1 lm2
    rfl


theorem last_eq_max' : (BorderIndex.last cf) = Finset.univ.max' (by simp only [Finset.univ_nonempty]) := by
  simp [Finset.max'_eq_sorted_last]
  unfold BorderIndex.last
  let univ0 := @Finset.univ (BorderIndex cf) _
  have lm1 : univ0.card = univ0.sort.length := by
    subst univ0
    simp
  subst univ0
  conv =>
    rhs
    simp only [← Finset.card_univ]
    simp only [lm1]
    rw [List.getElem_length_sub_one_eq_getLast (by
      simp
      have lm2 := card_gt_zero cf
      simp [card_def] at lm2
      omega
    )]
  congr

theorem last_val_eq : (BorderIndex.last cf).val = cf.length := by
  simp [last_eq_max']
  have lm1 : Finset.univ = cf.borderIndexUniv := by rfl
  simp [lm1]
  unfold borderIndexUniv
  simp
  by_cases h1: cf.length = 0
  · simp [h1]
  · simp [h1]
    have lm2 (i1 i2: BorderIndex cf) : (max i1 i2).val = max _ _ := (val_monotone cf).map_max
    simp [lm2]
    conv => lhs; change 0
    omega

theorem first_last_eq_univ : Finset.univ = [BorderIndex.first cf, BorderIndex.last cf].toFinset := by
  symm
  simp
  have lm1 := @Finset.map_inj _ _ (valEmbedding cf)
  apply lm1.mp
  clear lm1
  unfold valEmbedding
  simp [first_val_eq, last_val_eq]
  have lm1 : Finset.univ = cf.borderIndexUniv := by rfl
  simp [lm1]
  unfold borderIndexUniv
  simp
  by_cases h1: cf.length = 0
  · simp [h1]
  · simp [h1]
    conv =>
      rhs
      arg 2
      change {0}
    simp [Finset.pair_comm]


def firstImpl : BorderIndex cf := ⟨0, Or.inr (Eq.refl _)⟩

@[csimp]
theorem first_eq_firstImpl : @BorderIndex.first = @firstImpl := by
  funext α cf
  apply Subtype.ext_iff.mpr
  simp [first_val_eq, firstImpl]

def lastImpl : BorderIndex cf := ⟨cf.length, Or.inl ((cf.isEofIndex_def cf.length).mp (Eq.refl _))⟩

@[csimp]
theorem last_eq_lastImpl : @BorderIndex.last = @lastImpl := by
  funext α cf
  apply Subtype.ext_iff.mpr
  simp [last_val_eq, lastImpl]

def embedToValidIndex : BorderIndex cf ↪ ValidIndex cf :=
  Subtype.impEmbedding _ _ (by simp [isBorderIndex_def, isValidIndex_def]; omega)

end BorderIndex

namespace InteriorIndex

variable (cf: CodeFragment α)

protected def card : Nat := Fintype.card cf.InteriorIndex

theorem card_def : InteriorIndex.card cf = Fintype.card cf.InteriorIndex := by rfl

theorem card_eq_interiorIndexUniv_card : InteriorIndex.card cf = cf.interiorIndexUniv.card := by
  simp [card_def]; rfl


theorem card_spec : InteriorIndex.card cf = if cf.length ≤ 1 then 0 else cf.length - 1 := by
  simp [card_eq_interiorIndexUniv_card]
  unfold interiorIndexUniv
  simp
  conv => lhs; simp [InteriorIndex]
  simp
  intro _
  omega

def embedToValidIndex : InteriorIndex cf ↪ ValidIndex cf :=
  Subtype.impEmbedding _ _ (by
    simp [IsInteriorIndex_def, isValidIndex_iff_le_length]
    intro _ h _; exact h)

end InteriorIndex

namespace ValidIndex

variable (cf: CodeFragment α)

instance : DecidableEq cf.ValidIndex := inferInstanceAs (DecidableEq (Subtype _))

instance : LinearOrder cf.ValidIndex := inferInstanceAs (LinearOrder (Subtype _))

protected def card : Nat := Fintype.card cf.ValidIndex

theorem card_def : ValidIndex.card cf = Fintype.card cf.ValidIndex := by rfl

theorem card_eq_validIndexUniv_card : ValidIndex.card cf = cf.validIndexUniv.card := by
  simp [card_def]; rfl

theorem card_spec : ValidIndex.card cf = cf.length + 1 := by
  simp [card_eq_validIndexUniv_card]
  unfold validIndexUniv
  simp [ValidIndex]

theorem card_eq_borderIndex_card_add_interiorIndex_card
  : ValidIndex.card cf = BorderIndex.card cf + InteriorIndex.card cf := by
  simp [ValidIndex.card_spec, BorderIndex.card_spec, InteriorIndex.card_spec]
  by_cases h1: cf.length = 0 <;> simp [h1]
  · by_cases h2: cf.length ≤ 1 <;> simp [h2] <;> omega


theorem univ_eq_borderIndex_univ_union_interiorIndex_univ
  : SetLike.coe (@Finset.univ (ValidIndex cf) _) =
    (SetLike.coe (Finset.univ.map (BorderIndex.embedToValidIndex _))) ∪
    (SetLike.coe (Finset.univ.map (InteriorIndex.embedToValidIndex _)))
  := by
  simp only [Finset.coe_univ]
  simp only [Finset.coe_map]
  simp only [Finset.coe_univ, Set.image_univ]
  ext x
  simp only [Set.mem_union]
  simp only [Set.mem_univ, true_iff]
  simp only [Set.mem_range]
  by_cases h1: cf.IsBorderIndex x.val
  · left
    exists ⟨x.val, h1⟩
  · right
    have lm1: cf.IsInteriorIndex x.val := by
      simp [IsInteriorIndex_def]; simp [h1]; exact x.property
    exists ⟨x.val, lm1⟩

theorem border_interior_univ_map_disjoint
  : Disjoint (Finset.univ.map (BorderIndex.embedToValidIndex cf)) (Finset.univ.map (InteriorIndex.embedToValidIndex cf)) := by
  simp [Finset.disjoint_iff_ne]
  simp [BorderIndex, InteriorIndex, ValidIndex]
  intro x1 x1p x2 _ cont
  simp [Subtype.ext_iff] at cont
  change x1 = x2 at cont
  have lm1 := cf.isBorder_isInterior_disjoint x1 |> not_and.mp <| x1p
  simp [cont] at lm1
  contradiction

end ValidIndex

inductive BorderOrInterior where
  | border
  | interior
  deriving DecidableEq, Repr, Ord

namespace BorderOrInterior

variable (cf: CodeFragment α)

protected def all : List BorderOrInterior := [.border, .interior]

protected def univ : Finset BorderOrInterior := BorderOrInterior.all.toFinset

theorem univ_val_eq_all : BorderOrInterior.univ.val = ↑BorderOrInterior.all := by
  unfold BorderOrInterior.univ
  simp only [List.toFinset_val]
  simp only [Multiset.coe_eq_coe]
  unfold BorderOrInterior.all
  simp

instance : Fintype BorderOrInterior where
  elems := BorderOrInterior.univ
  complete x := by
    rw [← Finset.mem_val, univ_val_eq_all]
    simp [BorderOrInterior.all]
    cases x <;> trivial

def toValidIndexFinset (i: BorderOrInterior) : Finset (ValidIndex cf) :=
  match i with
  | .border => Finset.univ.map (BorderIndex.embedToValidIndex _)
  | .interior => Finset.univ.map (InteriorIndex.embedToValidIndex _)


theorem toValidIndexFinset_pairwiseDisjoint
  : (@Finset.univ BorderOrInterior _ : Set _).PairwiseDisjoint (toValidIndexFinset cf) := by
  simp
  have lm1: Set.univ = (BorderOrInterior.univ: Set BorderOrInterior) := by
    symm
    simp [Set.eq_univ_iff_forall]
    exact Fintype.complete
  simp [lm1]
  simp only [BorderOrInterior.univ, List.coe_toFinset]
  simp [BorderOrInterior.all]
  simp only [Set.PairwiseDisjoint]
  simp only [Set.Pairwise]
  simp
  simp [disjoint_comm]
  simp only [Function.onFun]
  unfold toValidIndexFinset
  simp
  exact ValidIndex.border_interior_univ_map_disjoint cf


def toValidIndexFinsetUnion : Finset (ValidIndex cf) :=
  Finset.univ.disjiUnion (toValidIndexFinset _) (toValidIndexFinset_pairwiseDisjoint _)

theorem toValidIndexFinsetUnion_eq_univ
  : (toValidIndexFinsetUnion cf) = Finset.univ := by
  apply SetLike.coe_injective
  ext x
  simp
  unfold toValidIndexFinsetUnion
  simp
  revert x
  unfold ValidIndex
  simp [isValidIndex_iff_border_or_interior]
  intro x xp
  cases xp <;> rename_i lm1 <;> unfold toValidIndexFinset
  · exists .border
    simp [BorderIndex, ValidIndex]
    exists x; exists lm1
  · exists .interior
    simp [InteriorIndex, ValidIndex]
    exists x; exists lm1


end BorderOrInterior


namespace IsValidIndex

def toTextOrEof {cf: CodeFragment α} {i: Nat} (req: IsValidIndex cf i) : IsValidIndex.TextOrEof _ _ req :=
  if h: cf.IsTextIndex i then
    .isText h
  else
    .isEof (by simp only [isValidIndex_def, h, false_or] at req; exact req)

def toBorderOrInterior {cf: CodeFragment α} {i: Nat} (req: IsValidIndex cf i) : IsValidIndex.BorderOrInterior _ _ req :=
  if h: cf.IsBorderIndex i then
    .isBorder h
  else
    .isInterior (And.intro req h)

end IsValidIndex



protected def get [Eof α] (cf: CodeFragment α) (i: Nat) (req: IsValidIndex cf i) : α :=
  match req.toTextOrEof with
  | .isText _ => cf.toList[i]
  | .isEof _ => Eof.eof


instance [Eof α] : GetElem (CodeFragment α) Nat α IsValidIndex where
  getElem := CodeFragment.get


end CodeFragment


structure Divider.Raw where
  ofNat:: toNat: Nat
  deriving Repr, DecidableEq

/-
structure Divider.Pred.Raw where
  idx?: Option Nat
  deriving Repr, DecidableEq
-/

/-
namespace Divider.Pred.Raw

set_option trace.Meta.synthInstance true

def equiv : Equiv Raw (Option Nat) where
  toFun := Raw.idx?
  invFun := Raw.mk

instance : LinearOrder Raw := Equiv.linearOrder equiv

@[mk_iff]
structure IsValid {α} (cf: CodeFragment α) (r: Raw) : Prop where
  none_or_le_length: match r.idx? with | .none => True | .some n => n ≤ cf.length

end Divider.Pred.Raw

open Divider.Pred in
@[ext]
structure Divider.Pred {α} (cf: CodeFragment α) where
  raw: Pred.Raw
  is_valid: Raw.IsValid cf raw
-/

namespace Divider.Raw

set_option trace.Meta.synthInstance true

def equiv : Divider.Raw ≃ Nat where
  toFun := Divider.Raw.toNat
  invFun := Divider.Raw.ofNat

theorem equiv_coe_eq : ⇑equiv = Raw.toNat := by rfl

theorem equiv_symm_coe_eq : ⇑equiv.symm = Raw.ofNat := by rfl

instance : LinearOrder Divider.Raw := Equiv.linearOrder equiv

instance : LT Divider.Raw := LinearOrder.toPartialOrder.toLT

def orderIso : Divider.Raw ≃o Nat where
  toEquiv := equiv
  map_rel_iff' := by
    intro _ _
    simp [equiv_coe_eq]
    rfl

def IsValid {α} (cf: CodeFragment α) (i: Raw) : Prop := cf.IsValidIndex i.toNat

theorem isValid_def {α} (cf: CodeFragment α) (i: Raw)
  : i.IsValid cf ↔ cf.IsValidIndex i.toNat := by
  rfl

def IsInterior {α} (cf: CodeFragment α) (i: Raw) : Prop := cf.IsInteriorIndex i.toNat

theorem isInterior_def {α} (cf: CodeFragment α) (i: Raw)
  : i.IsInterior cf ↔ cf.IsInteriorIndex i.toNat := by
  rfl

instance {α} (cf: CodeFragment α) (i: Raw) : Decidable (IsInterior cf i) := decidable_of_iff' _ (isInterior_def _ _)

open CodeFragment in
theorem IsInterior.imp_isValid {α} {cf: CodeFragment α} {i: Raw} (req: i.IsInterior cf)
  : i.IsValid cf := by
  simp only [isInterior_def, IsInteriorIndex_def] at req
  simp only [isValid_def]
  exact req.left

def IsBorder {α} (cf: CodeFragment α) (i: Raw) : Prop := cf.IsBorderIndex i.toNat

theorem isBorder_def {α} (cf: CodeFragment α) (i: Raw)
  : i.IsBorder cf ↔ cf.IsBorderIndex i.toNat := by
  rfl

instance {α} (cf: CodeFragment α) (i: Raw) : Decidable (IsBorder cf i) := decidable_of_iff' _ (isBorder_def _ _)

open CodeFragment in
theorem IsBorder.imp_isValid {α} {cf: CodeFragment α} {i: Raw} (req: i.IsBorder cf)
  : i.IsValid cf := by
  simp [isValid_def, isValidIndex_iff_border_or_interior]
  simp [isBorder_def] at req
  simp [req]


end Divider.Raw

def Divider.Interior {α} (cf: CodeFragment α) := { raw: Divider.Raw // Raw.IsInterior cf raw }


def Divider.Border {α} (cf: CodeFragment α) := { raw: Divider.Raw // Raw.IsBorder cf raw }

namespace Divider.Raw

variable {α} (cf: CodeFragment α)

def toInterior? (raw: Raw) : Option (Interior cf) :=
  if h: Raw.IsInterior cf raw then some ⟨raw, h⟩ else none

theorem toInterior?_ne_none_iff_isInterior (raw: Raw)
  : (toInterior? cf raw ≠ .none) ↔ Raw.IsInterior cf raw := by
  unfold toInterior?
  simp


def toBorder? (raw: Raw) : Option (Border cf) :=
  if h: Raw.IsBorder cf raw then some ⟨raw, h⟩ else none

end Divider.Raw



namespace Divider.Interior

variable {α} (cf: CodeFragment α)

def equiv : Divider.Interior cf ≃ cf.InteriorIndex where
  toFun l := ⟨l.val.toNat, l.property⟩
  invFun r := ⟨⟨r.val⟩, r.property⟩

instance : Fintype (Interior cf) := Fintype.ofEquiv _ (equiv cf).symm

instance instIsEmpty (req: cf.length ≤ 1) : IsEmpty (Interior cf) :=
  Subtype.isEmpty_of_false (by
    intro r
    simp [Raw.isInterior_def]
    exact CodeFragment.length_le_one_imp_not_isInteriorIndex cf r.toNat req
  )

end Divider.Interior



namespace Divider.Border

variable {α} {cf: CodeFragment α}

/-
def finset : Finset (Border cf) where
  val :=
-/

end Divider.Border

open Divider Raw in
def Divider {α} (cf: CodeFragment α) := { raw: Divider.Raw // Raw.IsValid cf raw }

namespace Divider

set_option trace.Meta.synthInstance true

variable {α} {cf: CodeFragment α}

instance : LinearOrder (Divider.Interior cf) := inferInstanceAs (LinearOrder (Subtype _))

--instance : DecidableEq (Divider cf) := Equiv.decidableEq (equiv cf)

instance : LinearOrder (Divider cf) := inferInstanceAs (LinearOrder (Subtype _))

--theorem le_iff_raw_le (da db: Divider cf) : (da ≤ db) ↔ (da.raw ≤ db.raw) := by rfl

end Divider

@[ext]
structure DividerList.Raw where
  ofList :: toList: List Divider.Raw
  deriving Repr, DecidableEq

namespace DividerList.Raw

instance : Coe (List Divider.Raw) (DividerList.Raw) where
  coe l := Raw.ofList l

def equiv : Raw ≃ List Divider.Raw where
  toFun := Raw.toList
  invFun := Raw.ofList

theorem equiv_coe_eq : ⇑equiv = Raw.toList := by rfl

theorem equiv_symm_coe_eq : ⇑equiv.symm = Raw.ofList := by rfl

instance : Membership Divider.Raw Raw where
  mem cont elem := elem ∈ cont.toList

def length (rs: Raw) : Nat := rs.toList.length

@[simp]
theorem length_def (rs: Raw) : rs.length = rs.toList.length := by rfl

@[simp]
theorem mem_def {cont: Raw} {elem: Divider.Raw} : elem ∈ cont ↔ elem ∈ cont.toList := by rfl

@[mk_iff]
structure IsValid {α} (cf: CodeFragment α) (r: Raw) : Prop where
  for_all: ∀x ∈ r, Divider.Raw.IsValid cf x
  pairwise: r.toList.Pairwise (· < ·)

@[mk_iff]
structure IsInterior {α} (cf: CodeFragment α) (r: Raw) : Prop where
  all_interior: ∀x ∈ r, Divider.Raw.IsInterior cf x
  strict_lt: r.toList.SortedLT

theorem IsInterior.nil {α} (cf: CodeFragment α) : IsInterior cf ⟨[]⟩ := by
    simp [Raw.isInterior_iff]
    simp [List.sortedLT_iff_pairwise]

end DividerList.Raw

open DividerList in
def DividerList.Interior {α} (cf: CodeFragment α) := { r: Raw // Raw.IsInterior cf r }

namespace DividerList.Interior

variable {α} {cf: CodeFragment α}

protected abbrev mk (cf: CodeFragment α) r (p: Raw.IsInterior cf r) : Interior cf := ⟨r, p⟩

instance : DecidableEq (Interior cf) := inferInstanceAs (DecidableEq (Subtype _))

def empty : Interior cf where
  val := ([] : List Divider.Raw)
  property := Raw.IsInterior.nil cf

instance : EmptyCollection (Interior cf) := ⟨empty⟩

instance : Nonempty (Interior cf) := ⟨∅⟩

--@[simp]
--theorem emptyCollection_def : (∅: Interior cf) = empty := by rfl

--theorem empty_val_def : (∅: Interior cf).val = ([] : List Divider.Raw) := by rfl

theorem empty_def : (∅: Interior cf) = ⟨⟨[]⟩, Raw.IsInterior.nil cf⟩ := by rfl

variable (r: Divider.Raw) (rs: Interior cf)

/-- expensive -/
protected def insert : Interior cf :=
  match h1: r.toInterior? cf with
  | .none => rs
  | .some d =>
  if h2: d.val ∈ rs.val.toList then rs else
  { val := rs.val.toList.orderedInsert (· ≤ ·) d.val
    property := by
      simp [Raw.isInterior_iff]
      constructor <;> try constructor
      · simp [Divider.Raw.toInterior?.eq_def] at h1
        obtain ⟨h1_w, h1_p⟩ := h1
        rw [← h1_p]
        simpa using h1_w
      · exact rs.property.all_interior
      · rcases rs with ⟨⟨rs_v⟩, all_interior, strict_lt⟩
        simp [List.sortedLT_iff_nodup_and_sortedLE]
        have lm1 := strict_lt.nodup
        cases rs_v with
        | nil => simp [List.sortedLE_iff_pairwise]
        | cons rs_hd rs_tl =>
          simp
          by_cases h3: d.val ≤ rs_hd
          · simp at h2
            simp at lm1
            simp [h3]; simp [h2, lm1]
            simp [List.sortedLT_iff_nodup_and_sortedLE, lm1] at strict_lt
            revert strict_lt
            simp [List.sortedLE_iff_isChain, h3]
          · simp [h3]
            simp at h2
            obtain ⟨lm2, lm3⟩ := h2
            simp at h3
            constructorm* _ ∧ _
            · intro cont; exact lm2 cont.symm
            · intro cont
              simp [List.nodup_iff_pairwise_ne] at lm1
              exact lm1.1 _ cont (Eq.refl _)
            · simp at lm1
              exact lm1.2.orderedInsert_nodup (· ≤ ·) d.val rs_tl lm3
            · simp [List.sortedLT_iff_nodup_and_sortedLE, lm1] at strict_lt
              simp [List.sortedLE_iff_pairwise]
              simp [List.sortedLE_iff_pairwise] at strict_lt
              obtain ⟨strict_lt_1, strict_lt_2⟩ := strict_lt
              constructorm* _ ∧ _
              · exact le_of_lt h3
              · exact strict_lt_1
              · apply List.SortedLE.pairwise
                simp [← List.sortedLE_iff_pairwise] at strict_lt_2
                exact strict_lt_2.orderedInsert_sortedLE d.val rs_tl
  }

inductive InsertIsConst : Prop where
  | not_interior (h: ¬Divider.Raw.IsInterior cf r)
  | is_mem (h: r ∈ rs.val.toList)

theorem insertIsConst_iff_or
  : InsertIsConst r rs ↔ (¬Divider.Raw.IsInterior cf r ∨ r ∈ rs.val.toList) := by
  constructor <;> (intro h1; cases h1 <;> rename_i h2)
  · simp [h2]
  · simp [h2]
  · exact InsertIsConst.not_interior h2
  · exact InsertIsConst.is_mem h2

instance : Insert (Divider.Raw) (Interior cf) where
  insert := Interior.insert

theorem insert_def : insert r rs = Interior.insert r rs := by
  rfl

theorem insert_const_or_cons : (insert r rs = rs) ∨ ((insert r rs).val.length = rs.val.length + 1) := by
  have (eq := lm1) rs0 := insert r rs
  rw [← lm1]
  simp [insert_def, Interior.insert.eq_def] at lm1
  split at lm1
  · left; exact lm1
  · split at lm1
    · left; exact lm1
    · right
      rcases rs0 with ⟨⟨rs0_v⟩, _⟩
      simp [Interior] at lm1
      simp
      replace lm1 := congrArg List.length lm1
      simpa [List.orderedInsert_length] using lm1

theorem insert_const_cons_disjoint : ¬((insert r rs = rs) ∧ ((insert r rs).val.length = rs.val.length + 1)) := by
  simp; intro lm1; simp [lm1]


theorem insertIsConst_iff_insert_eq : InsertIsConst r rs ↔ (insert r rs = rs) := by
  simp [insert_def, Interior.insert.eq_def]
  split <;> rename_i lm1
  · rewrite [(Divider.Raw.toInterior?_ne_none_iff_isInterior cf r).not_right] at lm1
    simp
    exact InsertIsConst.not_interior lm1
  · split
    · rename_i lm2; simp
      simp [Divider.Raw.toInterior?.eq_def] at lm1
      obtain ⟨lm3, lm4⟩ := lm1
      simp only [← lm4] at lm2
      exact InsertIsConst.is_mem lm2
    · rename_i lm2
      simp [Divider.Raw.toInterior?.eq_def] at lm1
      obtain ⟨lm3, lm4⟩ := lm1
      replace lm4 := lm4.symm
      subst lm4; simp at lm2
      have lm6: ¬InsertIsConst r rs := by
        intro cont
        cases cont <;> rename_i lm6_1 <;> contradiction
      simp [lm6]
      intro cont
      rcases rs with ⟨⟨rs_v⟩, rs_p⟩
      simp [Interior, Subtype.ext_iff] at cont
      have lm7 := congrArg List.length cont
      simp [List.orderedInsert_length] at lm7


protected def cons (_: ¬InsertIsConst r rs) : Interior cf := insert r rs

theorem cons_def (req: ¬InsertIsConst r rs)
  : Interior.cons r rs req = insert r rs := by
  rfl


theorem cons_length (req: ¬InsertIsConst r rs)
  : (Interior.cons r rs req).val.length = rs.val.length + 1 := by
  simp only [insertIsConst_iff_insert_eq] at req
  have lm1 := insert_const_or_cons r rs
  simpa only [req, false_or] using lm1

/-
protected def singleton : Interior cf := Interior.cons r ∅ (by
    intro cont
    cases cont
  )
-/



def uncheckedCons (elem: Divider.Raw) (coll: DividerList.Interior cf) : DividerList.Raw := coll.val.toList.orderedInsert (· ≤ ·) elem

def IsCons (elem: Divider.Raw) (coll: DividerList.Interior cf) : Prop := (uncheckedCons elem coll).IsInterior cf

theorem isCons_def {elem} {coll: DividerList.Interior cf}
  : IsCons elem coll ↔ (uncheckedCons elem coll).IsInterior cf := by rfl

theorem empty_iff_ne_nil (ds: Interior cf) : (ds ≠ ∅) ↔ (ds.val.toList ≠ []) := by
  conv => lhs; arg 2; change empty
  contrapose
  simp [empty]
  revert ds
  simp [Interior]
  intro _ _
  simp [← Raw.equiv_coe_eq, ← Raw.equiv_symm_coe_eq]
  simp [Raw.equiv.apply_eq_iff_eq_symm_apply]

theorem cons_imp_ne_empty (r: Divider.Raw) (rs: Raw) (req: Raw.IsInterior cf (r :: rs.toList))
  : (Interior.mk cf (r :: rs.toList) req) ≠ ∅ := by
  unfold Interior.mk
  simp [empty_def]
  intro cont
  have lm1 := Subtype.ext_iff.mp cont
  simp at lm1


protected def head (ds: Interior cf) (req: ds ≠ ∅) : Divider.Interior cf :=
  let rs := ds.val.toList
  let headR := rs.head (ds.empty_iff_ne_nil.mp req)
  have headR_spec: headR.IsInterior cf := by
    have lm1 := ds.property.all_interior headR
    subst rs
    subst headR
    simpa using lm1
  ⟨headR, headR_spec⟩

theorem head_cons (r: Divider.Raw) (rs: Raw) (req: Raw.IsInterior cf (r :: rs.toList))
  : ∃(th: r.IsInterior cf), (Interior.mk cf (r :: rs.toList) req).head (cons_imp_ne_empty r rs req) = ⟨r, th⟩ := by
  obtain ⟨lm1, lm2⟩ := by have lm1 := req.all_interior; simp at lm1; exact lm1
  exists lm1

protected def tail (ds: Interior cf) : Interior cf :=
  let tailR := ds.val.toList.tail
  have tailR_spec: Raw.IsInterior cf tailR := by
    have ⟨lm1, lm2⟩ := ds.property
    subst tailR
    constructor
    · simp
      simp at lm1
      intro e ep
      exact lm1 e (List.mem_of_mem_tail ep)
    · revert lm2
      cases ds.val.toList
      · simp only [List.tail_nil]; exact id
      · simp only [List.sortedLT_iff_pairwise]
        simp only [List.pairwise_cons]
        simp only [List.tail_cons]
        exact (fun ⟨x, y⟩ => y)
  ⟨tailR, tailR_spec⟩

protected theorem tail_empty : Interior.tail (∅: Interior cf) = ∅ := by
  simp [emptyCollection_def]
  unfold Interior.tail
  rfl

theorem tail_cons (r: Divider.Raw) (rs: DividerList.Raw) (req: Raw.IsInterior cf (r :: rs.toList))
  : ∃(th: Raw.IsInterior cf rs), (Interior.tail ⟨r :: rs.toList, req⟩) = ⟨rs, th⟩ := by
  unfold Interior.tail
  extract_lets tailR tailR_spec
  subst tailR
  simp at tailR_spec
  exists tailR_spec

theorem tail_length_succ_eq (ds: Interior cf) (req: ds ≠ ∅) : ds.tail.val.length + 1 = ds.val.length := by
  simp
  unfold Interior.tail
  extract_lets tailR tailR_spec
  subst tailR
  simp
  have lm1 := (empty_iff_ne_nil _).mp req |> List.length_pos_of_ne_nil
  exact Nat.sub_one_add_one_eq_of_pos lm1


theorem tail_length_lt (ds: Interior cf) (req: ds ≠ ∅) : ds.tail.val.length < ds.val.length := by
  have lm1 := ds.tail_length_succ_eq req
  calc
    _ = _ := lm1.symm
    _ > _ := by simp only [Nat.lt_add_one]


set_option trace.Meta.synthInstance true in
def dropWhileLt (r: Divider.Raw) (rs: Interior cf) : Interior cf :=
  if h1: rs = ∅ then ∅ else
  let head0 := rs.head h1
  if head0.val < r then dropWhileLt r rs.tail else rs
  termination_by rs.val.length
  decreasing_by
    apply tail_length_lt
    exact h1

theorem dropWhileLt_head_ge_ne_empty
  (r: Divider.Raw) (rs: Interior cf) (req1: rs ≠ ∅) (req2: (rs.head req1).val ≥ r)
  : dropWhileLt r rs ≠ ∅ := by
  unfold dropWhileLt
  simp [req1]
  revert req2
  simp only [Interior.head.eq_def]
  intro req2
  simpa [req2, not_lt_of_ge] using req1


inductive IsConsInterior (r: Divider.Interior cf) : Interior cf → Prop where
  | nil: IsConsInterior r ∅
  | cons
      (r1: Divider.Raw)
      (rs1: DividerList.Raw)
      (req1: Raw.IsInterior cf (r1 :: rs1.toList))
      (req2: r.val < r1)
      : IsConsInterior r ⟨r1 :: rs1.toList, req1⟩

instance {r: Divider.Interior cf} {rs: Interior cf} : Decidable (IsConsInterior r rs) :=
  if h1: rs = ∅ then
    .isTrue (h1.symm ▸ IsConsInterior.nil)
  else if h2: r.val < (rs.head h1).val then
    match h3: rs.val.toList with
    | [] => h1 |> absurd (by
      apply Subtype.val_injective;
      apply Raw.equiv.injective;
      simpa only [Raw.equiv_coe_eq, empty_val_def] using h3)
    | r1::rs1 =>
      .isTrue (by
        have lm1 := @IsConsInterior.cons _ cf r r1 rs1 (by simp [← h3]; exact rs.property) (by
          simp only [Subtype.coe_lt_coe] at h2
          unfold Interior.head at h2
          extract_lets rs_1 headR headR_spec at h2
          subst rs_1
          subst headR
          revert headR_spec h2
          simp only [h3, List.head_cons]
          intro h2
          simpa only [← Subtype.coe_lt_coe] using h2
        )
        simp at lm1
        have lm4 := congrArg Raw.ofList h3
        simp only [← lm4] at lm1
        exact lm1
      )
  else
    .isFalse (by
      intro cont
      cases cont
      · simp only [not_true_eq_false] at h1
      · rename_i _ _ _ h3
        unfold Interior.head at h2
        simp at h2
        have lm1 : _ < _ := calc
          _ ≤ _ := h2
          _ < _ := h3
        simp only [lt_self_iff_false] at lm1
    )

instance {r: Divider.Raw} {rs: Interior cf} : Decidable (∃(req: r.IsInterior cf), IsConsInterior ⟨r, req⟩ rs) :=
  if h1: r.IsInterior cf then
    if h2: IsConsInterior ⟨r, h1⟩ rs then
      .isTrue (by exists h1)
    else
      .isFalse (by simp [h2])
  else
    .isFalse (by simp [h1])


--set_option pp.explicit true in
theorem isCons_iff_isConsInterior_dropWhileLt
  (r: Divider.Raw) (rs: Interior cf) (req: r.IsInterior cf)
  : IsCons r rs ↔ IsConsInterior ⟨r, req⟩ (rs.dropWhileLt r) := by
  simp [isCons_def]
  --unfold uncheckedCons
  let (eq := lm1) ⟨⟨rs_v⟩, rs_all, rs_st⟩ := rs
  let (eq := lm2) ⟨⟨rsd_v⟩, rsd_all, rsd_st⟩ := rs.dropWhileLt r
  cases rs_v --<;> cases rsd_v
  · unfold uncheckedCons; unfold dropWhileLt; simp [empty_def]
    simp [Raw.isInterior_iff, req, List.sortedLT_iff_pairwise]
    exact IsConsInterior.nil
  next rs_hd rs_tl =>
    cases rsd_v
    · by_cases lm3: r ≤ rs_hd
      · have lm4 := dropWhileLt_head_ge_ne_empty r rs (by rw [empty_def]; simp [lm1, Interior.eq_1])
                    (by simp [Interior.head.eq_def]; subst lm1; simp [lm3])
        rewrite [lm2, empty_def] at lm4
        simp at lm4 --contradiction
      · rw [uncheckedCons.eq_def, ← lm1, lm2, ← empty_def]
        simp [IsConsInterior.nil]; clear lm2
        subst lm1
        simp [Raw.isInterior_iff, lm3, rs_all, req]
        constructor; · simp at rs_all; exact rs_all.2
        simp at lm3
        --simp [List.sortedLT_iff_isChain]
        --simp only [List.isChain_cons]
        --simp only [← List.sortedLT_iff_isChain]
        --revert rs_st
        simp [List.sortedLT_iff_pairwise]
        simp [lm3]
        simp only [← List.sortedLT_iff_pairwise]
        constructor; · simp [List.sortedLT_iff_pairwise] at rs_st; exact rs_st.1
        simp [List.sortedLT_iff_nodup_and_sortedLE]
        constructor
        case neg.right.right.right =>
          simp [List.sortedLE_iff_pairwise]
          apply List.Pairwise.orderedInsert
          simp [List.sortedLT_iff_nodup_and_sortedLE, List.sortedLE_iff_pairwise] at rs_st
          exact rs_st.2.2
        · have lm5 := List.sortedLT_iff_nodup_and_sortedLE.mp rs_st; simp at lm5
        --simp only [List.sortedLT_iff_nodup_and_sortedLE] at rs_st
/-
        simp [List.sortedLT_iff_pairwise] at rs_st
        constructor; · exact rs_st.1
        simp
-/
/-
        intro lm4 lm5
        constructor
        · intro rX lm6
          specialize lm4 rX
          simp at lm3
-/
/-
          cases rs_tl
          · simp at lm6; simpa [lm6] using lm3
          next hd2 tl2 =>
            simp at lm4 lm6
            by_cases lm7: hd2 = rX
            · exact lm4 lm7
            ·
-/
          --apply lm4
          --rw [← lm6]

        --intro rs_st
        --have := List.isChain_cons

      --rewrite [lm1] at lm2; rw [lm2]; rewrite [← lm1] at lm2
      --simp only [← empty_def, IsConsInterior.nil, iff_true]
      --unfold uncheckedCons; simp only [List.orderedInsert_cons]
/-
      cases lm3: compare r rs_hd
      case eq =>
        simp only [compare_eq_iff_eq] at lm3
        have lm4 := dropWhileLt_eq_head_ne_empty r rs (by rw [empty_def]; simp [lm1, Interior.eq_1])
                      (by simp [Interior.head.eq_def]; subst lm3; subst lm1; simp)
        rewrite [lm2, empty_def] at lm4
        simp at lm4
      case lt =>
        simp only [compare_lt_iff_lt] at lm3
        simp [le_of_lt lm3]
        simp [Raw.isInterior_iff, req, rs_all];
        constructor; · simp at rs_all; exact rs_all.2
        revert rs_st
        simp [List.sortedLT_iff_isChain, lm3]
        exact (fun x _ => x)
      case gt =>
        simp only [compare_gt_iff_gt] at lm3
        simp [not_le_of_gt lm3]
        simp [Raw.isInterior_iff, req, rs_all];
        constructor; · simp at rs_all; exact rs_all.2
        revert rs_st
        simp [List.sortedLT_iff_isChain]
        intro rs_st lm1
        simp
-/





      --· simp [le_of_lt lm3]
/-
      by_cases h1: r ≤ rs_hd
      · simp [h1]
        revert rs_all
        simp
        intro rs_all1 rs_all2 lm1
        simp [Raw.isInterior_iff, req, rs_all1]
        apply (fun x => And.intro rs_all2 x)
        revert rs_st
        simp [List.sortedLT_iff_isChain]
        intro rs_st lm1; simp [rs_st]
        by_cases h2: r ≠ rs_hd
        · have lm3 := lt_or_eq_of_le h1
          simpa [h2] using lm3
        · simp at h2
          have lm3 := rs.property
          subst lm1
          unfold dropWhileLt at lm2
          simp [empty_def] at lm2
          obtain ⟨_, lm4⟩ := head_cons rs_hd ⟨rs_tl⟩ lm3
          simp [Interior.mk] at lm4
          simp [lm4] at lm2
          simp [h2] at lm2
          simp [Interior] at lm2 --contradiction
      · simp [h1]; simp at h1
        simp [Raw.isInterior_iff]
-/
    --cases rsd_v
    --· simp
  --cases lm1: rs.val.toList

/-
  · simp
    rw [dropWhileLt.eq_def]
    have lm2 : rs = ∅ := by
      apply Subtype.val_injective
      apply Raw.equiv.injective
      rw [Raw.equiv_coe_eq]
      simpa [empty_val_def] using lm1
    simp [lm2]
    simp [Raw.isInterior_iff]
    have lm3 : IsConsInterior ⟨r, req⟩ ∅ := IsConsInterior.nil
    simp at lm3
    simp [lm3, req]
    simp [List.sortedLT_iff_pairwise]
  · rename_i head tail
    rcases lm2: rs with ⟨rs_v, all_interior, strict_lt⟩
    simp
-/
    --simp only [← lm1]
/-
    simp
    rw [dropWhileLt.eq_def]
    simp
    have lm3 := Raw.equiv.symm.congr_arg lm1
    conv at lm3 =>
      simp only [← Raw.equiv_coe_eq]
      simp [Raw.equiv_symm_coe_eq]
    have lm4 : Raw.IsInterior cf (head :: tail) := by simpa [lm3] using rs.property
    have lm5 : rs ≠ ∅ := by
      have lm5_1 := cons_imp_ne_empty head ⟨tail⟩ lm4
      simpa [Interior.mk, ← lm3] using lm5_1
    simp [lm5]
    obtain ⟨lm6, lm7⟩ := head_cons head ⟨tail⟩ lm4
    simp [Interior.mk] at lm7
    have lm8 : rs.head lm5 = ⟨head, lm6⟩ := by
      symm
      calc
        _ = _ := lm7.symm
        _ = rs.head lm5 := by simp [← lm3]
    simp [lm8]
    obtain ⟨lm9, lm10⟩ := Interior.tail_cons head ⟨tail⟩ lm4
    simp only at lm10
    have lm11 : rs.tail = ⟨⟨tail⟩, lm9⟩ := by
      unfold Interior.tail
      simp [lm1]
    simp [lm11]
    rcases rs with ⟨rs, rs_p⟩
    subst_eqs; clear lm4
    have lm12: (r ≤ head) ↔ ¬(r > head) := by simp
    by_cases h1: r ≤ head
    · simp [h1]
      simp [lm12.mp h1]
      constructor
      · intro h2
        exact IsConsInterior.cons head ⟨tail⟩ rs_p (by
          simp
          have lm13 := h2.strict_lt.pairwise
          simp at lm13
          exact lm13.1.1)
      · intro h2
        cases h2
        rename_i rs1 req1 req2; clear req1; simp at req2
        simp [Raw.isInterior_iff]
        simp [req, lm6]
        constructor
        · simpa using lm9.all_interior
        · have lm13 := rs_p.strict_lt
          revert lm13
          simp [List.sortedLT_iff_isChain]
          simp [req2]
    · simp [h1, not_le.mp h1]
      conv => lhs; arg 2; arg 1; rw [← List.orderedInsert_of_not_le _ _ h1]
      simp
      --simp at h1
      --simp [h1]
      --unfold dropWhileLt
      --simp [empty_def]
-/
/-
      match h3: tail with
      | .nil =>
        subst h3
        simp [Raw.isInterior_iff, List.sortedLT_iff_pairwise, req, lm6, h1]
        exact IsConsInterior.nil
      | .cons _ _ =>
        subst h3
        simp
-/
/-
      by_cases h3: tail = []
      · subst h3
        simp [empty_def]
        simp [Raw.isInterior_iff, List.sortedLT_iff_pairwise, req, lm6, h1]
        exact IsConsInterior.nil
      · simp [empty_def]
        conv => rhs; arg 2; arg @3
-/
      --have lm13 := isCons_iff_isConsInterior_dropWhileLt head ⟨⟨tail⟩, lm9⟩ lm6
      --revert req
      --simp [Divider.Raw.isInterior_def, CodeFragment.IsInteriorIndex_iff_ioo]
      --intro req_1 req_2
      --simp [isCons_def, uncheckedCons] at lm13
      --let ordIso := Divider.Raw.orderIso
      --have := ordIso.lt
      --simp [Divider.Raw.isInterior_def, CodeFragment.IsInteriorIndex_iff_ioo] at req




      --subst_eqs
      --have lm12 : ¬(r > head) := by simp [h1]
      --simp [lm12]

    --have lm9 := rs.tail = Interior.mk cf ⟨tail⟩
    --have lm2 : rs = _ := by

    --have lm2 : rs = _ :=
    --  lm1 |> Raw.equiv.symm.congr_arg

      --Raw.equiv.apply_eq_iff_eq_symm_apply.mp lm1

    --have lm2 : rs = _ := by
      --simp [Raw.equiv.]


/-
theorem isCons_iff {r: Divider.Raw} {rs: Interior cf}
  : IsCons r rs ↔ ∃(req: r.IsInterior cf), IsConsInterior ⟨r, req⟩ (rs.dropWhileLt r) := by
  constructor
  · revert rs
    simp [Interior]
    rintro rs ⟨all_interior, strict_lt⟩ h1
    conv at h1 =>
      simp [isCons_def]
      unfold uncheckedCons
      tactic =>
        simp
        cases lm1: rs.toList
        · simp; rw [← lm1]
        · rename_i head tail
          revert strict_lt
          simp [lm1]
          intro strict_lt h1
          have ⟨lm2, lm3⟩ := strict_lt.pairwise |> List.pairwise_cons.mp
          --have lm2: r < head := strict_lt.
          --simp [lm1] at strict_lt
-/







/-
protected def drop (n: Nat) (ds: Interior cf) : Interior cf :=
  match n with
  | 0 => ds
  | np+1 => if ds = ∅ then ∅ else ds.tail.drop np
-/





end DividerList.Interior

open DividerList Raw in
@[ext]
structure DividerList {α} (cf: CodeFragment α) where
  raw: DividerList.Raw
  is_valid: IsValid cf raw
  deriving DecidableEq

namespace DividerList

variable {α} (cf: CodeFragment α)

def equiv : DividerList cf ≃ { raw: DividerList.Raw // Raw.IsValid cf raw } where
  toFun d := ⟨d.raw, d.is_valid⟩
  invFun st := ⟨st.val, st.property⟩

@[match_pattern]
protected def nil : DividerList cf where
  raw := Raw.ofList []
  is_valid := by simp [Raw.isValid_iff]

protected def head? (l: DividerList cf) : Option (Divider cf) :=
  match h: l.raw.toList with
  | [] => .none
  | hdR::_ => .some ⟨hdR, by
    have lm1 := l.is_valid.for_all
    specialize lm1 hdR
    simpa [h] using lm1
  ⟩


protected theorem head?_eq_none_iff (l: DividerList cf)
  : (l.head? = .none) ↔ (l = .nil cf) := by
  cases l
  rename_i raw is_valid
  cases raw
  rename_i toList
  simp [DividerList.head?, DividerList.nil]
  induction toList with
  | nil => simp
  | cons _ _ _ => simp

--set_option pp.explicit true in
attribute [-simp] List.pairwise_cons in
theorem head?_cons_is_valid (l: DividerList cf) (head: Divider cf)
  : (∀head1 ∈ l.head?, head ≤ head1) → DividerList.Raw.IsValid cf (head.raw :: l.raw.toList) := by
  simp only [Option.mem_def]
  intro head1
  simp [Raw.isValid_iff]
  simp [head.is_valid]
  match lm1: l.raw.toList with
  | .nil => simp [List.pairwise_cons]
  | .cons hd1R tl1R =>
    constructor
    · have lm2 := l.is_valid.for_all;
      simp [lm1] at lm2
      simpa using lm2
    · simp only [List.pairwise_cons_cons_iff_of_trans]
      have (eq := lm2) hd1: Divider cf := ⟨hd1R, by
          have lm2 := l.is_valid.for_all; simp [lm1] at lm2; exact lm2.1
      ⟩
      specialize head1 hd1 (by
        simp [DividerList.head?]
        rcases l with ⟨raw, _⟩
        rcases raw with ⟨toList⟩
        match lm3: toList with
        | .nil => simp at lm1
        | .cons _ _ =>
          simp
          subst lm2
          simp
          simp at lm1
          exact lm1.1
      )
      have lm3 := l.is_valid.pairwise
      simp [lm1] at lm3
      simp [lm3]
      subst lm2
      simpa [Divider.le_iff_raw_le] using head1

@[match_pattern]
protected def cons
  (head: Divider cf) (tail: DividerList cf)
  (req: (∀head1 ∈ tail.head?, head ≤ head1)) : DividerList cf where
  raw := Raw.ofList (head.raw :: tail.raw.toList)
  is_valid := tail.head?_cons_is_valid cf head req

end DividerList

/-!
3. It doesn't just recognize the substrings.
4. It also needs to classify the different elements of the string according to their role.
5. We call these token classes.
-/

structure TokenRange.Raw where
  lower: Nat
  upper: Nat

namespace TokenRange.Raw

variable {α} (cf: CodeFragment α)





end TokenRange.Raw



end LexicalAnalysis

end Nemonuri.Study.StanfordOnline.Compilers

end public_s
