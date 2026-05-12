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

inductive Directly.LabelView where
  | other
  | produceOutput
  deriving DecidableEq

def Directly (lts: Cslib.LTS St Directly.LabelView) : Prop :=
  ∀st1 st2,
    (lts.Tr st1 .produceOutput st2) →
    (ProduceOutput St st1 st2)

end ProduceOutput

/-!
5. Meaning that it(*self*) doesn't do any processing of the program(*prog*) before it executes it on the input.
-/
inductive DoesNotDoAnyProcessing.LabelView where
  | takeAsInput (prog: Program.T St)
  | produceOutput

structure DoesNotDoAnyProcessing (lts: Cslib.LTS St (DoesNotDoAnyProcessing.LabelView St)) : Prop where
  takeAsInput_spec prog data st1 st2 : (lts.Tr st1 (.takeAsInput prog) st2) → (TakeAsInput St prog data st1 st2)
  produceOutput_spec st1 st2 : (lts.Tr st1 .produceOutput st2) → (ProduceOutput St st1 st2)
  main_spec prog (st1 st2 st3: St) :
    (lts.Tr st1 (.takeAsInput prog) st2) →
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


inductive ImmediatelyBeginsRunning.LabelView where
  | other
  | invokeOnTheData

class IsRunning where
  v: (Program.T St) → Bool

variable [IsRunning St]

structure ImmediatelyBeginsRunning (lts: Cslib.LTS St ImmediatelyBeginsRunning.LabelView) : Prop where
  lts_spec (st1 st2: St) (data: Data.T St) : (lts.Tr st1 .invokeOnTheData st2) → (InvokeOnTheData St data st1 st2)
  main_spec (st1 st2: St) :
    (lts.Tr st1 .invokeOnTheData st2) →
    (IsRunning.v (Program.v st1) = false ∧ IsRunning.v (Program.v st2) = true)


/-!
7. we can say that the interpreter(*self*) is, *is online*, meaning it the work that it does is all part of running your program.
-/
inductive IsOnline.LabelView where
  | other
  | partOfRunningYourProgram

def IsOnline (lts: Cslib.LTS St IsOnline.LabelView) : Prop := ∀st1 st2, (lts.Tr st1 .partOfRunningYourProgram st2)




end Interpreter



end Nemonuri.Study.StanfordOnline.Compilers.Introduction

end expose_s
end public_s
