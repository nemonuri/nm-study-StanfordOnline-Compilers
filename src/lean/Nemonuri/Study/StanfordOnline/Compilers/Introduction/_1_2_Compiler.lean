module

public import Nemonuri.Study.StanfordOnline.Compilers.Introduction.Basic
public import Nemonuri.Study.StanfordOnline.Compilers.Introduction._1_1_Interpreter
public import Mathlib.Logic.Embedding.Basic
public import Cslib.Foundations.Semantics.LTS.Execution


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

variable
  (St: Type us)
  [Program St] [Zero (Program.T St)]
  [Data St] [Zero (Data.T St)]

structure TakeAsInput (prog: Program.T St) (_ st2: St) : Prop where
  pre: (prog ≠ 0)
  post: (Program.v st2 = prog) ∧ (Data.v st2 = 0)

/-!
2. And then it produces an **Executable**(*exec*).
-/
class Executable (St: Type us) where
  protected T: Type us
  protected v: (st: St) → T

variable [Executable St] [Zero (Executable.T St)]

structure ProduceExecutable (st1 st2: St) : Prop where
  pre: (Program.v st1 ≠ 0) ∧ (Executable.v st1 = 0)
  post: (Program.v st1 = Program.v st2) ∧ (Executable.v st2 ≠ 0)

/-!
3. And this executable(*exec*) is another **Program**, might be assembly language, it might be bytecode.
4. It could be in any number of different implementation languages.
-/
class ExecutableIsProgram where
  embedding: Function.Embedding (Executable.T St) (Program.T St)
  embedding_zero (st: St) : (embedding 0 = 0)
  another_program (st: St) : (Executable.v st) ≠ 0 → embedding (Executable.v st) ≠ (Program.v st)

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

variable [ExecutableIsProgram St]

inductive Language.MightBe (α: Type _) where
  | assembly
  | bytecode
  | diffrent (a: α)

class Language.{u} (P: Type u) where
  protected TLanguage: Type u
  protected language: (p: P) → (Language.MightBe TLanguage)

variable [Language (Program.T St)]


/-!
5. But now this can be run separately on your data. And that will produce the output. Okay?
-/
export Interpreter (IsRunning IsRunning.isRunning)

variable [IsRunning St]

--namespace IsRunning

def IsNonZeroRunning (prog: Program.T St) : Prop := prog ≠ 0 ∧ IsRunning.isRunning prog

--end IsRunning

def IsYourProgramRunning (st: St) : Prop := st |> Program.v |> IsNonZeroRunning _

def IsExecutableRunning (st: St) : Prop := st |> Executable.v |> ExecutableIsProgram.embedding |> IsNonZeroRunning _

structure RunSeparately (data: Data.T St) (st1 st2: St) : Prop where
  pre: (data ≠ 0) ∧ (Program.v st1 ≠ 0) ∧ (IsExecutableRunning _ st1 |> Not)
  post: (Data.v st2 = data) ∧
        (st2 |> IsYourProgramRunning _ |> Not) ∧
        (st2 |> IsExecutableRunning _)
/-
  ∃(st2: St),
      (Data.v st2 ≠ 0) ∧
      (st2 |> isYourProgramRunning _ |> not) ∧
      (st2 |> isExecutableRunning _)
-/

export Interpreter (Output Output.T Output.v)

variable
  [Output St] [Zero (Output.T St)]

structure WillProduceTheOutput (st1 st2: St) : Prop where
  pre: (IsYourProgramRunning _ st1 |> Not) ∧ (IsExecutableRunning _ st1) ∧ (Data.v st1 ≠ 0) ∧ (Output.v st1 = 0)
  post: (IsYourProgramRunning _ st2 |> Not) ∧ (Output.v st2 ≠ 0)

