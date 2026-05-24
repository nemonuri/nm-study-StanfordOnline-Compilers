module

public import Mathlib.Algebra.Group.Hom.Defs
--public import Mathlib.Logic.Unique

@[expose] public section public_s

set_option autoImplicit false
universe u

namespace Nemonuri


structure ZeroHomBuilder (M N: Type*) [Zero M] where
  build: [Zero N] → ZeroHom M N


--class ZeroContext (M N: Type*) extends Zero N

--def ZeroHomBuilder (M N: Type*) [Zero M] := [Zero N] → ZeroHom M N


instance {N: Type*} [Unique (Zero N)] : Zero N := default



namespace ZeroHomBuilder


variable {M N: Type u} [Zero M] [Unique (Zero N)]


/-
@[implicit_reducible]
protected def zeroN (bd: ZeroHomBuilder M N) : Zero N where
  zero :=
    match bd with
    | @mk M _ N zeroN _ => zeroN.zero
-/


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

set_option pp.proofs true in
#print instFunLike._proof_5
#print instFunLike._simp_4

/-
@[implicit_reducible]
protected def zeroHomClass (bd: ZeroHomBuilder M N) : @ZeroHomClass (ZeroHomBuilder M N) M N _ bd.zeroN _ :=
  let (eq := lm1) zeroN : Zero N := bd.zeroN
  ⟨fun bd => by
    have lm2 : bd 0 = bd.zeroHom 0 := by rfl
    simp [lm2]
    have lm3 := bd.zeroHom.map_zero
    --subst zeroN
    --apply (ZeroHom.map_zero)
  ⟩

--instance : ZeroHomClass (ZeroHomBuilder M N) M N where


end ZeroHomBuilder
-/

structure PropertyBuilder (M N: Type*) [Zero M] [Zero N] where
  zeroHom: ZeroHom M N
  deq': DecidableEq N

--inductive PropertyBuilder (M: Type u) [Zero M] : (N: Type u) → Type u where
--  | mk (N: Type u) (zeroHomBuilder: ZeroHomBuilder M N) [DecidableEq N] : PropertyBuilder M N

namespace PropertyBuilder

variable {M N: Type u} [Zero M] [Zero N]


@[implicit_reducible]
protected def deq (bd: PropertyBuilder M N) : DecidableEq N :=
  match bd with | @mk M _ N _ decN => decN

protected def zeroHomBuilder (bd: PropertyBuilder M N) : ZeroHomBuilder M N :=
  match bd with | @mk M _ N zhb _ => zhb


instance instFunLike : FunLike (PropertyBuilder M N) M N where
  coe bd := bd.zeroHomBuilder
  coe_injective' := by
    intro bd1 bd2 h1
    simp at h1
    have lm1 : Subsingleton (DecidableEq N) := inferInstance
    replace lm1 := lm1.allEq
    match bd1, bd2 with
    | @mk _ _ _ zhb1 dn1, @mk _ _ _ zhb2 dn2 =>
      simp
      simp only [PropertyBuilder.zeroHomBuilder] at h1
      specialize lm1 dn1 dn2
      constructor <;> assumption

@[reducible]
protected def zeroN (bd: PropertyBuilder M N) : Zero N := bd.zeroHomBuilder.zeroN

@[reducible]
protected def zero (bd: PropertyBuilder M N) : N := bd.zeroN.zero


end PropertyBuilder


end Nemonuri

end public_s
