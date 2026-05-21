module

public import Mathlib.Data.Set.Operations

@[expose] public section public_s

set_option autoImplicit false

namespace Nemonuri.Function

variable {α: Type _} {ι: Sort _}

def range (f : ι → α) : Set α := Set.range f

def range_set_range {f : ι → α} : range f = Set.range f := by rfl

def IsInRange (f : ι → α) (x: α) : Prop := x ∈ range f

def rangeFactorization (f : ι → α) : ι → { x // IsInRange f x } := Set.rangeFactorization f

@[simp]
theorem rangeFactorization_surjective {f : ι → α}
  : Function.Surjective (rangeFactorization f) := by
  simp only [rangeFactorization]
  exact Set.rangeFactorization_surjective


end Nemonuri.Function


end public_s