/-!
6. And so in this structure the compiler **is offline**, Meaning that we **pre-process** the program first.
-/
structure IsOffline (st1 st2 st3: St) : Prop where
  pre: (Program.v st1 ≠ 0) ∧ (Executable.v st1 = 0) ∧ (IsExecutableRunning _ st1 |> Not)
  mid: (Program.v st2 = Program.v st1) ∧ (Executable.v st2 ≠ 0) ∧ (IsExecutableRunning _ st2 |> Not)
  post: (Program.v st3 = Program.v st2) ∧ (Executable.v st3 = Executable.v st2) ∧ (IsExecutableRunning _ st3)


/-!
7. The compiler is essentially a pre-processing step that produces the executable,
-/
def PreProcessProducesExecutable {La} (lts: Cslib.LTS St La) : Prop :=
  ∀(st2: St), (Executable.v st2 ≠ 0) → ∃(st1: St), (ProduceExecutable _ st1 st2) ∧ (lts.CanReach st1 st2)

/-!
8. we can run that same executable on many, many different inputs,
on many different data sets without having to recompile or do any other processing of
the program
-/

def CanRunSameMany {La} (lts: Cslib.LTS St La) : Prop :=
  ∀(st1: St), (Data.v st1 ≠ 0) ∧ (IsExecutableRunning _ st1) →
  ∀(st2: St), (Data.v st2 ≠ 0) ∧ (IsExecutableRunning _ st2) ∧ (Data.v st1 ≠ Data.v st2) →
    (Executable.v st1 = Executable.v st2) →
  ∃(ls: List La), (Executable.v st1 = Executable.v st2) ∧ (lts.MTr st1 ls st2) ∧
  ∃(ss: List St), (ss.Pairwise (fun stL stR => (Data.v stL) = (Data.v stR))) ∧ (lts.Execution st1 ls st2 ss)

namespace RunSeparately

protected structure TakeData (data: Data.T St) (st1 st2: St) where
  pre: (data ≠ 0) ∧ (IsExecutableRunning _ st1 |> Not) ∧ (IsYourProgramRunning _ st1 |> Not)
  post: (Data.v st2 = data) ∧ (Executable.v st1 = Executable.v st2) ∧ (Program.v st1 = Program.v st2)

@[ext]
protected structure BeginRun (st1 st2: St) where
  pre: (Data.v st1 ≠ 0) ∧ (Executable.v st1 ≠ 0) ∧ (IsExecutableRunning _ st1 |> Not)
  post: (Data.v st2 = Data.v st1) ∧ (IsExecutableRunning _ st1)

end RunSeparately

end spec_s

section def_s

protected class abbrev State.Executable (St: Type us) := Executable St, Zero (Executable.T St)

class State (St: Type us) extends
  toInterpreterState: Interpreter.State St,
  toStateExecutable: State.Executable St,
  ExecutableIsProgram St,
  Language St

variable (St: Type us) [State St]

inductive Label where
  | takeAsInput (prog: Program.T St)
  | produceExecutable
  | willProduceTheOutput --InvokeOnTheData
  | takeData (data: Data.T St)
  | beginRun

instance instLTS : Cslib.LTS St (Label St) where
  Tr st1 l st2 :=
    match l with
    | .takeAsInput prog => TakeAsInput _ prog st1 st2
    | .produceExecutable => ProduceExecutable _ st1 st2
    | .willProduceTheOutput => WillProduceTheOutput _ st1 st2
    | .takeData data => RunSeparately.TakeData _ data st1 st2
    | .beginRun => RunSeparately.BeginRun _ st1 st2

#print RunSeparately

--attribute [ext] Cslib.LTS.MTr.split

open Cslib LTS in
attribute [simp] CanReach IsNonZeroRunning IsYourProgramRunning IsExecutableRunning in
example (data: Data.T St) (st1 st2: St) (h: RunSeparately _ data st1 st2)
  : (instLTS _).CanReach st1 st2 := by
  obtain ⟨h_pre, h_post⟩ := h
  obtain ⟨h1, h2⟩ := h_pre
  obtain ⟨h3, h4, h5⟩ := h_post
  simp at *
  by_cases h6: Executable.v st1 = 0
  · have lm1 := (ExecutableIsProgram.embedding_zero _).mp h6
    simp [lm1] at *
    clear lm1
    exists [.takeData data]
  ·
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
