module

public import Nemonuri.Study.StanfordOnline.Compilers.Lemma
public import Mathlib.Logic.Equiv.Defs

@[expose] public section public_s

namespace Nemonuri.Study.StanfordOnline.Compilers

set_option autoImplicit false

/-
#### 2.2. lexical analysis

The goal of lexical analysis, then, is to divide the program text into its words, or what we call in compiler speak, the tokens.
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



end Nemonuri.Study.StanfordOnline.Compilers

end public_s
