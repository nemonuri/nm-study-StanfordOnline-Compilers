module

public import Nemonuri.Study.StanfordOnline.Compilers.Introduction.Basic
public import Nemonuri.Study.StanfordOnline.Compilers.Introduction._1_1_Interpreter
public import Mathlib.Logic.Embedding.Basic
public import Cslib.Foundations.Semantics.LTS.Execution
public import Mathlib.Tactic.MkIffOfInductiveProp
public import Mathlib.Logic.Basic
public import Cslib.Foundations.Semantics.LTS.Relation
public import Mathlib.Data.Subtype
public import Mathlib.Logic.ExistsUnique
public import Mathlib.Tactic.Lift
public import Nemonuri.Builder
public import Mathlib.Data.List.Pairwise


public section public_s
@[expose] section expose_s

namespace Nemonuri.Study.StanfordOnline.Compilers.Introduction


namespace Compiler

universe us
set_option autoImplicit false

section spec_s

/-!

#### 1.2. Compiler

Now a compiler is structured differently.

1. The **Compiler**(*self*) takes as input just **Your Program**(*yourProg*).
-/
variable (St: Type us) [Zero St]

class YourProgram where
  protected T: Type us
  protected ctx: PropertyContext T
  protected v: Property St T

attribute [reducible, instance] YourProgram.ctx

variable [YourProgram St]


namespace TakeAsInput

@[reducible]
def Req (prog: YourProgram.T St) (st1: St) : Prop :=
  (prog ≠ 0) ∧ (YourProgram.v st1 ≠ 0) -- → ¬IsRunning.isRunning Program.v st1


def Ens (prog: YourProgram.T St) (st2: St) : Prop :=
  (YourProgram.v st2 = prog) --∧ (¬IsRunning.isRunning Program.v st2)

end TakeAsInput

