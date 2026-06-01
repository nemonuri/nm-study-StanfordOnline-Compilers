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

namespace LexicalAnalysis

structure CodeFragment (α: Type*) where --StructureOfCompiler.LexicalAnalysis.ProgramText α
  ofList :: toList : List α
  deriving Repr, DecidableEq

namespace CodeFragment

def equiv (α: Type*) : CodeFragment α ≃ List α where
  toFun := CodeFragment.toList
  invFun := CodeFragment.ofList

--instance {α} : CoeHead (CodeFragment α) (List α) where
--  coe := equiv α

protected def length {α} (cf: CodeFragment α) : Nat := List.length (equiv _ cf)

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
structure Divider {α} (cf: CodeFragment α) where
  raw: Raw
  is_valid: IsValid cf raw

structure DividerList.Raw where
  ofList :: toList: List Divider.Raw
  deriving Repr, DecidableEq

namespace DividerList.Raw

def equiv : Raw ≃ List Divider.Raw where
  toFun := Raw.toList
  invFun := Raw.ofList

instance : Membership Divider.Raw Raw where
  mem cont elem := elem ∈ cont.toList

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

namespace DividerList

variable {α} (cf: CodeFragment α)

protected def nil : DividerList cf where
  raw := Raw.ofList []
  is_valid := by
    simp [Raw.isValid_iff, Membership.mem]
    intro _ h
    cases h


protected def cons
  (head: Divider cf) (tail: DividerList cf)
  (req: ∀x ∈ tail.raw, head.raw ≤ x) : DividerList cf where
  raw := Raw.ofList (head.raw :: tail.raw.toList)
  is_valid := by
    simp [Raw.isValid_iff, Membership.mem]
    have lm1 := head.is_valid
    have lm2 := tail.is_valid.for_all
    have lm3 := tail.is_valid.pairwise
    constructor <;>

    --constructor <;> try assumption
/-
  is_valid := by
    simp [Raw.isValid_iff]
    have lm1 := head.is_valid
    have lm2 := tail.is_valid.for_all
    have lm3 := tail.is_valid.pairwise
    constructor <;> constructor <;> try assumption
    ·
-/


end DividerList

end LexicalAnalysis

end Nemonuri.Study.StanfordOnline.Compilers

end public_s
