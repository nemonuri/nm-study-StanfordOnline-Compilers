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
  protected z: Zero T
  protected v: PropertyBuilder St T

attribute [reducible, instance] YourProgram.z

variable [YourProgram St]

instance : DecidableEq (YourProgram.T St) := YourProgram.v.deq


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
  protected z: Zero T
  protected v: PropertyBuilder St T

attribute [reducible, instance] Executable.z

variable [Executable St]

instance : DecidableEq (Executable.T St) := Executable.v.deq


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
  protected z: Zero T
  ofYourProgram: PropertyBuilder (YourProgram.T St) T
  ofExecutable: PropertyBuilder (Executable.T St) T
  another_program (prog: YourProgram.T St) (exe: Executable.T St) : (ofYourProgram prog ≠ 0) → (ofYourProgram prog) ≠ (ofExecutable exe)

attribute [reducible, instance] Program.z

variable [Program St]

instance : DecidableEq (Program.T St) := Program.ofYourProgram.deq


inductive Language.MightBe (α: Type _) where
  | assembly
  | bytecode
  | diffrent (a: α)

class Language.{u} (P: Type u) [Zero P] where
  protected TLanguage: Type u
  protected toZero: Zero TLanguage
  protected language: PropertyBuilder P TLanguage --(p: P) → (Language.MightBe TLanguage)

attribute [reducible, instance] Language.toZero

variable [Language (Program.T St)]


/-!
5. But now this can be run separately on your data. And that will produce the output. Okay?
-/
protected class IsRunning.State where
  protected T: Type us
  protected z: Zero T
  protected v: PropertyBuilder St T

attribute [reducible, instance] IsRunning.State.z

variable [IsRunning.State St]

instance : DecidableEq (IsRunning.State.T St) := IsRunning.State.v.deq
instance : Zero Bool where zero := false

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
  protected z: Zero T
  protected v: PropertyBuilder St T

attribute [reducible, instance] Data.z

variable [Data St]

instance : DecidableEq (Data.T St) := Data.v.deq


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
  protected z: Zero T
  protected v: PropertyBuilder St T

attribute [reducible, instance] Output.z

variable [Output St]

instance : DecidableEq (Output.T St) := Output.v.deq


namespace WillProduceTheOutput

@[reducible]
def Req (st1: St) : Prop := (IsExecutableRunning _ st1) ∧ (Output.v st1 = 0)

def Ens (st1 st2: St) : Prop := (AppEq Executable.v st1 st2) ∧ (Output.v st2 ≠ 0)

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

def ReqPre (st1: St) : Prop := (Executable.v st1 ≠ 0)

def ReqAll (st1 stX: St) : Prop := (Executable.v st1 = Executable.v stX)

def ReqPost (st2: St) : Prop := (Data.v st2 ≠ 0) ∧ (IsExecutableRunning _ st2)

def ReqAny (st2 stX: St) : Prop := (Data.v stX ≠ 0) ∧ (IsExecutableRunning _ st2) ∧ (Data.v stX ≠ Data.v st2)

end CanRunSameMany

