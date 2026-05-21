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
public import Nemonuri.Function


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
export Interpreter (Program Program.T Program.v Data Data.T Data.v)
export Interpreter (IsRunning IsRunning.isRunning)


variable (St: Type us) [Zero St]
variable [Program St] [PropertyOf St (Program.v)]
variable [Data St] [PropertyOf St (Data.v)]
variable [IsRunning St Program.v]

/-
def TakeAsInput.Req (prog: Program.T St) (st1: St) : Prop :=
  (prog ≠ 0) ∧ (Program.v st1 = 0)

@[mk_iff]
structure TakeAsInput.Ens (prog: Program.T St) (st1 st2: St) : Prop where
  pre: TakeAsInput.Req _ prog st1
  post: (Program.v st2 = prog) ∧ (Data.v st2 = Data.v st1)

def TakeAsInput : Prop :=
  (h: ∃(prog: Program.T St) (st1: St), TakeAsInput.Req _ prog st1) →
  (∃st2, TakeAsInput.Ens _ h.choose h.choose_spec.choose st2)
-/

namespace TakeAsInput

def Req (prog: Program.T St) (st1: St) : Prop :=
  (prog ≠ 0) ∧ (Program.v st1 ≠ 0 → ¬IsRunning.isRunning Program.v st1)

def Ens (prog: Program.T St) (st2: St) : Prop :=
  (Program.v st2 = prog) ∧ (¬IsRunning.isRunning Program.v st2)

end TakeAsInput

