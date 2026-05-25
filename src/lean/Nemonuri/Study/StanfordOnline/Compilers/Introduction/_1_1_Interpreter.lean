module

--public import Std.Data.ExtDHashMap
public import Nemonuri.Study.StanfordOnline.Compilers.Introduction.Basic
public import Nemonuri.Study.StanfordOnline.Compilers.Lemma
public import Cslib.Foundations.Semantics.LTS.Basic
public import Cslib.Foundations.Semantics.LTS.Relation
public import Cslib.Foundations.Semantics.LTS.LTSCat.Basic
public import Mathlib.Computability.Language
public import Nemonuri

public section public_s
@[expose] section expose_s

set_option autoImplicit false

namespace Nemonuri.Study.StanfordOnline.Compilers.Introduction

universe us ul


namespace Interpreter
section spec_s

variable
  (St: Type us) [Zero St]

/-!

#### 1.1. Interpreter

So, what does an interpreter do?

1. You wrote a **Program**(*prog*).
-/
class Program where
  protected T: Type us
  protected ctx: PropertyContext T
  protected v: Property St T

attribute [reducible, instance] Program.ctx

variable [Program St]



/-!
2. You have a **Data**(*data*), whatever you want to run the program on.
-/
class Data where
  protected T: Type us
  protected ctx: PropertyContext T
  protected v: Property St T

attribute [reducible, instance] Data.ctx

variable [Data St]

/-!
3. An **Interpreter**(*self*) takes as input, your program(*prog*) and your data(*data*).
-/
namespace TakeAsInput

set_option trace.Meta.synthInstance true

abbrev Req (prog: Program.T St) (data: Data.T St) : Prop := (prog ≠ 0) ∧ (data ≠ 0)

instance {prog data} : Decidable (Req St prog data) := inferInstanceAs (Decidable (_ ∧ _))

abbrev Ens (prog: Program.T St) (data: Data.T St) (st2: St) : Prop := (Program.v st2 = prog) ∧ (Data.v st2 = data)

instance {prog data st2} : Decidable (Ens St prog data st2) := inferInstanceAs (Decidable (_ ∧ _))

end TakeAsInput

