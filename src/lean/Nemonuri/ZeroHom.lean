module

public import Mathlib.Algebra.Group.Hom.Defs

@[expose] public section public_s

set_option autoImplicit false
universe u

namespace Nemonuri

/-
structure Zero.Context : Type (u + 1) where
  protected N: Type u
  protected zero: Zero N
-/

--set_option trace.Meta.synthInstance true in
--structure ZeroHomContext (M N: Type u) extends ZeroContext M N where
--  zeroHom: ZeroHom M N

--structure ZeroHom.Builder (M: Type u) [Zero M] where
--  toZeroHom (N: Type u) [Zero N] : ZeroHom M N

/-
structure ZeroHom.Context (M: Type u) [Zero M] where
  protected T: Type u
  protected v [Zero T] : ZeroHom M T
-/


protected inductive Builder.ZeroHom (M: Type u) [Zero M] : (N: Type u) → Type u where
  | mk (N: Type u) [Zero N] (zeroHom: ZeroHom M N) : Builder.ZeroHom M N



--def ZeroHom.Context.zeroHom {M} [Zero M] (ctx: ZeroHom.Context M) [Zero ctx.T] : ZeroHom M ctx.T :=
--  ctx.toZeroHom ctx.T

/-
@[ext]
structure ZeroHom.Context (M N: Type u) [Zero M] extends Zero N where
  zeroHom: ZeroHom M N
-/

namespace Builder.ZeroHom

variable {M N: Type u} [Zero M]


@[implicit_reducible]
protected def zeroN (bd: Builder.ZeroHom M N) : Zero N where
  zero :=
    match bd with
    | @mk M _ N zeroN _ => zeroN.zero


protected def zeroHom (bd: Builder.ZeroHom M N) : @ZeroHom M N _ bd.zeroN :=
  match bd with
  | @mk _ _ _ _ zeroHom => zeroHom


instance instFunLike : FunLike (Builder.ZeroHom M N) M N where
  coe bd := bd.zeroHom
  coe_injective' := by
    intro bd1 bd2 h1
    simp [Builder.ZeroHom.zeroHom] at h1
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



end Builder.ZeroHom


end Nemonuri

end public_s