open CanRunSameMany in
def CanRunSameMany {La} (lts: Cslib.LTS St La) : Prop :=
  ∀(st1: St), (ReqPre _ st1) →
  ∃(st2: St), (ReqPost _ st2) ∧
  ∃sts μs, (List.Forall (ReqAll _ st1) sts) ∧
            (∃stX ∈ sts, ReqAny _ st2 stX) ∧
            (lts.Execution st1 μs st2 sts)


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
/-
  intro data data_p st1 req_pre stL req_last sts sts_ne sts_head_eq sts_last_eq req_all --data data_p --sts sts_p st2 st2_p
  --conv at req_all => rw [IsOffline.ReqAll]
  --unfold IsOffline.ReqAll at req_all ; simp [appEq_iff] at req_all
  simp [IsOffline.ReqFirst] at req_pre
  simp [IsOffline.ReqLast, IsExecutableRunning] at req_last
  --simp [IsOffline.ReqPre] at req_pre
  obtain ⟨_,_,_,_⟩ := req_pre
  --simp [IsOffline.ReqAll, appEq_iff]
  --simp [IsOffline.ReqPost, IsExecutableRunning]
  --simp [IsOffline.EnsAll, IsExecutableRunning]
  obtain ⟨st2, ⟨st2_tr, st2_ens⟩⟩ := produceExecutable_exists _ st1 (by simp; trivial)
  simp [ProduceExecutable.Ens, RunningStateEq, appEq_iff] at st2_ens
  obtain ⟨⟨_,_⟩,⟨_,_⟩⟩ := st2_ens
  have st2_lm1 : IsOffline.ReqAll _ st1 st2 := by simp [IsOffline.ReqAll, appEq_iff, *]
  obtain ⟨st3, ⟨st3_tr, st3_ens⟩⟩ := takeData_exists St data st2 (by simp; exact data_p)
  simp [TakeData.Ens, appEq_iff] at st3_ens
  obtain ⟨_,_,_,_⟩ := st3_ens
  have st3_lm1 : IsOffline.ReqAll _ st2 st3 := by simp [IsOffline.ReqAll, appEq_iff, *]
  have stL_intro (req1: BeginRun.Req _ st3) (req2: IsOffline.ReqAll _ st3 stL)
    : (instLTS _).Tr st3 (.beginRun) stL := by
    simp [instLTS]
    exists req1
    simp only [Subtype.coe_eq_iff]
    have lm1: BeginRun.Ens St st3 stL := by
      simp [BeginRun.Ens, appEq_iff]
      simp [BeginRun.Req] at req1
      obtain ⟨_,_,_,_⟩ := req1
      simp [IsOffline.ReqAll, appEq_iff] at req2
      constructorm* _ ∧ _ <;> simp_all
      --simp_all
    --simp [BeginRun.Req] at req1
    --obtain ⟨_,_,_,_⟩ := req1
    --simp [IsOffline.ReqAll, appEq_iff] at req2
    --simp_all [instLTS]
-/

/-
  obtain ⟨st4, ⟨st4_tr, st4_ens⟩⟩ := beginRun_exists St st3 (by
    subst data
    simp_all
    simp [BeginRun.Req]
    constructorm* _ ∧ _ <;> trivial
  )
  simp [BeginRun.Ens, appEq_iff] at st4_ens
  obtain ⟨_,_,_,_,_⟩ := st4_ens
  --simp [h1, h2, h3] at * ; clear h1 h2 h3
-/
/-
  have lm1 := Execution.refl (instLTS _) st4 |>.stepL st4_tr |>.stepL st3_tr |>.stepL st2_tr
  obtain ⟨sts_head, sts_tail, sts_eq⟩ := List.ne_nil_iff_exists_cons.mp sts_ne
  have lm2 : (∃ x, (instLTS St).Execution st1 x stL sts) := by
    simp_all
-/

  --simp_all
  --MTr.single _ st2_tr |>.stepR _ st3_tr |>.stepR _ st4_tr
  --subst_eqs
  --simp [List.forall_iff_forall_mem, IsOffline.ReqAll, appEq_iff] at sts_p
  --simp [List.forall_iff_forall_mem, IsOffline.EnsAll, IsExecutableRunning]
/-
  simp [IsOffline.ReqPre] at req_pre
  obtain ⟨_,_,_,_⟩ := req_pre
  simp [IsOffline.ReqPost, IsExecutableRunning]
  obtain ⟨st2, ⟨st2_tr, st2_ens⟩⟩ := produceExecutable_exists _ st1 (by simp; trivial)
  simp [ProduceExecutable.Ens, RunningStateEq, appEq_iff] at st2_ens
  obtain ⟨⟨_,_⟩,⟨_,_⟩⟩ := st2_ens
  simp_all
