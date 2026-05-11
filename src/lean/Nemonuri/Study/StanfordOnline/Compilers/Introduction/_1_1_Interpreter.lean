module

--public import Std.Data.ExtDHashMap
--public import Nemonuri.Study.StanfordOnline.Compilers.Introduction.Program
public import Cslib.Foundations.Semantics.LTS.Basic
public import Cslib.Foundations.Semantics.LTS.Relation

public section public_s
@[expose] section expose_s

set_option autoImplicit false

namespace Nemonuri.Study.StanfordOnline.Compilers.Introduction

universe us ul


namespace Interpreter

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

def TakeAsInput (prog: Program.T St) (data: Data.T St) (_ st2: St) : Prop :=
  (prog ≠ 0) ∧ (Program.v st2 = prog) ∧ (data ≠ 0) ∧ (Data.v st2 = data)


variable [DecidableEq (Program.T St)] [DecidableEq (Data.T St)]
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


/-!
4. It(*self*) produces the **Output**(*output*) directly.
-/
class Output (St: Type us) where
  protected T: Type us
  protected v: (st: St) → T

variable
  [Output St] [DecidableEq (Output.T St)] [Zero (Output.T St)]

def ProduceOutput (st1 st2: St) : Prop :=
  (Program.v st1 ≠ 0) ∧ (Data.v st1 ≠ 0) ∧ (Output.v st2 ≠ 0)

namespace ProduceOutput

structure Label (St: Type us)

instance Label.instDecidableEq : DecidableEq (Label St) :=
  fun l1 l2 => .isTrue (by simp)

def toLTS (l: Label St) : Cslib.LTS St (Label St) :=
  Cslib.LTS.Relation.toLTS (ProduceOutput St) l

end ProduceOutput



/-!
5. Meaning that it(*self*) doesn't do any processing of the program(*prog*) before it executes it on the input.
6. So you just write the program(*prog*), and you invoke the interpreter(*self*) on the data(*data*), and the program(*prog*) immediately begins running.
-/
namespace DoesNotDoAnyProcessingBefore

open Std

inductive Label where
  | takeAsInput (l: TakeAsInput.Label St)
  | produceOutput (l: ProduceOutput.Label St)
  | other
  deriving DecidableEq


abbrev LabelList := List (Label St)

namespace LabelList

theorem length_aux {α} (ls: List α) : (ls ≠ []) ↔ (ls.length > 0) := by
  constructor
  case mp =>
    intro not_nil
    match meq1: ls with
    | .cons hd tl => simp
  case mpr =>
    intro gt_0
    match meq1: ls with
    | .cons _ _ => simp

abbrev NotNil (ls: LabelList St) : Prop := ls ≠ []

/-
def toMidRange ls (not_nil: NotNil St ls) : Roo (Fin (ls.length)) :=
  Roo.mk
    (Fin.mk 0 (Iff.mp (length_aux ls) not_nil))
    (Fin.mk (ls.length-1) (by simp [length_aux] at not_nil; omega))
-/

variable (ls: LabelList St) (not_nil: NotNil St ls)

abbrev IsHeadTakeAsInput : Prop := (ls.head not_nil) matches .takeAsInput _

abbrev IsLastProduceOutput : Prop := (ls.getLast not_nil) matches .produceOutput _

abbrev AllMidsOther : Prop := (ls.toSlice 1 (ls.length - 1)).toList.all (· matches .other)


end LabelList

/-
protected def Def (lts: Cslib.LTS St (Label St)) : Prop :=
  (st1: St) → (ls: List (Label St)) → (st2: St) →
  (ls_not_nil: ls ≠ []) →
  (ls_head_matches_takeAsInput: (ls.head ls_not_nil) matches .takeAsInput _) →
  (ls_last_matches_produceOutput: (ls.getLast ls_not_nil) matches .produceOutput _) →
-/

end DoesNotDoAnyProcessingBefore

open DoesNotDoAnyProcessingBefore LabelList in
def DoesNotDoAnyProcessingBefore (lts: Cslib.LTS St (Label St)) : Prop :=
  (st1: St) → (ls: List (Label St)) → (st2: St) → (mtr: lts.MTr st1 ls st2) →
  (ils: List (Label St)) →
  (List.IsInfix ils ls) →
  (not_nil: NotNil St ils) →
  (IsHeadTakeAsInput St ils not_nil) →
  (IsLastProduceOutput St ils not_nil) →
  (AllMidsOther St ils) →
  (Program.v st1 = Program.v st2)



