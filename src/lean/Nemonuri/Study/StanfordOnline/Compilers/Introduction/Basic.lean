module

public import Cslib.Foundations.Semantics.LTS.Basic
public import Cslib.Foundations.Semantics.LTS.LTSCat.Basic
public import Mathlib.Data.SetLike.Basic

public section public_s
@[expose] section expose_s

set_option autoImplicit false

namespace Nemonuri.Study.StanfordOnline.Compilers

/-
universe u v

structure State (St: Type u) where val: St

structure Label (La: Type v) where val: La

def LTS (St: Type u) (La: Type v) := Cslib.LTS (State St) (Label La)
-/

universe us ul


class LabelMorph (LabelA LabelB: Type _)
  extends
    SetLike LabelA (List (Option LabelB))


/-
instance (priority := low) (Label1 Label2: Type ul) : LabelMap Label1 Label2 where
  labelMap _ := .none
-/

--def LabelMap.toMorphism {Label1 Label2 State} (inst: LabelMap Label1 Label2)
/-
def LabelMap.mapLTS {St} (La1 La2) [LabelMap La1 La2] (lts: Cslib.LTS St La1) : Cslib.LTS St (Option La2) :=
  { Tr st1 la2? st2 :=
      ∀(la1: La1), ((LabelMap.labelMap la1) = la2?) → (la2?.isSome) → (lts.Tr st1 la1 st2) }
-/

end Nemonuri.Study.StanfordOnline.Compilers

/-
open Nemonuri.Study.StanfordOnline.Compilers in
def Cslib.LTS.mapByLabelMap {St La1} (lts: Cslib.LTS St La1) La2 [LabelMap La1 La2] : Cslib.LTS St (Option La2) :=
  { Tr st1 la2? st2 :=
      ∀(la1: La1), ((LabelMap.labelMap la1) = la2?) → (la2?.isSome) → (lts.Tr st1 la1 st2) }
-/

open Nemonuri.Study.StanfordOnline.Compilers in
def Cslib.LTS.mapBySetLike {St LaB} (ltsB: Cslib.LTS St LaB) LaA [LabelMorph LaA LaB] : Cslib.LTS St LaA :=
  { Tr st1 la st2 := ∀lb, (lb ∈ SetLike.coe la) → ltsB.withIdle.MTr st1 lb st2 }



end expose_s
end public_s
