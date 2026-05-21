module

public import Mathlib.Algebra.Group.Hom.Defs

@[expose] public section public_s

set_option autoImplicit false
universe u

namespace Nemonuri


structure Zero.Context : Type (u + 1) where
  protected N: Type u
  protected zero: Zero N

--set_option trace.Meta.synthInstance true in
--structure ZeroHomContext (M N: Type u) extends ZeroContext M N where
--  zeroHom: ZeroHom M N

structure ZeroHom.Context (M: Type u) [Zero M] extends Zero.Context where
  zeroHom: ZeroHom M N



namespace ZeroHomContext

/-
instance {M N: Type u} [Zero M] : FunLike (ZeroHomContext M N) M N where
  coe ctx := ctx.zeroHom
  coe_injective' := by
    intro a1 a2 h
    simp at h
    simp only [DFunLike.coe] at h
-/


end ZeroHomContext


end Nemonuri

end public_s
