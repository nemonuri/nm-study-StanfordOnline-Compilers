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

/-
protected theorem ext_iff {bd1 bd2: Builder.ZeroHom M N}
  : (bd1 = bd2) ↔ (bd1.zeroN = bd2.zeroN ∧ bd1.zeroHom ≍ bd2.zeroHom) := by
  constructor
  · intro h ; rw [h]; simp only [heq_eq_eq, and_self]
  · rintro ⟨h1, h2⟩
    match lm1: bd1, lm2: bd2 with
    | @mk M _ N zeroN1 zeroHom1, @mk M _ N zeroN2 zeroHom2 =>
      unfold Builder.ZeroHom.zeroN at h1
-/



set_option trace.Meta.synthInstance true in
instance : FunLike (Builder.ZeroHom M N) M N where
  coe bd := bd.zeroHom
  coe_injective' := by
    intro bd1 bd2 h1
    simp [Builder.ZeroHom.zeroHom] at h1
    simp only [← ZeroHom.toFun_eq_coe] at h1
    --have lm1 := @bd1.zeroHom.map_zero
    --have lm2 := @bd2.zeroHom.map_zero
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
      rewrite [lm4] at *
      --simp [lm4]
      --have lm5 : type_of% zeroHom1 = type_of% zeroHom2 := by rw [lm4]
      --apply (heq_of_cast_eq lm5)
/-
      ext x
      specialize h1 x
      rw [← h1]
-/



      --apply heq_of_eq_cast
      --conv => rhs; simp [lm4]


      --simp [← ZeroHom.toFun_eq_coe] at h1

    --have _ := (inferInstance : FunLike (type_of% bd1.zeroHom) M N).coe_injective
/-
  coe ctx := ctx.zeroHom
  coe_injective' := by
    intro a1 a2 h
    simp at h
    simp only [DFunLike.coe] at h
    have lm1 : a1.zeroHom = a2.zeroHom := by
    --simp only [← ZeroHom.toFun_eq_coe] at h
    --simp [DFunLike.coe_fn_eq] at h
    --simp only [DFunLike.coe] at h
    --simp only [ZeroHom.Context.ext_iff]
-/


end Builder.ZeroHom


end Nemonuri

end public_s
