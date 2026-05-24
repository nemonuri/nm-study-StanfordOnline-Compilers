module

public import Mathlib.Algebra.Group.Hom.Defs
--public import Mathlib.Data.TypeVec
--public import Mathlib.Data.Fin.Fin2
--public import Mathlib.Logic.Unique

@[expose] public section public_s

set_option autoImplicit false
universe u

namespace Nemonuri

/-
structure ZeroVec where
  length: Nat
  typeVec: TypeVec.{u} length
  toZero (i: Fin length) : Zero (typeVec (Fin2.ofFin i))

class HasZeroVec (T: Type*) where
  zeroVec: ZeroVec.{u}
-/

--abbrev ZeroContext (_: Type*) := (α: Type*) → Zero α


--structure ZeroHomBuilder (M N: Type*) [Zero M] [ZeroContext M] where
--  build: ZeroHom M N


--class ZeroContext (M N: Type*) extends Zero N

--def ZeroHomBuilder (M N: Type*) [Zero M] := [Zero N] → ZeroHom M N

/-
instance (priority := low) {N: Type*} [Unique (Zero N)] : Zero N := default

instance (priority := mid) {N: Type*} [Zero N] : Unique (Zero N) where
  default := inferInstance
  uniq z := by
    unfold inferInstance
-/

/-
namespace ZeroHomBuilder


variable {M N: Type u} [Zero M] [Unique (Zero N)]



@[implicit_reducible]
protected def zeroN (bd: ZeroHomBuilder M N) : Zero N where
  zero :=
    match bd with
    | @mk M _ N zeroN _ => zeroN.zero



protected def zeroHom (bd: ZeroHomBuilder M N) : ZeroHom M N := bd.build


set_option pp.explicit true in
instance instFunLike : FunLike (ZeroHomBuilder M N) M N where
  coe bd := bd.zeroHom
  coe_injective' := by
    intro bd1 bd2 h1
    simp [ZeroHomBuilder.zeroHom] at h1
    match bd1, bd2 with
    | ⟨build1⟩, ⟨build2⟩ =>
      simp ; simp at h1
      apply funext
      simp [Unique.forall_iff]
      exact h1

theorem coe_eq (bd: ZeroHomBuilder M N) : ⇑bd = ⇑bd.zeroHom := by rfl


instance instZeroHomClass : ZeroHomClass (ZeroHomBuilder M N) M N where
  map_zero bd := by simp only [coe_eq, map_zero]


end ZeroHomBuilder
-/

structure PropertyBuilder (M N: Type*) [Zero M] [Zero N] where
  zeroHom: ZeroHom M N
  deq: DecidableEq N

--inductive PropertyBuilder (M: Type u) [Zero M] : (N: Type u) → Type u where
--  | mk (N: Type u) (zeroHomBuilder: ZeroHomBuilder M N) [DecidableEq N] : PropertyBuilderBuilder M N

namespace PropertyBuilder

variable {M N: Type u} [Zero M] [Zero N]


instance instFunLike : FunLike (PropertyBuilder M N) M N where
  coe bd := bd.zeroHom
  coe_injective' := by
    intro bd1 bd2 h1
    simp at h1
    have lm1 : Subsingleton (DecidableEq N) := inferInstance
    replace lm1 := lm1.allEq
    match bd1, bd2 with
    | ⟨zhb1, dn1⟩, ⟨zhb2, dn2⟩ =>
      simp
      simp at h1
      specialize lm1 dn1 dn2
      constructor <;> assumption


theorem coe_eq (bd: PropertyBuilder M N) : ⇑bd = ⇑bd.zeroHom := by rfl


instance : ZeroHomClass (PropertyBuilder M N) M N where
  map_zero bd := by simp only [coe_eq, map_zero]


end PropertyBuilder


end Nemonuri

end public_s
