module

--public import Std.Data.ExtDHashMap
public import Nemonuri.Study.StanfordOnline.Compilers.Introduction.Basic
public import Cslib.Foundations.Semantics.LTS.Basic
public import Cslib.Foundations.Semantics.LTS.Relation
public import Cslib.Foundations.Semantics.LTS.LTSCat.Basic
public import Mathlib.Computability.Language

public section public_s
@[expose] section expose_s

set_option autoImplicit false

namespace Nemonuri.Study.StanfordOnline.Compilers.Introduction

universe us ul


namespace Interpreter
section _1

/-!

#### 1.1. Interpreter

So, what does an interpreter do?

1. You wrote a **Program**(*prog*).
-/
class Program (St: Type us) where
  protected T: Type us
  protected v: (st: St) → T
--variable (TProgram: Type u) (prog: TProgram)

--protected def prog := @Spec.Program.v


/-!
2. You have a **Data**(*data*), whatever you want to run the program on.
-/
class Data (St: Type us) where
  protected T: Type us
  protected v: (st: St) → T
--variable (TData: Type u) (data: TData)

--protected def data := @Spec.Data.v

/-!
3. An **Interpreter**(*self*) takes as input, your program(*prog*) and your data(*data*).
-/
variable
  (St: Type us)
  [Program St] [Zero (Program.T St)]
  [Data St] [Zero (Data.T St)]

structure TakeAsInput (prog: Program.T St) (data: Data.T St) (_ st2: St) : Prop where
  pre: (prog ≠ 0) ∧ (data ≠ 0)
  post: (Program.v st2 = prog) ∧ (Data.v st2 = data)
  --(prog ≠ 0) ∧ (Program.v st2 = prog) ∧ (data ≠ 0) ∧ (Data.v st2 = data)

variable [DecidableEq (Program.T St)] [DecidableEq (Data.T St)]
/-
namespace TakeAsInput

  structure Label [DecidableEq (Program.T St)] [DecidableEq (Data.T St)] where
    prog: Program.T St
    data: Data.T St


  instance Label.instDecidableEq : DecidableEq (Label St) :=
    fun l1 l2 =>
      if h: l1.prog = l2.prog ∧ l1.data = l2.data then
        .isTrue (by simp [← Label.mk.injEq] at h; exact h)
      else
        .isFalse (by simp [← Label.mk.injEq] at h; exact h)

  def toLTS (l: Label St) : Cslib.LTS St (Label St) :=
    Cslib.LTS.Relation.toLTS (TakeAsInput St l.prog l.data) l

end TakeAsInput
-/

/-!
4. It(*self*) produces the **Output**(*output*) directly.
-/
class Output (St: Type us) where
  protected T: Type us
  protected v: (st: St) → T

variable
  [Output St] [DecidableEq (Output.T St)] [Zero (Output.T St)]

structure ProduceOutput (st1 st2: St) : Prop where
  pre: (Program.v st1 ≠ 0) ∧ (Data.v st1 ≠ 0)
  post: (Output.v st2 ≠ 0)
  --(Program.v st1 ≠ 0) ∧ (Data.v st1 ≠ 0) ∧ (Output.v st2 ≠ 0)

namespace ProduceOutput

inductive Directly.Label where
  | other
  | produceOutput
  deriving DecidableEq

def Directly (lts: Cslib.LTS St (Directly.Label)) : Prop :=
  ∀st1 st2,
    (lts.Tr st1 (.produceOutput) st2) →
    (ProduceOutput St st1 st2)

end ProduceOutput

/-!
5. Meaning that it(*self*) doesn't do any processing of the program(*prog*) before it executes it on the input.
-/
inductive DoesNotDoAnyProcessing.Label where
  | takeAsInput (prog: Program.T St)
  | produceOutput

structure DoesNotDoAnyProcessing (lts: Cslib.LTS St (DoesNotDoAnyProcessing.Label St)) : Prop where
  takeAsInput_spec prog data st1 st2 : (lts.Tr st1 (.takeAsInput prog) st2) → (TakeAsInput St prog data st1 st2)
  produceOutput_spec st1 st2 : (lts.Tr st1 .produceOutput st2) → (ProduceOutput St st1 st2)
  main_spec prog (st1 st2 st3: St) :
    (lts.Tr st1 ((.takeAsInput prog)) st2) →
    (lts.Tr st2 .produceOutput st3) →
    ((prog = (Program.v st2)) ∧ ((Program.v st2) = (Program.v st3)))

/-!
6. So you just **write the program**(*prog*), and you **invoke~** the interpreter(*self*) **~on the data**(*data*), and the program(*prog*) immediately begins running.
-/
structure WriteTheProgram (prog: Program.T St) (st1 st2: St) : Prop where
  pre: (Program.v st1 = 0) ∧ (prog ≠ 0)
  post: (Program.v st2 = prog)