/-
def DoesNotDoAnyProcessingBefore (st1: St) (ls: List (Label St)) (st2: St) : Prop :=
  match ls with
  | [] => st1 = st2
  | hd::tl =>
  match hd with
  | .takeAsInput (.mk prog data) =>
-/




  --(prog: Program.T St) → (data: Data.T St) → (TakeAsInput prog data st1 st2) →

/-
protected abbrev Specs.doesn't_do_any_processing_before
  {tc: TypeContext}
  (self: Specs.TakesAsInput tc.TProgram tc.TData tc.TOutput)
  (label)
  (state: self.ReqState label)
  (data: tc.TData)
  : Prop :=
  match self.takeInput state data with
  | .takeInput state1 _ prog2 => (toProgram state1) = prog2
-/


/-!
7. we can say that the interpreter(*self*) is, *is online*, meaning it the work that it does is all part of running your program.
-/
@[reducible]
def Specs.is_online
  {tc: TypeContext}
  (self: Specs.ProduceOutput tc.TProgram tc.TData tc.TOutput)
  (state: State tc .takeInput)
  : Prop :=
  match self.produceOutput state with
  | .produceOutput (.takeInput _ _ prog1) _ prog2 => prog1 = prog2


/-
structure Traces.Invoke (TProgram: Type u) (TData: Type u) (TInterpreter: type_of% TInterpreter) where
  takeAsInput: TInterpreter TProgram TData .takeAsInput
  produceOutput: TInterpreter TProgram TData .produceOutput

@[reducible]
def Specs.is_well_formed
  (label: StateLabel)
  (self: TInterpreter TProgram TData label)
  (prog: TProgram) (data: TData)
  : Prop :=
  match label with
  | .init => Specs.doesn't_do_any_processing_before self prog data
  | .takeAsInput => Specs.is_online TOutput self
  | .produceOutput => True

def invoke
  (wf: ∀label (self: TInterpreter TProgram TData label) prog data, Specs.is_well_formed TOutput label self prog data)
  (self: TInterpreter TProgram TData .init)
  (prog: TProgram) (data: TData) :
  { post: TOutput × (Traces.Invoke TProgram TData TInterpreter)
    // Tests.doesn't_do_any_processing_before prog post.snd.takeAsInput ∧
       Tests.is_online post.snd.takeAsInput post.snd.produceOutput }
  :=
  let trTakeAsInput := ctai.takeAsInput self prog data
  match meq1: CanProduceOutput.produceOutput trTakeAsInput with
  | ⟨(output: TOutput), trProduceOutput⟩ =>
    let postVal := Prod.mk output (Traces.Invoke.mk trTakeAsInput trProduceOutput)
    have lemma1 : Tests.doesn't_do_any_processing_before prog postVal.snd.takeAsInput := by
      unfold Specs.is_well_formed at wf
      unfold Specs.doesn't_do_any_processing_before at wf
      replace wf := wf _ self prog data
      simp at wf
      congr
    have lemma2 : Tests.is_online postVal.snd.takeAsInput postVal.snd.produceOutput := by
      unfold Specs.is_well_formed at wf
      unfold Specs.is_online at wf
      replace wf := wf _ trTakeAsInput prog data
      simp at wf
      subst postVal
      simp_all
    Subtype.mk postVal (And.intro lemma1 lemma2)
-/


end Interpreter

open Interpreter in
class Interpreter (tc: TypeContext)
  extends
    toProduceOutput: Specs.ProduceOutput tc.TProgram tc.TData tc.TOutput,
    toTakesAsInput: Specs.TakesAsInput tc.TProgram tc.TData tc.TOutput
  where
    doesn't_do_any_processing_before label state data :
      Specs.doesn't_do_any_processing_before toTakesAsInput label state data
    is_online state : Specs.is_online toProduceOutput state

instance Interpreter.instRuntime {tc: TypeContext} [self: Interpreter tc] : Runtime tc :=
  @Runtime.mk _ self.toTakesAsInput self.toProduceOutput



end Nemonuri.Study.StanfordOnline.Compilers.Introduction

end expose_s
end public_s
