module

--public import Nemonuri.Study.StanfordOnline.Compilers.StructureOfCompiler.LexicalAnalysis
public import Nemonuri.Study.StanfordOnline.Compilers.Lemma
public import Mathlib.Logic.Equiv.Defs
--public import Mathlib.Logic.Encodable.Basic
public import Mathlib.Order.Lattice

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

protected def length (cf: CodeFragment α) : Nat := cf.toList.length --List.length (equiv _ cf)

section Index

variable (cf: CodeFragment α) (i: Nat)

def IsTextIndex : Prop := i < cf.length

instance : Decidable (cf.IsTextIndex i) := inferInstanceAs (Decidable (_ < _))

def IsEofIndex : Prop := i = cf.length

instance : Decidable (cf.IsEofIndex i) := inferInstanceAs (Decidable (_ = _))

def IsValidIndex : Prop := cf.IsTextIndex i ∨ cf.IsEofIndex i

instance : Decidable (cf.IsValidIndex i) := inferInstanceAs (Decidable (_ ∨ _))

theorem isValidIndex_iff
  : cf.IsValidIndex i ↔ (cf.IsTextIndex i ∨ cf.IsEofIndex i) := by
  rfl

theorem isValidIndex_iff_le_length
  : cf.IsValidIndex i ↔ i ≤ cf.length := by
  simp [IsValidIndex, IsTextIndex, IsEofIndex]
  omega

end Index


section Index.Subtype

variable (cf: CodeFragment α)

def TextIndex := { i: Nat // cf.IsTextIndex i }

def EofIndex := { i: Nat // cf.IsEofIndex i }

instance : Unique cf.EofIndex where
  default := ⟨cf.length, by rfl⟩
  uniq := by simp [EofIndex, IsEofIndex]


def ValidIndex := { i: Nat // cf.IsValidIndex i }

end Index.Subtype


namespace ValidIndex


protected def mk (cf: CodeFragment α) (i: Nat) (h: cf.IsValidIndex i) : ValidIndex cf := Subtype.mk i h

protected inductive Sum (cf: CodeFragment α) where
  | text (i: cf.TextIndex)
  | eof (i: cf.EofIndex)

namespace Sum

protected def val {cf: CodeFragment α} (v: ValidIndex.Sum cf) : Nat :=
  match v with
  | .text s => s.val
  | .eof s => s.val

protected def ext_iff {cf: CodeFragment α} (v1 v2: ValidIndex.Sum cf)
  : (v1 = v2) ↔ (v1.val = v2.val) := by
  constructor
  · cases v1 <;> cases v2 <;> simp_all
  · match v1, v2 with
    | .text ⟨i1, h1⟩, .text ⟨i2, h2⟩ =>
      simp [Sum.val]
      intro h3; simp [h3]
    | .eof ⟨i1, h1⟩, .eof ⟨i2, h2⟩ =>
      simp [Sum.val]
      intro h3; simp [h3]
    | .text ⟨i1, h1⟩, .eof ⟨i2, h2⟩
    | .eof ⟨i1, h1⟩, .text ⟨i2, h2⟩ =>
      revert h1 h2
      simp [IsTextIndex, IsEofIndex, Sum.val]
      omega

end Sum

def toSum {cf: CodeFragment α} (v: ValidIndex cf) : ValidIndex.Sum cf :=
  let ⟨i, h1⟩ := v
  if h2: cf.IsTextIndex i then
    .text ⟨i, h2⟩
  else
    .eof ⟨i, by have lm1 := (cf.isValidIndex_iff i).mp h1; simp_all only [false_or]⟩

@[simp]
theorem toSum_val_eq {cf: CodeFragment α} (v: ValidIndex cf) : v.toSum.val = v.val := by
  obtain ⟨val, val_p⟩ := v
  simp [toSum]
  by_cases h2: cf.IsTextIndex val <;> (simp [h2]; rfl)


def ofSum {cf: CodeFragment α} (v: ValidIndex.Sum cf) : ValidIndex cf where
  val := v.val
  property := by
    cases v <;> simp [Sum.val, isValidIndex_iff] <;> rename_i i
    · exact (Or.inl i.property)
    · exact (Or.inr i.property)

@[simp]
theorem ofSum_val_eq {cf: CodeFragment α} (v: ValidIndex.Sum cf)
  : (ofSum v).val = v.val := by
  rfl


def equiv (cf: CodeFragment α) : ValidIndex cf ≃ ValidIndex.Sum cf where
  toFun v := toSum v
  invFun v := ofSum v
  left_inv := by
    intro v
    simp [ValidIndex, Subtype.ext_iff]
  right_inv := by
    intro v
    simp [Sum.ext_iff]


end ValidIndex



protected def get [Eof α] (cf: CodeFragment α) (i: Nat) (req: IsValidIndex cf i) : α :=
  match (ValidIndex.mk cf i req).toSum with
  | .text ⟨i1, h1⟩ => cf.toList[i1]
  | .eof ⟨i1, h1⟩ => Eof.eof


instance [Eof α] : GetElem (CodeFragment α) Nat α IsValidIndex where
  getElem := CodeFragment.get


end CodeFragment


structure Divider.Raw where
  idx: Nat
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
  toFun := Divider.Raw.idx
  invFun := Divider.Raw.mk

--instance : Encodable Divider.Raw := Encodable.ofEquiv Nat equiv
instance : LinearOrder Divider.Raw := Equiv.linearOrder equiv

@[mk_iff]
structure IsValid {α} (cf: CodeFragment α) (r: Raw) : Prop where
  le_length : r.idx ≤ cf.length

end Divider.Raw

open Divider Raw in
@[ext]
structure Divider {α} (cf: CodeFragment α) where
  raw: Raw
  is_valid: IsValid cf raw
  --deriving DecidableEq

namespace Divider

set_option trace.Meta.synthInstance true

def equiv {α} (cf: CodeFragment α) : Divider cf ≃ { raw: Divider.Raw // Raw.IsValid cf raw } where
  toFun d := ⟨d.raw, d.is_valid⟩
  invFun st := ⟨st.val, st.property⟩

variable {α} {cf: CodeFragment α}

instance : DecidableEq (Divider cf) := Equiv.decidableEq (equiv cf)

instance : LinearOrder (Divider cf) := Equiv.linearOrder (equiv cf)

theorem le_iff_raw_le (da db: Divider cf) : (da ≤ db) ↔ (da.raw ≤ db.raw) := by rfl

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
  pairwise: r.toList.Pairwise (· ≤ ·)

end DividerList.Raw

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
