module

public import Std.Data.ExtDHashMap

public section public_s
@[expose] section expose_s


namespace Nemonuri.Study.StanfordOnline.Compilers

set_option autoImplicit false

universe u

protected class abbrev Key (T: Type u) := BEq T, Hashable T, EquivBEq T, LawfulHashable T

namespace Programs

class HasProgramMap (T: Type u) (TKey: Type u) [Compilers.Key TKey] (toProgramType: TKey → Type u) where
  toProgramMap (self: T) : Std.ExtDHashMap TKey toProgramType

inductive Writer (TOther: Type u) [Compilers.Key TOther] where
  | you
  | other (x: TOther)
  deriving BEq, Hashable

namespace Writer


variable {TOther: Type u} [k: Compilers.Key TOther]

instance instEquivBEq : EquivBEq (Writer TOther) where
  symm := by
    unfold BEq.beq instBEqWriter instBEqWriter.beq at *
    intro a b h1
    match meq1: a, meq2: b with
    | .you, .you => simp
    | .other _, .other _ =>
      apply k.symm; trivial
  rfl := by
    unfold BEq.beq instBEqWriter instBEqWriter.beq at *
    intro a
    simp
    match meq1: a with
    | .you => simp
    | .other xa =>
      apply k.rfl
  trans := by
    unfold BEq.beq instBEqWriter instBEqWriter.beq at *
    intro a b c h1 h2
    match meq1: a, meq2: b, meq3: c with
    | .you, .you, .you => simp
    | .other _, .other _, .other _ =>
      simp_all
      exact k.trans h1 h2

instance instLawfulHashable : LawfulHashable (Writer TOther) where
  hash_eq a b h1 := by
    unfold Hashable.hash instHashableWriter instHashableWriter.hash at *
    simp
    match meq1: a, meq2: b with
    | .you, you => simp
    | .other xa, .other xb =>
      simp
      rw [k.hash_eq xa xb]
      unfold BEq.beq instBEqWriter instBEqWriter.beq at h1
      simp at h1
      exact h1

variable [k2: LawfulBEq TOther]

instance : LawfulBEq (Writer TOther) where
  eq_of_beq := by
    unfold BEq.beq instBEqWriter instBEqWriter.beq at *
    intro a b h1
    simp_all
    match meq1: a, meq2: b with
    | .you, .you => rfl
    | .other _, other _ =>
      simp at h1
      simp [h1]

end Writer

end Programs



end Nemonuri.Study.StanfordOnline.Compilers


end expose_s
end public_s
