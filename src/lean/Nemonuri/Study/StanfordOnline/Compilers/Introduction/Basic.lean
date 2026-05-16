module

public import Cslib.Foundations.Semantics.LTS.Basic
public import Cslib.Foundations.Semantics.LTS.LTSCat.Basic
public import Mathlib.Data.SetLike.Basic

public section public_s
@[expose] section expose_s

set_option autoImplicit false




namespace Nemonuri.Study.StanfordOnline.Compilers


universe us ul


class LabelMorph (LabelA LabelB: Type _)
  extends
    SetLike LabelA (List (Option LabelB))

/-
class LtsM (St La: Type _)
  extends
    Cslib.LTS St La

def _root_.Cslib.LTS.map {St LaB} (lts: Cslib.LTS St LaB) [LtsM St LaB] LaA [LabelMorph LaA LaB] : Cslib.LTS St LaA :=
  lts.mapByLabelMorph LaA

theorem LtsM.map_eq
  {St LaB} (lts: Cslib.LTS St LaB) [LtsM St LaB] LaA [LabelMorph LaA LaB] st1 laa st2
  : (LtsM.map lts).Tr st1 laa st2
-/

end Nemonuri.Study.StanfordOnline.Compilers

def Cslib.LTS.mapByLabelMorph {St LaB} (ltsB: Cslib.LTS St LaB) LaA [SetLike LaA (List (Option LaB))] : Cslib.LTS St LaA :=
  { Tr st1 la st2 := ∀lb, (lb ∈ SetLike.coe la) → ltsB.withIdle.MTr st1 lb st2 }

open Nemonuri.Study.StanfordOnline.Compilers in
def Cslib.LTS.mapTo {St LaB} (ltsB: Cslib.LTS St LaB) LaA [LabelMorph LaA LaB] : Cslib.LTS St LaA :=
  ltsB.mapByLabelMorph LaA


open Nemonuri.Study.StanfordOnline.Compilers Cslib.LTS in
theorem Cslib.LTS.mapTo_iff {St LaB} (ltsB: Cslib.LTS St LaB) LaA [lm: LabelMorph LaA LaB] st1 la st2
  : (Cslib.LTS.mapTo ltsB LaA).Tr st1 la st2 ↔ (∀lb, ((lm.coe la lb) → ltsB.withIdle.MTr st1 lb st2)) := by
  simp [mapTo]
  simp [mapByLabelMorph]
  simp [Membership.mem, Set.Mem]

/-
open Nemonuri.Study.StanfordOnline.Compilers in
def Cslib.LTS.mapByLabelMap {St La1} (lts: Cslib.LTS St La1) La2 [LabelMap La1 La2] : Cslib.LTS St (Option La2) :=
  { Tr st1 la2? st2 :=
      ∀(la1: La1), ((LabelMap.labelMap la1) = la2?) → (la2?.isSome) → (lts.Tr st1 la1 st2) }
-/

/-
theorem Cslib.LTS.MTr.single_iff.{u, v}
  {State : Type u} {Label : Type v} (lts : LTS State Label) (s1 : State) (μ : Label) (s2 : State)
  : lts.Tr s1 μ s2 ↔ lts.MTr s1 [μ] s2 :=
  Iff.intro (Cslib.LTS.MTr.single lts) (Cslib.LTS.MTr.single_invert lts s1 μ s2)
-/

end expose_s
end public_s