-/





  --obtain ⟨st2, ⟨st2_tr, st2_ens⟩⟩ := (TakeData.req_iff _ data st1).mp req.left
  --simp [TakeData.Ens] at st2_ens
  /-
  have lm_st3_1 (h: Executable.v st2 = 0) := (ProduceExecutable.req_iff _ st2).mp (by
    obtain ⟨h1_1, h1_2, h1_3⟩ := req
    obtain ⟨h2_1, h2_2, h2_3⟩ := st2_ens
    simp [ProduceExecutable.Req] ; rw [← h2_2] ; simp only [h1_2]
    simp [IsYourProgramRunning, IsNonZeroRunning] at h1_3
    simp_all
  )
  -/
/-
    if h: (Executable.v st2 = 0) then
      st2
    else
      st2
-/
/-
  have lm_st3 := (BeginRun.req_iff _ st2).mp (by
    simp [BeginRun.Req]
    constructor
    · rw [← st2_ens.left] ; exact req.left
    · constructor
      · simp [IsYourProgramRunning, IsNonZeroRunning] at req
  )
-/


    --simp [BeginRun.Req]
    --conv => arg 1; rw [eq_self_iff_true]; tactic => have := And.intro req.left st2_ens.left;
  --obtain ⟨st3, ⟨st3_tr, st3_ens⟩⟩ := (BeginRun.req_iff _ st1).mp req.left
  --obtain ⟨st2, st2_tr⟩ := (TakeData.req_iff _ data st1).mp req.left |> isImageNonempty_iff.mp



  --have lm1 : (instLTS _).IsImageNonempty st1 (.takeData data) := by
  --  simp [isImageNonempty_iff, instLTS]
/-
    match h1: Ability.toTakeData.dreq data with
    | .isTrue _ => simp
    | .isFalse cont =>
      simp [TakeData.Req] at cont
      have := req.left
      contradiction
-/
  --simp [isImageNonempty_iff] at lm1
  --obtain ⟨st2, st2_p⟩ := isImageNonempty_iff.mp lm1
/-
    have req1 : TakeData.Req St data := by simp [TakeData.Req] ; exact req.left
    exists (Ability.toTakeData.run data st1 req1).val
    simp
-/

  --intro data st1 _ _ _ stX _ _ _ _ _
  --simp_all only [not_false_eq_true, forall_const]



/-
open Cslib LTS in
attribute [simp]
  CanReach IsNonZeroRunning IsYourProgramRunning IsExecutableRunning RunningStateEq
  Nemonuri.zeroEq_iff Nemonuri.sourceExists_iff Cslib.LTS.isImageNonempty_iff
in
example (data: Data.T St) (st1: St) (h: ∃st2, (RunSeparately _ data st1 st2))
  : (instLTS _).CanReach st1 h.choose := by
  rename (State St) => st
  have lm1 := h.choose_spec
  simp [runSeparately_iff, RunSeparately.Post] at lm1
  obtain ⟨⟨lm1_1, lm1_2⟩, _, _, _, _⟩ := lm1
  by_cases h1: st1 = h.choose
  · simp [←h1]; exists []; exact MTr.refl
  · obtain ⟨st3, st3_p⟩ := st.toStateData.source_exists _ lm1_1
    have lm2 : (instLTS _).IsImageNonempty st1 (.takeData data) := by
      simp [instLTS, RunSeparately.takeData_iff]
  --obtain ⟨st3, st3_p⟩ := st.toStateProgram.toSourceExists.source_exists _ lm1_2
  --unfold Exists.choose
  --rename (State St) => st
-/
/-
  obtain ⟨lm_prog1, lm_prog2⟩ := st.toStateProgram.toIsProperty.imp_and
  obtain ⟨lm_data1, lm_data2⟩ := st.toStateData.toIsProperty.imp_and
  obtain ⟨lm_exe1, lm_exe2⟩ := st.toStateExecutable.toIsProperty.imp_and
  simp [runSeparately_iff]  at h
  let (eq := h_eq) ⟨hw, hw_p⟩ := h
  simp at lm_prog1 lm_prog2 lm_data1 lm_data2 lm_exe1 lm_exe2
  simp [runSeparately_iff] at hw_p