structure InvokeOnTheData (data: Data.T St) (st1 st2: St) : Prop where
  pre: (Program.v st1 ≠ 0) ∧ (data ≠ 0)
  post: (Program.v st2 = Program.v st1) ∧ (Data.v st2 = data)


inductive ImmediatelyBeginsRunning.Label where
  | invokeOnTheData
  deriving DecidableEq

class IsRunning where
  v: (Program.T St) → Bool

variable [IsRunning St]

structure ImmediatelyBeginsRunning (lts: Cslib.LTS St (Option ImmediatelyBeginsRunning.Label)) : Prop where
  lts_spec (st1 st2: St) (data: Data.T St) : (lts.Tr st1 (.some .invokeOnTheData) st2) → (InvokeOnTheData St data st1 st2)
  main_spec (st1 st2: St) :
    (lts.Tr st1 (.some .invokeOnTheData) st2) →
    (IsRunning.v (Program.v st1) = false ∧ IsRunning.v (Program.v st2) = true)


/-!
7. we can say that the interpreter(*self*) is, *is online*, meaning it the work that it does is all part of running your program.
-/
inductive IsOnline.Label where
  | partOfRunningYourProgram
  deriving DecidableEq

def IsOnline (lts: Cslib.LTS St (Option IsOnline.Label)) : Prop := ∀st1 st2, (lts.Tr st1 (.some .partOfRunningYourProgram) st2)

end _1

section def_s

protected class abbrev State.Program (St: Type us) := Program St, Zero (Program.T St)

protected class abbrev State.Data (St: Type us) := Data St, Zero (Data.T St)

protected class abbrev State.Output (St: Type us) := Output St, Zero (Output.T St)

class State (St: Type us) extends
  toStateProgram: State.Program St,
  toStateData: State.Data St,
  toStateOutput: State.Output St

--abbrev LawfulState (St: Type us) [State St] [Zero (Program.T St)] [Zero (Data.T St)] [Zero (Output.T St)] := State St


variable (St: Type us) [State St]

inductive Label where
  | writeTheProgram (prog: Program.T St)
  | invokeOnTheData (data: Data.T St)
--  | takeAsInput (prog: Program.T St) (data: Data.T St)
  | produceOutput

instance instLTS : Cslib.LTS St (Label St) where
  Tr st1 μ st2 :=
    match μ with
    --| .takeAsInput prog data => TakeAsInput St prog data st1 st2
    | .produceOutput => ProduceOutput St st1 st2
    | .writeTheProgram prog => WriteTheProgram St prog st1 st2
    | .invokeOnTheData data => InvokeOnTheData St data st1 st2

attribute [scoped simp] Singleton.singleton Set.singleton setOf Set funext_iff

instance : LabelMorph (ProduceOutput.Directly.Label) (Label St) where
  coe la :=
    match la with
    | .produceOutput =>
      { [.some .produceOutput] }
    | .other =>
      { x | (match x with | [.some .produceOutput] => False | [.some _] => True | _ => False) }
  coe_injective' := by
    intro la1 la2 h1
    simp at h1
    match meq1: la1, meq2: la2 with
    | .other, .other | .produceOutput, .produceOutput => rfl
    | .other, .produceOutput | .produceOutput, .other =>
      --unfold setOf Set at h1
      simp at h1
      --rewrite [funext_iff] at h1
      replace h1 := h1 [.some .produceOutput]
      simp at h1


/-
theorem forall_prop_congr_iff.{u} {α : Sort u} {p q : α → Prop} [dp: DecidablePred p] [dq: DecidablePred q] : (∀ a, p a = q a) ↔ (∀ a, p a) = (∀ a, q a) := by
  constructor
  case mp =>
    intro h
    exact (forall_congr h)
  case mpr =>
    intro h1 a
    simp [DecidablePred] at dp dq
    match mdp: (dp a).decide, mdq: (dq a).decide with
    | .true, .true | .false, .false => simp_all
    | .true, .false =>
      have lm1 : (∃a, p a) := ⟨a, (by simp_all)⟩
      have lm2 : (∃a, ¬q a) := ⟨a, (by simp_all)⟩
      replace lm2 := Classical.not_forall.mpr lm2
      simp [lm2] at h1
      have lm3 := And.intro lm1 h1 --: (∃ x, p x) ∧ ∃ x, ¬p x --...모순이 아니네...!!!!!
    --have lm1 : (∃a, p a) := ⟨a, dp a⟩
    --simp [←decide_eq_decide]
    --simp [←decide_eq_decide] at h1
-/
/-
    match mdp: (dp a).decide, mdq: (dq a).decide with
    | .true, .true | .false, .false => simp_all
    | .true, .false =>
