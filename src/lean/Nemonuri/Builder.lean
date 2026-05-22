module

public import Mathlib.Algebra.Group.Hom.Defs

@[expose] public section public_s

set_option autoImplicit false
universe u

namespace Nemonuri



inductive ZeroHomBuilder (M: Type u) [Zero M] : (N: Type u) → Type u where
  | mk (N: Type u) [Zero N] (zeroHom: ZeroHom M N) : ZeroHomBuilder M N



namespace ZeroHomBuilder

variable {M N: Type u} [Zero M]


@[implicit_reducible]
protected def zeroN (bd: ZeroHomBuilder M N) : Zero N where
  zero :=
    match bd with
    | @mk M _ N zeroN _ => zeroN.zero


protected def zeroHom (bd: ZeroHomBuilder M N) : @ZeroHom M N _ bd.zeroN :=
  match bd with
  | @mk _ _ _ _ zeroHom => zeroHom


instance instFunLike : FunLike (ZeroHomBuilder M N) M N where
  coe bd := bd.zeroHom
  coe_injective' := by
    intro bd1 bd2 h1
    simp [ZeroHomBuilder.zeroHom] at h1
    simp only [← ZeroHom.toFun_eq_coe] at h1
    simp_all
    match bd1, bd2 with
    | @mk M _ N zeroN1 zeroHom1, @mk M _ N zeroN2 zeroHom2 =>
      simp
      simp only [funext_iff] at h1
      have lm4 : zeroN1 = zeroN2 := by
        specialize h1 0
        simp only [@zeroHom1.map_zero, @zeroHom2.map_zero] at h1
        simp only [OfNat.ofNat] at h1
        match zeroN1, zeroN2 with
        | .mk _, .mk _ => simp_all
      subst lm4
      simp
      match zeroHom1, zeroHom2 with
      | ⟨_,_⟩, ⟨_,_⟩ =>
        simp
        simp at h1
        ext x
        exact h1 x



end ZeroHomBuilder


inductive PropertyBuilder (M: Type u) [Zero M] : (N: Type u) → Type u where
  | mk (N: Type u) (zeroHomBuilder: ZeroHomBuilder M N) [DecidableEq N] : PropertyBuilder M N

namespace PropertyBuilder

variable {M N: Type u} [Zero M]


@[implicit_reducible]
protected def decN (bd: PropertyBuilder M N) : DecidableEq N :=
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
