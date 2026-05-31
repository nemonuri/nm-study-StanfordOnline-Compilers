module

public import Nemonuri.Study.StanfordOnline.Compilers.Lemma
public import Mathlib.Logic.Equiv.Defs

@[expose] public section public_s

namespace Nemonuri.Study.StanfordOnline.Compilers

set_option autoImplicit false

/-
#### 2.2. lexical analysis

1. The goal of lexical analysis, then, is to divide the program text into its words, or what we call in compiler speak, the tokens.
-/

namespace LexicalAnalysis

structure ProgramText (α: Type*) where
  ofList :: toList: List α
  deriving Repr, DecidableEq


namespace ProgramText

def equiv {α: Type*} : Equiv (ProgramText α) (List α) where
  toFun := ProgramText.toList
  invFun := ProgramText.ofList

structure Pos.Raw where
  idx: Nat
  deriving Repr, DecidableEq

namespace Pos.Raw

def equiv : Equiv Pos.Raw Nat where
  toFun := Pos.Raw.idx
  invFun := Pos.Raw.mk

instance {n} : OfNat Pos.Raw n where
  ofNat := ⟨n⟩

instance : Inhabited Pos.Raw := ⟨0⟩

def eofPos {α} (s: ProgramText α) : Pos.Raw := s.toList.length |> Pos.Raw.mk

instance : LE Pos.Raw where
  le a b := a.idx ≤ b.idx

@[mk_iff]
structure IsValid {α} (s: ProgramText α) (off: Pos.Raw) : Prop where
  le_eofPos : off ≤ eofPos s

end Pos.Raw

@[ext]
structure Pos {α} (s: ProgramText α) where
  offset: Pos.Raw
  is_valid: offset.IsValid s
  deriving DecidableEq

instance {α} {s: ProgramText α} : LE (Pos s) where
  le a b := a.offset ≤ b.offset

structure Range.Raw where
  lower: Pos.Raw
  upper: Pos.Raw
  deriving DecidableEq

namespace Range.Raw

def toRco (r: Range.Raw) : Std.Rco Pos.Raw := ⟨r.lower, r.upper⟩

def ofRco (r: Std.Rco Pos.Raw) : Range.Raw := ⟨r.lower, r.upper⟩

def equiv : Equiv (Range.Raw) (Std.Rco Pos.Raw) where
  toFun := toRco
  invFun := ofRco

@[mk_iff]
structure IsValid {α} (s: ProgramText α) (r: Range.Raw) : Prop where
  lower_is_valid: r.lower.IsValid s
  upper_is_valid: r.upper.IsValid s
  lower_le_upper: r.lower ≤ r.upper

end Range.Raw

@[ext]
structure Range {α} (s: ProgramText α) where
  raw : Range.Raw
  is_valid: raw.IsValid s
  deriving DecidableEq


namespace Range

variable {α} (s: ProgramText α)

protected def lower (r: Range s) : Pos s where
  offset := r.raw.lower
  is_valid := r.is_valid.lower_is_valid

protected def upper (r: Range s) : Pos s where
  offset := r.raw.upper
  is_valid := r.is_valid.upper_is_valid

end Range

end ProgramText

abbrev Token {α} (s: ProgramText α) := ProgramText.Range s

end LexicalAnalysis

open LexicalAnalysis in
structure LexicalAnalysis (α: Type*) where
  run (s: ProgramText α) : List (Token s)

namespace LexicalAnalysis

instance {α} : DFunLike (LexicalAnalysis α) (ProgramText α) (fun s => List <| Token s) where
  coe la := la.run
  coe_injective' := by
    intro la1 la2 h1
    cases la1 ; cases la2
    simpa only [LexicalAnalysis.mk.injEq]

end LexicalAnalysis

/-!
#### 2.3. Parsing

1. So for humans, once the words are understood, the next step is to understand the structure of the sentence, and this is called parsing.
2. this means diagramming sentences, and these diagrams are trees, and it's a very simple procedure.
-/

namespace Parsing

open LexicalAnalysis ProgramText

inductive Tree.Raw where
  | leaf (range: Range.Raw)
  | branch (left: Tree.Raw) (right: Tree.Raw)
  deriving DecidableEq

namespace Tree.Raw

inductive IsWeakValid {α} (s: ProgramText α) : Tree.Raw → Prop where
  | leaf (range: Range s) : IsWeakValid s (.leaf range.raw)
  | branch
      (left: Tree.Raw) (left_req: IsWeakValid s left)
      (right: Tree.Raw) (right_req: IsWeakValid s right)
      : IsWeakValid s (.branch left right)

end Tree.Raw

open Tree.Raw in
@[ext]
structure Tree.WeakValid {α} (s: ProgramText α) where
  raw: Tree.Raw
  is_weak_valid: IsWeakValid s raw

namespace Tree.WeakValid

open LexicalAnalysis ProgramText Tree.Raw

variable {α} {s: ProgramText α}

@[match_pattern]
protected def leaf (range: Range s) : Tree.WeakValid s where
  raw := .leaf range.raw
  is_weak_valid := IsWeakValid.leaf range

@[match_pattern]
protected def branch (left: Tree.WeakValid s) (right: Tree.WeakValid s) : Tree.WeakValid s where
  raw := .branch left.raw right.raw
  is_weak_valid := IsWeakValid.branch left.raw left.is_weak_valid right.raw right.is_weak_valid

end Tree.WeakValid


end Parsing


end Nemonuri.Study.StanfordOnline.Compilers

end public_s