-/
/-
    | .isTrue _, .isTrue _ | .isFalse _, .isFalse _ => simp_all
    | .isTrue hp, .isFalse hq =>
      simp_all
      have := (Iff.mpr h1)
    | .isTrue _, .isFalse _ =>
-/
    --simp [DecidablePred] at dp dq
    --dite

    --let dpa (a: α) := dp a
    --let dqa (a: α) := dq a

  --intro h
  --exact (forall_congr h) |> propext_iff.mp


instance : LabelMorph (DoesNotDoAnyProcessing.Label St) (Label St) where
  coe (la: DoesNotDoAnyProcessing.Label St) :=
    match la with
    | .takeAsInput prog => { x | ∃data, [.some (.writeTheProgram prog), .some (.invokeOnTheData data)] = x }
    | .produceOutput => { [.some .produceOutput] }
  coe_injective' := by
    intro la1 la2 h1
    simp at h1
    match meq1: la1, meq2: la2 with
    | .produceOutput, .produceOutput => rfl
    | .takeAsInput prog1, .takeAsInput prog2 =>
      simp at h1
      have lm1 (data: Data.T St) := h1 [.some (.writeTheProgram prog1), .some (.invokeOnTheData data)]
      simp at lm1
      simp [lm1]
    | .produceOutput, .takeAsInput prog | .takeAsInput prog, .produceOutput =>
      simp at h1
      have lm1 (data: Data.T St) := h1 [.some (.writeTheProgram prog), .some (.invokeOnTheData data)]
      simp at lm1

#print instLabelMorphLabelLabel_1


/-
instance : LabelMap (Label St) (DoesNotDoAnyProcessing.Label St) where
  labelMap l :=
    match l with
    | .takeAsInput prog _ => .some (.takeAsInput prog)
    | _ => .none

instance : LabelMap (Label St) (ImmediatelyBeginsRunning.Label) where
  labelMap l :=
    match l with
    | .invokeOnTheData _ => .some (.invokeOnTheData)
    | _ => .none

instance : LabelMap (Label St) (IsOnline.Label) where
  labelMap _ := .some (.partOfRunningYourProgram)
-/






attribute [scoped simp] Cslib.LTS.mapTo_iff Cslib.LTS.withIdle
--attribute [scoped simp ←]

protected theorem ProduceOutput.Directly.proof
  : (instLTS St).mapTo (ProduceOutput.Directly.Label) |> ProduceOutput.Directly St := by
  simp [ProduceOutput.Directly]
  intro st1 st2 h1
  replace h1 := h1 [.some .produceOutput]
  simp [←Cslib.LTS.MTr.single_iff, SetLike.coe, setOf] at h1
  exact h1


namespace DoesNotDoAnyProcessing

open Cslib.LTS
attribute [scoped simp] Membership.mem Set.Mem SetLike.coe

protected abbrev M.lts := (instLTS St).mapTo (DoesNotDoAnyProcessing.Label St)

private theorem proof_aux1
  prog st1 st3
  (h1: (M.lts St).Tr st1 (.takeAsInput prog) st3)
  data
  : ∃(st2: St), WriteTheProgram St prog st1 st2 ∧ InvokeOnTheData St data st2 st3 := by
  simp at h1
  replace h1 := h1 data
  match meq1: h1 with
  | MTr.stepL hd tl =>
    rename St => st1_2
    simp at hd
    simp [←Cslib.LTS.MTr.single_iff] at tl
    obtain ⟨hd1, hd2⟩ := hd
    obtain ⟨tl1, tl2⟩ := tl
    constructor
    case w => exact st1_2
    case h =>
      constructor <;> constructor <;> simp_all
    --unfold WriteTheProgram at cont


    --constructor <;> constructor
    --simp_all

protected theorem proof
  : (M.lts St) |> DoesNotDoAnyProcessing St := by
  constructor
  case takeAsInput_spec =>
    intro prog data st1 st3 h1
    obtain ⟨st2, st2_p⟩ := proof_aux1 St prog st1 st3 h1 data
    obtain ⟨⟨st2_p1, st2_p2⟩,⟨st2_p3,st2_p4⟩⟩ := st2_p
    constructor <;> simp_all
  case produceOutput_spec =>
    intro st1 st2 h1
    simp [←Cslib.LTS.MTr.single_iff] at h1
    exact h1
  case main_spec =>
    intro prog st1 st2 st3 h1 h2
    obtain ⟨st2, st2_p⟩ := proof_aux1 St prog st1 st2 h1 (0: Data.T St)
    obtain ⟨⟨st2_p1, st2_p2⟩,⟨st2_p3,st2_p4⟩⟩ := st2_p
    constructor <;> simp_all

end DoesNotDoAnyProcessing







end def_s

end Interpreter



end Nemonuri.Study.StanfordOnline.Compilers.Introduction

end expose_s
end public_s