open TakeAsInput in
structure TakeAsInput where
  run (prog: Program.T St) (data: Data.T St) (st1: St) (req: Req _ prog data) : { st2: St // Ens _ prog data st2 }


/-!
4. It(*self*) produces the **Output**(*output*) directly.
-/
class Output where
  protected T: Type us
  protected ctx: PropertyContext T
  protected v: Property St T

attribute [reducible, instance] Output.ctx

variable [Output St] --[Property (@Output.v St _)]

namespace ProduceOutput

abbrev Req (st1: St) : Prop := (Program.v st1 ≠ 0) ∧ (Data.v st1 ≠ 0)

instance {st1} : Decidable (Req St st1) := inferInstanceAs (Decidable (_ ∧ _))

def Ens (st2: St) : Prop := (Output.v st2 ≠ 0)

--instance {st2} : Decidable (Ens St st2) := inferInstance

end ProduceOutput

open ProduceOutput in
structure ProduceOutput where
  run (st1: St) (req: Req _ st1) : { st2: St // Ens _ st2 }

/-
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
-/

/-!
5. Meaning that it(*self*) doesn't do any processing of the program(*prog*) before it executes it on the input.
-/
namespace ProduceOutputDirectly

abbrev ReqFirst (stF: St) : Prop := ProduceOutput.Req _ stF

abbrev EnsLast (stL: St) : Prop := ProduceOutput.Ens _ stL

abbrev EnsAll (stX1 stX2: St) : Prop := AppEq Program.v stX1 stX2

end ProduceOutputDirectly


open ProduceOutputDirectly Cslib LTS in
def ProduceOutputDirectly {La} (lts: LTS St La) : Prop :=
  ∀(stF: St), (ReqFirst _ stF) →
  ∃ls stL sts, (lts.Execution stF ls stL sts) ∧
    (EnsLast _ stL) ∧
    (sts.Pairwise (EnsAll _))


/-
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
-/

/-!
6. So you just **write the program**(*prog*), and you **invoke~** the interpreter(*self*) **~on the data**(*data*), and the program(*prog*) immediately begins running.
-/
protected class IsRunning.State where
  protected T: Type us
  protected ctx: PropertyContext T
  protected v: Property St T

attribute [reducible, instance] IsRunning.State.ctx

instance : Zero Bool where zero := false

variable [IsRunning.State St]

class IsRunning where
  protected v : ZeroHom (IsRunning.State.T St) (ZeroHom (Program.T St) Bool)

variable [IsRunning St]

abbrev IsProgramRunning (st: St) : Prop := IsRunning.v (IsRunning.State.v st) (Program.v st)

namespace WriteTheProgram

abbrev Req (prog: Program.T St) (_: St) : Prop := (prog ≠ 0)

instance (prog: Program.T St) (st1: St) : Decidable (Req _ prog st1) := inferInstance

abbrev Ens (prog: Program.T St) (st2: St) : Prop := (Program.v st2 = prog) ∧ (¬IsProgramRunning _ st2)

end WriteTheProgram

open WriteTheProgram in
structure WriteTheProgram where
  run (prog: Program.T St) (st1: St) (req: Req _ prog st1) : { st2: St // Ens _ prog st2 }


namespace InvokeOnTheData

abbrev Req (data: Data.T St) (st1: St) : Prop := (Program.v st1 ≠ 0) ∧ (data ≠ 0) ∧ (¬IsProgramRunning _ st1)

instance {data st1} : Decidable (Req St data st1) := inferInstance

abbrev Ens (data: Data.T St) (st1 st2: St) : Prop := (Program.v st2 = Program.v st1) ∧ (Data.v st2 = data) ∧ (IsProgramRunning _ st2)

end InvokeOnTheData

open InvokeOnTheData in
structure InvokeOnTheData where
  run (data: Data.T St) (st1: St) (req: Req _ data st1) : { st2: St // Ens _ data st1 st2 }

namespace ImmediatelyBeginsRunning

abbrev ReqFirst (data: Data.T St) (stF: St) : Prop := InvokeOnTheData.Req _ data stF

abbrev EnsLast (data: Data.T St) (stF stL: St) : Prop := InvokeOnTheData.Ens _ data stF stL

abbrev EnsList (sts: List St) (stF stL: St) : Prop :=
  ∃(h: sts.length = 2),
    (sts.head (by apply List.ne_nil_of_length_pos; simp [h]) = stF) ∧
    (sts.getLast (by apply List.ne_nil_of_length_pos; simp [h]) = stL)

end ImmediatelyBeginsRunning

open ImmediatelyBeginsRunning in
def ImmediatelyBeginsRunning {La} (lts: Cslib.LTS St La) : Prop :=
  ∀data (stF: St), (ReqFirst _ data stF) →
  ∃ls sts stL, (lts.Execution stF ls stL sts) ∧
    (EnsLast _ data stF stL) ∧
    (EnsList _ sts stF stL)


/-!
7. we can say that the interpreter(*self*) is, *is online*, meaning it the work that it does is all part of running your program.
-/
def IsOnline (sts: List St) : Prop :=
  (∀stX ∈ sts, Program.v stX ≠ 0) ∧ (sts.Pairwise (AppEq Program.v))

end spec_s

section def_s

variable (St: Type us) [Zero St]


class State where
  toProgram: Program St
  toData: Data St
  toOutput: Output St
  toIsRunningState: IsRunning.State St
  toIsRunning: IsRunning St

attribute [reducible, instance]
  State.toProgram State.toData State.toOutput State.toIsRunningState State.toIsRunning


variable [State St]

class Ability where
  writeTheProgram: WriteTheProgram St
  invokeOnTheData: InvokeOnTheData St
  produceOutput: ProduceOutput St

variable [Ability St]

--class State extends State.Context St, IsRunning St (Program.v)
  --protected isYourProgramRunning : IsRunning St (Program.v)

--instance [State St] : IsRunning St (Program.v) := State.isYourProgramRunning


inductive Label where
  | writeTheProgram (prog: Program.T St)
  | invokeOnTheData (data: Data.T St)
--  | takeAsInput (prog: Program.T St) (data: Data.T St)
  | produceOutput

--instance Label.instNonempty : Nonempty (Label St) := .intro (.produceOutput)

instance instLTS : Cslib.LTS St (Label St) where
  Tr st1 μ st2 :=
    match μ with
    --| .takeAsInput prog data => TakeAsInput St prog data st1 st2
    | .writeTheProgram prog => --WriteTheProgram St prog st1 st2
      if req: WriteTheProgram.Req _ prog st1 then Ability.writeTheProgram.run prog st1 req = st2 else False
    | .invokeOnTheData data =>
      if req: InvokeOnTheData.Req _ data st1 then Ability.invokeOnTheData.run data st1 req = st2 else False
    | .produceOutput => --ProduceOutput St st1 st2
      if req: ProduceOutput.Req _ st1 then Ability.produceOutput.run st1 req = st2 else False

      --(InvokeOnTheData St data st1 st2) ∧ (ImmediatelyBeginsRunning.MainEns St st1 st2)
theorem writeTheProgram_exists (prog: Program.T St) (st1: St) (req: WriteTheProgram.Req _ prog st1)
  : ∃st2, ((instLTS _).Tr st1 (.writeTheProgram prog) st2) ∧ (WriteTheProgram.Ens _ prog st2) := by
  simp [instLTS] ; exists req ; apply Subtype.property

theorem invokeOnTheData_exists (data: Data.T St) (st1: St) (req: InvokeOnTheData.Req _ data st1)
  : ∃st2, ((instLTS _).Tr st1 (.invokeOnTheData data) st2) ∧ (InvokeOnTheData.Ens _ data st1 st2) := by
  simp [instLTS] ; exists req ; apply Subtype.property

theorem produceOutput_exists (st1: St) (req: ProduceOutput.Req _ st1)
  : ∃st2, ((instLTS _).Tr st1 (.produceOutput) st2) ∧ (ProduceOutput.Ens _ st2) := by
  simp [instLTS] ; exists req ; apply Subtype.property

theorem TakeAsInput.simulatable prog data (stF: St) (req: TakeAsInput.Req St prog data)
  : ∃stL, TakeAsInput.Ens _ prog data stL := by
  simp [TakeAsInput.Req] at req
  obtain ⟨_,_⟩ := req
  obtain ⟨st1,st1_tr,_,_⟩ := writeTheProgram_exists _ prog stF (by
    simp [WriteTheProgram.Req] ; trivial
  )
  simp only [IsProgramRunning, Bool.not_eq_true] at *
  obtain ⟨st2,st2_tr,_,_,_⟩ := invokeOnTheData_exists _ data st1 (by
    simp [InvokeOnTheData.Req]
    constructorm* _ ∧ _ <;> subst_eqs <;> trivial
  )
  exists st2
  simp [TakeAsInput.Ens]
  constructorm* _ ∧ _ <;> subst_eqs <;> trivial


def TakeAsInput.simulate : TakeAsInput St where
  run prog data stF req :=
    have ⟨_,_⟩ := req
    let ⟨st1,_,_⟩ := Ability.writeTheProgram.run prog stF (by
      simp [WriteTheProgram.Req] ; trivial
    )
    let ⟨st2,_,_,_⟩ := Ability.invokeOnTheData.run data st1 (by
      simp only [IsProgramRunning, Bool.not_eq_true] at *
      simp [InvokeOnTheData.Req]
      constructorm* _ ∧ _ <;> subst_eqs <;> trivial
    )
    Subtype.mk st2 (by
      simp [TakeAsInput.Ens]
      constructorm* _ ∧ _ <;> subst_eqs <;> trivial
    )

end def_s

end Interpreter



end Nemonuri.Study.StanfordOnline.Compilers.Introduction

end expose_s
end public_s