open TakeAsInput in
structure TakeAsInput where
  run (prog: Program.T St) (st1: St) (req: Req _ prog st1)
    : { st2: St // Ens _ prog st2 }

--#print TakeAsInput

/-!
2. And then it produces an **Executable**(*exec*).
-/
class Executable where
  protected T: Type us
  protected v: St → T

variable [Executable St] [PropertyOf St (Executable.v)]

namespace ProduceExecutable

def Req (st1: St) : Prop := (Program.v st1 ≠ 0) ∧ (¬IsRunning.isRunning Program.v st1)

def Ens (st1 st2: St) : Prop := (AppEq Program.v st1 st2) ∧ (Executable.v st2 ≠ 0) ∧ (¬IsRunning.isRunning Program.v st1)

end ProduceExecutable

open ProduceExecutable in
structure ProduceExecutable where
  run (st1: St) (req: Req _ st1) : { st2: St // Ens _ st1 st2 }


/-!
3. And this executable(*exec*) is another **Program**, might be assembly language, it might be bytecode.
4. It could be in any number of different implementation languages.
-/
open Nemonuri Function in
protected class Program.Extend where
  protected T: Type us
  ofProgram : { t: Program.T St // IsInRange Program.v t } → T --(s: Program.T St) (req: IsInRange Program.v s)
  ofExecutable : { t: Executable.T St // IsInRange Executable.v t } → T

variable [Program.Extend St] [Zero (Program.Extend.T St)]


namespace Program.Extend

instance : ZeroEq ofProgram where



end Program.Extend

  --[ZeroEq (@Program.Extend.ofProgram St _ _ _)]
  --[ZeroEq (@Program.Extend.ofExecutable St _ _ _)]

--#print programOfExecutable

--[Property (@ExecutableIsProgram.toProgram St _ _ _ _ _)]
--def programOfExecutable [Proper] := --Program.v (Executable.v st)

/-
class ExecutableIsProgram where
  embedding: Function.Embedding (Executable.T St) (Program.T St)
  embedding_zero : (embedding 0 = 0)
  another_program (st: St) : (Executable.v st) ≠ 0 → embedding (Executable.v st) ≠ (Program.v st)
-/

/-
omit [Data St] [Zero (Data.T St)] [Zero St] [IsProperty (@Program.v St _)] [IsProperty (@Executable.v St _)] [IsRunning St] in
open ExecutableIsProgram Function in
theorem ExecutableIsProgram.zero_iff [ExecutableIsProgram St] (st: St)
  : ((Executable.v st) = 0) ↔ (embedding (Executable.v st) = 0) := by
  constructor
  · intro h
    rw [h]
    exact embedding_zero
  · intro h
    rewrite [← embedding_zero] at h
    apply embedding.injective
    exact h
-/

/-
open ExecutableIsProgram Function in
def ExecutableIsProgram.embedding_zero_iff [ExecutableIsProgram St] (st: St)
  : (Executable.v st) = 0 ↔ (embedding (Executable.v st) = 0) :=
  Iff.intro (ExecutableIsProgram.embedding_zero st) (by
    intro h
    --have lm1 : Injective (embedding: Embedding (Executable.T St) (Program.T St)) := embedding.injective
    --unfold Injective at lm1
    rw [← Injective.eq_iff embedding.injective]
    rw [h] -- 0 = embedding 0
    symm

    )
-/

inductive Language.MightBe (α: Type _) where
  | assembly
  | bytecode
  | diffrent (a: α)

class Language.{u} (P: Type u) where
  protected TLanguage: Type u
  protected language: (p: P) → (Language.MightBe TLanguage)

variable [Language (Program.Extend.T St)]


/-!
5. But now this can be run separately on your data. And that will produce the output. Okay?
-/

--namespace IsRunning

--def IsNonZeroRunning (st: St) (v: St → Program.T St) : Prop := (v st) ≠ 0 ∧ IsRunning.isRunning st (v st)

--end IsRunning

--def IsYourProgramRunning (st: St) : Prop := IsNonZeroRunning _ st (Program.v)

--def IsExecutableRunning (st: St) : Prop := IsNonZeroRunning _ st (Executable.v · |> ExecutableIsProgram.embedding)

/-
def RunningStateEq (st1 st2: St) : Prop :=
  (IsYourProgramRunning _ st1 = IsYourProgramRunning _ st2) ∧
  (IsExecutableRunning _ st1 = IsExecutableRunning _ st2)
-/
--variable [IsRu]

def IsYourProgramRunning (st: St) : Prop := IsRunning.isRunning Program.v st

def IsExecutableRunning (st: St) : Prop := IsRunning.isRunning (Program.ofExecutable _) st

namespace CanRunSeparately


def ReqPre (data: Data.T St) (st1: St) : Prop :=
  (data ≠ 0) ∧ (Program.v st1 ≠ 0) ∧ (¬IsYourProgramRunning _ st1)

def ReqPost (data: Data.T St) (st1 st2: St) : Prop :=
  (Data.v st2 = data) ∧ (Program.v st2 = Program.v st1) ∧ (¬IsYourProgramRunning _ st2) ∧ (IsExecutableRunning _ st2)

end CanRunSeparately

open CanRunSeparately in
def CanRunSeparately {La} (lts: Cslib.LTS St La) : Prop :=
  ∀(data: Data.T St) (st1: St), (ReqPre _ data st1) →
  ∃(st2: St), (ReqPost _ data st1 st2) ∧
  lts.CanReach st1 st2


export Interpreter (Output Output.T Output.v)

variable
  [Output St] [Zero (Output.T St)]

namespace WillProduceTheOutput

def Req (st1: St) : Prop := (IsExecutableRunning _ st1) ∧ (Output.v st1 = 0)

def Ens (st1 st2: St) : Prop := (AppEq Executable.v st1 st2) ∧ (Output.v st2 ≠ 0)

end WillProduceTheOutput

open WillProduceTheOutput in
structure WillProduceTheOutput where
  run (st1: St) (req: Req _ st1) : { st2: St // Ens _ st1 st2 }


/-!
6. And so in this structure the compiler **is offline**, Meaning that we **pre-process** the program first.
-/
namespace IsOffline

def ReqPre (st1: St) : Prop :=
  (Program.v st1 ≠ 0) ∧ (Executable.v st1 = 0) ∧
  (¬IsYourProgramRunning _ st1) ∧ (¬IsExecutableRunning _ st1)

def ReqAll (st1 stX: St) : Prop := (AppEq Program.v st1 stX)

def ReqPost (st2: St) : Prop := IsExecutableRunning _ st2

def EnsAll (stX: St) : Prop :=
  ¬(Executable.v stX = 0) ∧ (IsExecutableRunning _ stX)

end IsOffline

open IsOffline in
def IsOffline {La} (lts: Cslib.LTS St La) : Prop :=
  ∀(st1: St), (ReqPre _ st1) →
  ∃(st2: St), (ReqPost _ st2) ∧
  ∃sts μs, (List.Forall (ReqAll _ st1) sts) ∧ (lts.Execution st1 μs st2 sts) ∧
    List.Forall (EnsAll _) sts

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

def Req (data: Data.T St) : Prop := data ≠ 0

def Ens (data: Data.T St) (st1 st2: St) : Prop :=
  (data = Data.v st2) ∧
  (Program.v st1 = Program.v st2) ∧
  (Executable.v st1 = Executable.v st2)

end TakeData

open TakeData in
structure TakeData where
  run (data: Data.T St) (st1: St) (req: Req _ data) : { st2: St // Ens _ data st1 st2 }


namespace BeginRun

def Req (st1: St) : Prop := (Data.v st1 ≠ 0) ∧ (Executable.v st1 ≠ 0) ∧ (Not <| IsExecutableRunning _ st1)

def Ens (st1 st2: St) : Prop :=
  (Data.v st1 = Data.v st2) ∧
  (Program.v st1 = Program.v st2) ∧
  (Executable.v st1 = Executable.v st2) ∧
  (IsExecutableRunning _ st1)

end BeginRun

open BeginRun in
structure BeginRun where
  run (st1: St) (req: Req _ st1) : { st2: St // Ens _ st1 st2 }


end spec_s

section def_s

variable (St: Type us) [Zero St]

class State.Context
  extends
    toInterpreterStateContext: Interpreter.State.Context St
  where
  protected executable : Property.Context St

section ctx_s

variable [ctx: State.Context St]

instance : Executable St := ⟨ctx.executable.T, ctx.executable.v⟩

instance : PropertyOf St (Executable.v) := (inferInstanceAs (Property ctx.executable.v))

end ctx_s

--protected class abbrev State.Executable := Executable St, Property (@Executable.v St _)

open Interpreter in
class State
  extends
    State.Context St,
    Language (Program.T St),
    ExecutableToProgram St
  where
  protected isYourProgramRunning : IsRunning St (Program.v)
  protected isExecutableRunning : IsRunning St (Program.ofExecutable St)
  --protected isYourProgramRunning : IsRunning St (Program.v)

variable [State St]


protected structure Ability.TakeAsInput extends TakeAsInput St where
  dreq (prog: Program.T St) (st: St) : Decidable (TakeAsInput.Req _ prog st)

protected structure Ability.ProduceExecutable extends ProduceExecutable St where
  dreq (st: St) : Decidable (ProduceExecutable.Req _ st)

protected structure Ability.WillProduceTheOutput extends WillProduceTheOutput St where
  dreq (st: St) : Decidable (WillProduceTheOutput.Req _ st)

protected structure Ability.TakeData extends TakeData St where
  dreq (data: Data.T St) : Decidable (TakeData.Req _ data)

protected structure Ability.BeginRun extends BeginRun St where
  dreq (st: St) : Decidable (BeginRun.Req _ st)

class Ability where
  toTakeAsInput: Ability.TakeAsInput St
  toProduceExecutable : Ability.ProduceExecutable St
  toWillProduceTheOutput : Ability.WillProduceTheOutput St
  toTakeData : Ability.TakeData St
  toBeginRun : Ability.BeginRun St

variable [Ability St]

inductive Label where
  | takeAsInput (prog: Program.T St)
  | produceExecutable
  | willProduceTheOutput --InvokeOnTheData
  | takeData (data: Data.T St)
  | beginRun

#print Set.coe_setOf

instance instLTS : Cslib.LTS St (Label St) where
  Tr st1 l st2 :=
    match l with
    | .takeAsInput prog =>
      match Ability.toTakeAsInput.dreq prog st1 with
      | .isTrue req => Ability.toTakeAsInput.run prog st1 req = st2 | _ => False
    | .produceExecutable => --(ProduceExecutable _ st1 st2) ∧ (Not <| IsExecutableRunning _ st2)
      match Ability.toProduceExecutable.dreq st1 with
      | .isTrue req => Ability.toProduceExecutable.run st1 req = st2 | _ => False
    | .willProduceTheOutput => --WillProduceTheOutput _ st1 st2
      match Ability.toWillProduceTheOutput.dreq st1 with
      | .isTrue req => Ability.toWillProduceTheOutput.run st1 req = st2 | _ => False
    | .takeData data => --RunSeparately.TakeData _ data st1 st2
      (Ability.toTakeData.dreq data).byCases
        (fun req => Ability.toTakeData.run data st1 req = st2) (fun _ => False)
    | .beginRun =>
      (Ability.toBeginRun.dreq st1).byCases
        (fun req => Ability.toBeginRun.run st1 req = st2) (fun _ => False)

    --| .beginRun => RunSeparately.BeginRun _ st1 st2

open Cslib LTS

attribute [scoped simp] isImageNonempty_iff

theorem TakeData.req_iff (data: Data.T St) (st1: St)
  : TakeData.Req St data ↔ ∃st2, ((instLTS _).Tr st1 (.takeData data) st2 ∧ TakeData.Ens _ data st1 st2) := by
  simp [instLTS, Decidable.byCases]
  match Ability.toTakeData.dreq data with
  | .isTrue req =>
    simp [req]
    exact (Ability.toTakeData.run data st1 req).property
  | .isFalse h => simp [h, exists_const]

/-
set_option pp.proofs true in
theorem TakeData.Req.imp_ens (data: Data.T St) (st1: St) (req: TakeData.Req St data)
  : ∃st2, TakeData.Ens St data st1 st2 := by
  obtain ⟨st2, st2_p⟩ := Ability.toTakeData.run data st1 req
  exists st2
-/
  --have (eq := lm1) req := h |> (TakeData.req_iff _ data st1).mpr
  --let (eq := lm2) st2E := Ability.toTakeData.run data st1 req
  --apply @Exists.choose_spec _ (fun x => Ens St data st1 x)
  --subst lm1
  --rw [Classical.choose_eq st2]
  --rewrite [← TakeData.req_iff] at *
  --rename_i h2
  --have lm1 :
  --have lm1 := TakeData.req_iff _ data st1
  --change TakeData.Req St data at h
  --have lm1 := isImageNonempty_iff.mpr ⟨st2, h⟩ |> (TakeData.req_iff _ data st1).mpr
  --let _ := Ability.toTakeData.run data st1 lm1

theorem ProduceExecutable.req_iff (st1: St)
  : ProduceExecutable.Req _ st1 ↔ ∃st2, ((instLTS _).Tr st1 (.produceExecutable) st2 ∧ ProduceExecutable.Ens _ st1 st2) := by
  simp [instLTS]
  match Ability.toProduceExecutable.dreq st1 with
  | .isTrue req =>
    simp [req]
    exact (Ability.toProduceExecutable.run st1 req).property
  | .isFalse h => simp [h, exists_const]

theorem BeginRun.req_iff (st1: St)
  : BeginRun.Req _ st1 ↔ ∃st2, ((instLTS _).Tr st1 (.beginRun) st2 ∧ BeginRun.Ens _ st1 st2) := by
  simp [instLTS, Decidable.byCases]
  match Ability.toBeginRun.dreq st1 with
  | .isTrue req =>
    simp [req]
    exact (Ability.toBeginRun.run st1 req).property
  | .isFalse h => simp [h, exists_const]

set_option pp.proofs true in
example : CanRunSeparately St (instLTS _) := by
  simp [CanRunSeparately]
  intro data st1 req
  simp [CanReach]
  simp [CanRunSeparately.ReqPre] at req
  simp [CanRunSeparately.ReqPost]
  obtain ⟨st2, ⟨st2_tr, st2_ens⟩⟩ := (TakeData.req_iff _ data st1).mp req.left
  simp [TakeData.Ens] at st2_ens
  have lm_st3_1 (h: Executable.v st2 = 0) := (ProduceExecutable.req_iff _ st2).mp (by
    obtain ⟨h1_1, h1_2, h1_3⟩ := req
    obtain ⟨h2_1, h2_2, h2_3⟩ := st2_ens
    simp [ProduceExecutable.Req] ; rw [← h2_2] ; simp only [h1_2]
    simp [IsYourProgramRunning, IsNonZeroRunning] at h1_3
    simp_all
  )
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
