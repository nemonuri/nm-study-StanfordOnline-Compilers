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


instance : Decidable (cf.IsInteriorIndex i) := decidable_of_iff' (0 < i ∧ i < cf.length) (cf.IsInteriorIndex_iff_ioo i)

theorem isValidIndex_iff_border_or_interior : cf.IsValidIndex i ↔ (cf.IsBorderIndex i ∨ cf.IsInteriorIndex i) := by
  simp [isValidIndex_iff_le_length, isBorderIndex_def, IsInteriorIndex_iff_ioo]
  omega

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
  rw [Finset.disjoint_iff_ne]
  unfold toValidIndexFinset
  simp
  intro bi ii
  suffices goal: bi.val ≠ ii.val from by
    simp
    revert goal
    contrapose
    intro lm2
    replace lm2 := congrArg Subtype.val lm2
    simp [BorderIndex.embedToValidIndex, InteriorIndex.embedToValidIndex, Subtype.impEmbedding] at lm2
    change bi.val = ii.val at lm2
    exact lm2
  revert bi ii
  simp [BorderIndex, InteriorIndex]
  simp [isBorderIndex_def]
  simp [IsInteriorIndex_iff_ioo]
  constructor <;> omega

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

instance : LinearOrder Divider.Raw := Equiv.linearOrder equiv

def IsValid {α} (cf: CodeFragment α) (i: Raw) : Prop := cf.IsValidIndex i.toNat

theorem isValid_def {α} (cf: CodeFragment α) (i: Raw)
  : i.IsValid cf ↔ cf.IsValidIndex i.toNat := by
  rfl

def IsInterior {α} (cf: CodeFragment α) (i: Raw) : Prop := cf.IsInteriorIndex i.toNat

theorem isInterior_def {α} (cf: CodeFragment α) (i: Raw)
  : i.IsInterior cf ↔ cf.IsInteriorIndex i.toNat := by
  rfl

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

open CodeFragment in
theorem IsBorder.imp_isValid {α} {cf: CodeFragment α} {i: Raw} (req: i.IsBorder cf)
  : i.IsValid cf := by
  simp [isValid_def, isValidIndex_iff_border_or_interior]
  simp [isBorder_def] at req
  simp [req]


end Divider.Raw

def Divider.Interior {α} (cf: CodeFragment α) := { raw: Divider.Raw // Raw.IsInterior cf raw }


def Divider.Border {α} (cf: CodeFragment α) := { raw: Divider.Raw // Raw.IsBorder cf raw }


namespace Divider.Interior

variable {α} (cf: CodeFragment α)

def equiv : Divider.Interior cf ≃ cf.InteriorIndex where
  toFun l := ⟨l.val.toNat, l.property⟩
  invFun r := ⟨⟨r.val⟩, r.property⟩

instance : Fintype (Interior cf) := Fintype.ofEquiv _ (equiv cf).symm

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

instance : Membership Divider.Raw Raw where
  mem cont elem := elem ∈ cont.toList

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

end DividerList.Raw

open DividerList in
def DividerList.Interior {α} (cf: CodeFragment α) := { r: Raw // Raw.IsInterior cf r }

namespace DividerList.Interior

variable {α} {cf: CodeFragment α}

def empty : Interior cf where
  val := ([] : List Divider.Raw)
  property := by
    simp [Raw.isInterior_iff]
    simp [List.sortedLT_iff_pairwise]

instance : EmptyCollection (Interior cf) := ⟨empty⟩

def uncheckedInsert (elem: Divider.Raw) (coll: DividerList.Interior cf) : DividerList.Raw := coll.val.toList.orderedInsert (· ≤ ·) elem

def IsStrictInsert (elem: Divider.Raw) (coll: DividerList.Interior cf) : Prop := (uncheckedInsert elem coll).IsInterior cf

theorem isStrictInsert_def {elem} {coll: DividerList.Interior cf}
  : IsStrictInsert elem coll ↔ (uncheckedInsert elem coll).IsInterior cf := by rfl







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