open TakeAsInput in
structure TakeAsInput where
  run (prog: YourProgram.T St) (st1: St) (req: Req _ prog st1)
    : { st2: St // Ens _ prog st2 }

--#print TakeAsInput

/-!
2. And then it produces an **Executable**(*exec*).
-/
class Executable where
  protected T: Type us
  protected ctx: PropertyContext T
  protected v: Property St T

attribute [reducible, instance] Executable.ctx

variable [Executable St]


namespace ProduceExecutable

@[reducible]
def Req (st1: St) : Prop := (YourProgram.v st1 ≠ 0) --∧ (¬IsRunning.isRunning Program.v st1)

def Ens (st1 st2: St) : Prop := (AppEq YourProgram.v st1 st2) ∧ (Executable.v st2 ≠ 0) --∧ (¬IsRunning.isRunning Program.v st1)

end ProduceExecutable

open ProduceExecutable in
structure ProduceExecutable where
  run (st1: St) (req: Req _ st1) : { st2: St // Ens _ st1 st2 }


/-!
3. And this executable(*exec*) is another **Program**, might be assembly language, it might be bytecode.
4. It could be in any number of different implementation languages.
-/
class Program where
  protected T: Type us
  protected ctx: PropertyContext T
  ofYourProgram: Property (YourProgram.T St) T
  ofExecutable: Property (Executable.T St) T
  another_program (prog: YourProgram.T St) (exe: Executable.T St) : (ofYourProgram prog ≠ 0) → (ofYourProgram prog) ≠ (ofExecutable exe)

attribute [reducible, instance] Program.ctx

variable [Program St]


inductive Language.MightBe (α: Type _) [Zero α] where
  | assembly
  | bytecode
  | diffrent (a: α)
  deriving DecidableEq

instance {α} [Zero α] : Zero (Language.MightBe α) where
  zero := Language.MightBe.diffrent 0

class Language.{u} (P: Type u) [Zero P] where
  protected T: Type u
  protected ctx : PropertyContext T
  protected language: Property P (Language.MightBe T) --(p: P) → (Language.MightBe TLanguage)

attribute [reducible, instance] Language.ctx

variable [Language (Program.T St)]


/-!
5. But now this can be run separately on your data. And that will produce the output. Okay?
-/
protected class IsRunning.State where
  protected T: Type us
  protected ctx : PropertyContext T
  protected v: Property St T

attribute [reducible, instance] IsRunning.State.ctx

variable [IsRunning.State St]


class IsRunning where
  protected v : ZeroHom (IsRunning.State.T St) (ZeroHom (Program.T St) Bool) --PropertyBuilder St (RunningTester St)
  --isRunning: PropertyBuilder St (PropertyBuilder (Program.T St) (ULift Bool))

variable [IsRunning St]

@[reducible]
def IsYourProgramRunning (st: St) : Prop := IsRunning.v (IsRunning.State.v st) (Program.ofYourProgram (YourProgram.v st))

@[reducible]
def IsExecutableRunning (st: St) : Prop := IsRunning.v (IsRunning.State.v st) (Program.ofExecutable (Executable.v st))

@[reducible]
def IsAnyProgramRunning (st: St) : Prop := IsYourProgramRunning _ st ∨ IsExecutableRunning _ st


class Data where
  protected T: Type us
  protected ctx : PropertyContext T
  protected v: Property St T

attribute [reducible, instance] Data.ctx

variable [Data St]



namespace CanRunSeparately


def ReqFirst (data: Data.T St) (stF: St) : Prop :=
  (data ≠ 0) ∧ (YourProgram.v stF ≠ 0) ∧ (¬IsAnyProgramRunning _ stF)

def EnsLast (data: Data.T St) (stF stL: St) : Prop :=
  (Data.v stL = data) ∧ (AppEq YourProgram.v stL stF) ∧ (¬IsYourProgramRunning _ stL) ∧ (IsExecutableRunning _ stL)

end CanRunSeparately

open CanRunSeparately in
def CanRunSeparately {La} (lts: Cslib.LTS St La) : Prop :=
  ∀(data: Data.T St) (stF: St), (ReqFirst _ data stF) →
  ∃(stL: St), (lts.CanReach stF stL) ∧
    (EnsLast _ data stF stL)


class Output where
  protected T: Type us
  protected ctx : PropertyContext T
  protected v: Property St T

attribute [reducible, instance] Output.ctx

variable [Output St]



namespace WillProduceTheOutput

@[reducible]
def Req (st1: St) : Prop := (IsExecutableRunning _ st1) --∧ (Output.v st1 = 0)

def Ens (st1 st2: St) : Prop :=
  (¬IsExecutableRunning _ st1) ∧
  (AppEq YourProgram.v st1 st2) ∧
  (AppEq Executable.v st1 st2) ∧
  (Output.v st2 ≠ 0)

end WillProduceTheOutput

open WillProduceTheOutput in
structure WillProduceTheOutput where
  run (st1: St) (req: Req _ st1) : { st2: St // Ens _ st1 st2 }


/-!
6. And so in this structure the compiler **is offline**, Meaning that we **pre-process** the program first.
-/
/-
namespace IsOffline

def ReqPre (st1: St) : Prop :=
  (YourProgram.v st1 ≠ 0) ∧ (Executable.v st1 = 0) ∧
  (¬IsYourProgramRunning _ st1) ∧ (¬IsExecutableRunning _ st1)

def ReqAll (st1 stX: St) : Prop := (AppEq YourProgram.v st1 stX)

def ReqPost (st2: St) : Prop := IsExecutableRunning _ st2

def EnsAll (stX: St) : Prop :=
  ¬(Executable.v stX = 0) ∧ (IsExecutableRunning _ stX)

end IsOffline

open IsOffline in
def IsOffline {La} (lts: Cslib.LTS St La) : Prop :=
  ∀(st1: St), (ReqPre _ st1) →
  ∀(data: Data.T St), (data ≠ 0) →
  ∀(st2: St), (ReqPost _ st2) →
  ∃sts μs,
    (∀stX ∈ sts, ReqAll _ st1 stX) ∧
    (lts.Execution st1 μs st2 sts) ∧
    (∀stX ∈ sts, EnsAll _ stX)
-/

namespace IsOffline

def ReqFirst (stF: St) : Prop :=
  (YourProgram.v stF ≠ 0) ∧ (Executable.v stF = 0) ∧ --(Data.v stF ≠ 0) ∧
  (¬IsYourProgramRunning _ stF) ∧ (¬IsExecutableRunning _ stF)

def ReqLast (stL: St) : Prop := IsExecutableRunning _ stL

def ReqAll (stX1 stX2: St) : Prop := (AppEq YourProgram.v stX1 stX2)

def EnsAll (stX: St) : Prop :=
  ¬((Executable.v stX = 0) ∧ (IsExecutableRunning _ stX))

end IsOffline

open IsOffline in
def IsOffline {La} (lts: Cslib.LTS St La) : Prop :=
  --∀(data: Data.T St), (data ≠ 0) →
  ∀(stF: St), (ReqFirst _ stF) →
  ∀(stL: St), (ReqLast _ stL) →
  ∀(sts: List St), ∀(ls: List La), (lts.Execution stF ls stL sts) →
  (sts.Pairwise (ReqAll St)) →
  (∀stX ∈ sts, EnsAll _ stX)
/-
    (sts_ne: sts ≠ []) →
    (sts.head sts_ne = stF) →
    (sts.getLast sts_ne = stL) →
    (sts.Pairwise (ReqAll St)) →
  ∃ls, (lts.Execution stF ls stL sts) ∧ (∀stX ∈ sts, EnsAll _ stX)
-/

/-!
7. The compiler is essentially a pre-processing step that produces the executable,
8. we can run that same executable on many, many different inputs,
on many different data sets without having to recompile or do any other processing of
the program
-/
namespace CanRunSameMany

def ReqData (dataF dataL: Data.T St) : Prop := (dataF ≠ 0) ∧ (dataL ≠ 0) ∧ (dataF ≠ dataL)

def ReqFirst (dataF: Data.T St) (stF: St) : Prop := (Data.v stF = dataF) ∧ (IsExecutableRunning _ stF)

def EnsAll (stX1 stX2: St) : Prop := (AppEq Executable.v stX1 stX2) ∧ (AppEq YourProgram.v stX1 stX2)

def EnsLast (dataL: Data.T St) (stL: St) : Prop := (Data.v stL = dataL) ∧ (IsExecutableRunning _ stL)

instance : IsTrans St (EnsAll _) where
  trans stF stM stL := by --h_fm h_ml
    simp [EnsAll, appEq_iff]
    intro _ _ _ _
    constructor <;> simp [*]

end CanRunSameMany

open CanRunSameMany in
def CanRunSameMany {La} (lts: Cslib.LTS St La) : Prop :=
  ∀dataF dataL, (ReqData _ dataF dataL) →
  ∀stF, (ReqFirst _ dataF stF) →
  ∃ls stL sts, (lts.Execution stF ls stL sts) ∧
    (sts.Pairwise (EnsAll _)) ∧
    (EnsLast _ dataL stL)
--  ∀(dataF dataL: Data.T St), (dataF ≠ 0) → (data2 ≠ 0) → (data1 ≠ data2) →
/-
  ∀(st1: St), (ReqPre _ st1) →
  ∃(st2: St), (ReqPost _ st2) ∧
  ∃sts μs, (List.Forall (ReqAll _ st1) sts) ∧
            (∃stX ∈ sts, ReqAny _ st2 stX) ∧
            (lts.Execution st1 μs st2 sts)
-/

/-
def CanRunSameMany {La} (lts: Cslib.LTS St La) : Prop :=
  ∀(st1: St), (Data.v st1 ≠ 0) ∧ (IsExecutableRunning _ st1) →
  ∀(st2: St), (Data.v st2 ≠ 0) ∧ (IsExecutableRunning _ st2) ∧ (Data.v st1 ≠ Data.v st2) →
    (Executable.v st1 = Executable.v st2) →
  ∃(ls: List La), (Executable.v st1 = Executable.v st2) ∧ (lts.MTr st1 ls st2) ∧
  ∃(ss: List St), (ss.Pairwise (fun stL stR => (Data.v stL) = (Data.v stR))) ∧ (lts.Execution st1 ls st2 ss)
-/

namespace TakeData

@[reducible]
def Req (data: Data.T St) : Prop := data ≠ 0

def Ens (data: Data.T St) (st1 st2: St) : Prop :=
  (data = Data.v st2) ∧
  (AppEq YourProgram.v st1 st2) ∧
  (AppEq Executable.v st1 st2) ∧
  (AppEq IsRunning.State.v st1 st2)

end TakeData

open TakeData in
structure TakeData where
  run (data: Data.T St) (st1: St) (req: Req _ data) : { st2: St // Ens _ data st1 st2 }


namespace BeginRun

@[reducible]
def Req (st1: St) : Prop := (Data.v st1 ≠ 0) ∧ (Executable.v st1 ≠ 0) ∧ (¬IsAnyProgramRunning _ st1)

def Ens (st1 st2: St) : Prop :=
  (AppEq Data.v st1 st2) ∧
  (AppEq YourProgram.v st1 st2) ∧
  (AppEq Executable.v st1 st2) ∧
  (IsYourProgramRunning _ st1 = IsYourProgramRunning _ st2) ∧
  (IsExecutableRunning _ st2)

end BeginRun

open BeginRun in
structure BeginRun where
  run (st1: St) (req: Req _ st1) : { st2: St // Ens _ st1 st2 }


end spec_s

section def_s

variable (St: Type us) [Zero St]

class State where
  toYourProgram: YourProgram St
  toExecutable: Executable St
  toProgram: Program St
  toLanguage: Language (Program.T St)
  toIsRunningState: IsRunning.State St
  toIsRunning: IsRunning St
  toData: Data St
  toOutput: Output St

attribute [instance_reducible, instance]
  State.toYourProgram State.toExecutable State.toProgram State.toLanguage
  State.toIsRunningState State.toIsRunning State.toData State.toOutput


variable [State St]

section dec_s

--set_option pp.explicit true
set_option trace.Meta.synthInstance true
--set_option allowUnsafeReducibility true

instance (prog: YourProgram.T St) (st: St) : Decidable (TakeAsInput.Req _ prog st) := inferInstance

instance (st: St) : Decidable (ProduceExecutable.Req _ st) := inferInstance

instance (st: St) : Decidable (IsExecutableRunning _ st) := inferInstance

instance (st: St) : Decidable (WillProduceTheOutput.Req _ st) := inferInstance

instance (data: Data.T St) : Decidable (TakeData.Req _ data) := inferInstance

instance (st: St) : Decidable (IsAnyProgramRunning _ st) := inferInstance

instance (st: St) : Decidable (BeginRun.Req _ st) := inferInstance

end dec_s


inductive Label where
  | takeAsInput (prog: YourProgram.T St)
  | produceExecutable
  | willProduceTheOutput --InvokeOnTheData
  | takeData (data: Data.T St)
  | beginRun

def RunningStateEq (st1 st2: St) : Prop :=
  (IsYourProgramRunning _ st1 = IsYourProgramRunning _ st2) ∧
  (IsExecutableRunning _ st1 = IsExecutableRunning _ st2)

class Ability where
  takeAsInput: TakeAsInput St
  takeAsInput_spec: ∀prog st1 req, RunningStateEq _ st1 ↑(takeAsInput.run prog st1 req)
  takeData: TakeData St
  produceExecutable: ProduceExecutable St
  produceExecutable_spec: ∀st1 req, RunningStateEq _ st1 ↑(produceExecutable.run st1 req)
  willProduceTheOutput: WillProduceTheOutput St
  beginRun: BeginRun St


instance : DecidableEq (Label St) := fun l1 l2 =>
  if h: l1.ctorIdx = l2.ctorIdx then
    match l1, l2 with
    | .takeAsInput p1, .takeAsInput p2 => decidable_of_iff (p1 = p2) (by simp)
    | .takeData d1, .takeData d2 => decidable_of_iff (d1 = d2) (by simp)
    | .produceExecutable, .produceExecutable
    | .willProduceTheOutput, .willProduceTheOutput
    | .beginRun, .beginRun => .isTrue (by simp) --decidable_of_iff True (by simp)
  else
    .isFalse (by revert h ; contrapose ; intro h2 ; rw [h2])


variable [Ability St]

instance instLTS : Cslib.LTS St (Label St) where
  Tr st1 l st2 :=
    match l with
    | .takeAsInput prog =>
      if req: TakeAsInput.Req _ prog st1 then
        (Ability.takeAsInput.run prog st1 req = st2) ∧ (RunningStateEq _ st1 st2)
      else False
    | .produceExecutable =>
      if req: ProduceExecutable.Req _ st1 then
        (Ability.produceExecutable.run st1 req = st2) ∧ (RunningStateEq _ st1 st2)
      else False
    | .willProduceTheOutput =>
      if req: WillProduceTheOutput.Req _ st1 then Ability.willProduceTheOutput.run st1 req = st2 else False
    | .takeData data =>
      if req: TakeData.Req _ data then Ability.takeData.run data st1 req = st2 else False
    | .beginRun =>
      if req: BeginRun.Req _ st1 then Ability.beginRun.run st1 req = st2 else False

    --| .beginRun => RunSeparately.BeginRun _ st1 st2

open Cslib LTS

theorem produceExecutable_exists (st1: St) (req: ProduceExecutable.Req _ st1)
  : ∃st2, (instLTS _).Tr st1 (.produceExecutable) st2 ∧ ProduceExecutable.Ens _ st1 st2 ∧ RunningStateEq _ st1 st2 := by
  have lm1 := Ability.produceExecutable_spec st1
  simp [instLTS]
  exists req
  simp [lm1]
  apply Subtype.property

theorem takeData_exists (data) (st1: St) (req: TakeData.Req _ data)
  : ∃st2, (instLTS _).Tr st1 (.takeData data) st2 ∧ TakeData.Ens _ data st1 st2 := by
  simp [instLTS]
  exists req
  apply Subtype.property

theorem beginRun_exists (st1: St) (req: BeginRun.Req _ st1)
  : ∃st2, (instLTS _).Tr st1 (.beginRun) st2 ∧ BeginRun.Ens _ st1 st2 := by
  simp [instLTS]
  exists req
  apply Subtype.property

theorem willProduceTheOutput_exists (st1: St) (req: WillProduceTheOutput.Req _ st1)
  : ∃st2, (instLTS _).Tr st1 (.willProduceTheOutput) st2 ∧ WillProduceTheOutput.Ens _ st1 st2 := by
  simp [instLTS]
  exists req
  apply Subtype.property

set_option pp.proofs true in
theorem CanRunSeparately.proof : CanRunSeparately St (instLTS _) := by
  simp [CanRunSeparately]
  intro data st1 req
  simp [CanReach]
  simp [CanRunSeparately.ReqFirst] at req
  simp [CanRunSeparately.EnsLast]
  obtain ⟨st2, ⟨st2_tr, st2_ens⟩⟩ := produceExecutable_exists _ _ req.2.1
  simp [ProduceExecutable.Ens, appEq_iff, RunningStateEq] at st2_ens
  obtain ⟨st3, ⟨st3_tr, st3_ens⟩⟩ := takeData_exists _ _ st2 req.1
  simp [TakeData.Ens, appEq_iff] at st3_ens
  obtain ⟨st4, ⟨st4_tr, st4_ens⟩⟩ := beginRun_exists _ st3 (by
      simp [BeginRun.Req]
      constructorm* _ ∧ _
      · rw [← st3_ens.1] ; exact req.1
      · rw [← st3_ens.2.2.1] ; exact st2_ens.1.2
      · rw [← st3_ens.2.2.2, ← st3_ens.2.1]
        simp only [← st2_ens.2.1]
        exact req.2.2.1
      · rw [← st3_ens.2.2.1, ← st3_ens.2.2.2]
        simp only [← st2_ens.2.2]
        exact req.2.2.2
  )
  simp [BeginRun.Ens, appEq_iff] at st4_ens
  exists st4
  simp [appEq_iff]
  obtain ⟨_,_,_,_⟩ := req
  obtain ⟨⟨_,_⟩,_,_⟩ := st2_ens
  obtain ⟨_,_,_,_⟩ := st3_ens
  obtain ⟨_,_,_,_,_⟩ := st4_ens
  simp_all [IsExecutableRunning]
  open Cslib LTS in
  have lm2 := MTr.single (instLTS _) st2_tr |>.stepR _ st3_tr |>.stepR _ st4_tr
  simp at lm2
  exact Exists.intro _ lm2


theorem IsOffline.proof : IsOffline St (instLTS _) := by
  simp [IsOffline]
  intro stF stF_p stL stL_p sts ls exe exe_p sts_elem sts_mem
  simp [EnsAll]
  intro sts_elem_exe_zero
  simp [sts_elem_exe_zero]
  rfl

attribute [simp] CanRunSameMany.EnsAll appEq_iff in
theorem CanRunSameMany.proof : CanRunSameMany St (instLTS _) := by
  simp [CanRunSameMany]
  rintro dataF dataL ⟨r1,r2,r3⟩ stF ⟨r4,r5⟩
  --simp [IsExecutableRunning] at r5
  --unfold CanRunSameMany.EnsAll
  --simp [appEq_iff]
  simp [CanRunSameMany.EnsLast]
  obtain ⟨st1,st1_tr,_,_,_,_⟩ := willProduceTheOutput_exists _ stF (by simp [WillProduceTheOutput.Req] ; exact r5)
  simp only [appEq_iff] at *
  --have ea_st1 : CanRunSameMany.EnsAll _ stF st1 := by simp [*]
  obtain ⟨st2,st2_tr,_,_,_,_⟩ := takeData_exists _ dataL st1 (by simp [TakeData.Req] ; exact r2)
  simp only [appEq_iff] at *
  --have ea_st2 : CanRunSameMany.EnsAll _ st1 st2 := by simp [*]
  obtain ⟨st3,st3_tr,_,_,_,_,_⟩ := beginRun_exists _ st2 (by
    simp [BeginRun.Req]
    constructorm* _ ∧ _ <;> trivial)
  simp only [appEq_iff] at *
  --have ea_st3 : CanRunSameMany.EnsAll _ st2 st3 := by simp [*]
  have := Execution.refl (instLTS _) st3 |>.stepL st3_tr |>.stepL st2_tr |>.stepL st1_tr
  exists [Label.willProduceTheOutput, Label.takeData dataL, Label.beginRun]



end def_s


end Compiler

end Nemonuri.Study.StanfordOnline.Compilers.Introduction

end expose_s
end public_s
