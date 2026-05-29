module

public import Mathlib.Tactic.Contrapose
public import Mathlib.Order.Bounds.Basic
public import Mathlib.Logic.Unique

@[expose] public section public_s


universe u
set_option autoImplicit false

namespace Nemonuri

variable (α: Type u) [LE α]

instance toLT [Std.IsPartialOrder α] : LT α where
  lt a b := (a ≤ b) ∧ (a ≠ b)

instance [po: Std.IsPartialOrder α] : Std.LawfulOrderLT α := Std.LawfulOrderLT.of_le (by
  intro a b
  unfold toLT
  simp
  intro h
  constructor
  · contrapose; exact po.le_antisymm a b h
  · contrapose; intro h2; simp only [h2, po.le_refl]
)

instance [po: Std.IsPartialOrder α] : PartialOrder α where
  le_refl := po.le_refl
  le_trans := po.le_trans
  le_antisymm := po.le_antisymm
  lt_iff_le_not_ge := (inferInstance : Std.LawfulOrderLT α).lt_iff

@[ext]
class LeastOn {α: Type*} [PartialOrder α] (pred: α → Prop) where
  least: α
  is_least: IsLeast { x: α | pred x } least

instance {α} [PartialOrder α] {pred: α → Prop} [LeastOn pred] : Unique (LeastOn pred) where
  default := inferInstance
  uniq := by
    rename (LeastOn pred) => b
    intro a
    have lm1 := IsLeast.unique a.is_least b.is_least
    simpa only [LeastOn.ext_iff] using lm1



end Nemonuri

end public_s