-/
/-
  simp [runSeparately_iff] at *
  obtain ⟨⟨h1, ⟨_, _⟩⟩, ⟨_, _, ⟨_, _, _, _⟩⟩⟩ := h
  have lm1 : ∃st3, (instLTS _).Tr st1 (.takeData data) st3 ∧ (instLTS _).Tr st3 (.beginRun) st2 := by
    have lm1_1 : (instLTS _).IsImageNonempty st1 (.takeData data) := by
      simp [instLTS, RunSeparately.takeData_iff]

  suffices goal: ∃stg, ((instLTS _).CanReach st1 stg) ∧ (ProduceExecutable.Post _ st1 stg) ∧ (Not <| IsExecutableRunning _ st2) from by
    simp [ProduceExecutable.Post] at goal
    obtain ⟨stg, ⟨stg_mtr, ⟨⟨_, h2⟩, _⟩⟩⟩ := goal
    simp [ExecutableIsProgram.zero_iff] at h2
    have lm2 : (instLTS _).IsImageNonempty stg (.takeData data) := by
      simp [instLTS, RunSeparately.takeData_iff]
      simp [*]
      --exists ((lm_data2 data h1) |> Classical.choose)
    obtain ⟨st4, st4_tr⟩ := lm2 |> isImageNonempty_iff.mp
  by_cases h8: Executable.v st1 = 0
  · have lm1 : (instLTS _).IsImageNonempty st1 .produceExecutable := by
      simp [instLTS, produceExecutable_iff]
      constructor <;> constructor <;> trivial
    obtain ⟨st3, st3_tr⟩ := lm1 |> isImageNonempty_iff.mp
    clear lm1
    have lm2 : (instLTS _).IsImageNonempty st3 (.takeData data) := by
      simp [instLTS, RunSeparately.takeData_iff]
      constructor <;> constructor <;> trivial
    obtain ⟨st4, st4_tr⟩ := lm2 |> isImageNonempty_iff.mp
    clear lm2
    have lm3 : (instLTS _).Tr st4 .beginRun st2 := by
      simp [instLTS, RunSeparately.beginRun_iff]
      constructor <;> constructor <;> trivial
    simp only [← MTr.single_iff] at st3_tr
    have lm4 := MTr.stepR (instLTS _) st3_tr st4_tr |> (MTr.stepR (instLTS _) · lm3)
    simp at lm4
    exists [Label.produceExecutable, Label.takeData data, Label.beginRun]
    --have lm2 : (instLTS _).Tr st1 .produceExecutable lm1_st := by
    --  simp [Membership.mem, Set.Mem] at lm1_p

  · sorry
      --by_contra cont
      --simp at cont
-/
/-
    exists [.produceExecutable, .takeData data, .beginRun]
    have lm1 : (instLTS _).image st1 .produceExecutable |> Set.Nonempty := by
      simp [instLTS, image, Set.Nonempty]
      constructor
      · constructor <;> constructor
        · exact h2.left
        · exact h6
        ·
-/
    --rw [← List.singleton_append]
    --rw [MTr.split_iff]
    --let (eq := h7) image1 := (instLTS _).image st1 .produceExecutable
    --simp [instLTS, image, setOf] at h7



/-
    let w : List (Label St) := [.takeData data, .beginRun]
    exists w
    have lm2 : w = ([.takeData data] ++ [.beginRun]) := by subst w ; simp
    subst w ; rw [lm2]
    apply (MTr.split_iff (instLTS _)).mpr
    simp [MTr.single_iff]
    simp [instLTS]
    open RunSeparately in
    constructor
    · constructor <;> constructor <;> constructor
      · exact h1
      · constructor
-/


    --apply (MTr.split_iff (instLTS _)).mpr




end def_s


end Compiler

end Nemonuri.Study.StanfordOnline.Compilers.Introduction

end expose_s
end public_s
