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
  another_program (st: St) : embedding (Executable.v st) ≠ (Program.v st)

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

def isYourProgramRunning (st: St) : Bool := st |> Program.v |> IsRunning.isRunning

def isExecutableRunning (st: St) : Bool := st |> Executable.v |> ExecutableIsProgram.embedding |> IsRunning.isRunning

def CanBeRunSeparatelyOnData : Prop :=
  ∃(st: St),
      (Data.v st ≠ 0) ∧
      (st |> isYourProgramRunning _ |> not) ∧
      (st |> isExecutableRunning _)

export Interpreter (Output Output.T Output.v)

variable
  [Output St] [Zero (Output.T St)]

structure WillProduceTheOutput (st1 st2: St) : Prop where
  pre: (isYourProgramRunning _ st1 |> not) ∧ (isExecutableRunning _ st1) ∧ (Data.v st1 ≠ 0) ∧ (Output.v st1 = 0)
  post: (isYourProgramRunning _ st2 |> not) ∧ (Output.v st2 ≠ 0)

/-!
6. And so in this structure the compiler **is offline**, Meaning that we **pre-process** the program first.
-/
def IsOffline (st: St) : Prop := (Program.v st ≠ 0) → (Executable.v st = 0) → ¬(isYourProgramRunning _ st)

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
  ∀(st1: St), (Data.v st1 ≠ 0) ∧ (isExecutableRunning _ st1) →
  ∀(st2: St), (Data.v st2 ≠ 0) ∧ (isExecutableRunning _ st2) ∧ (Data.v st1 ≠ Data.v st2) →
    (Executable.v st1 = Executable.v st2) →
  ∃(ls: List La), (lts.MTr st1 ls st2) ∧ (Executable.v st1 = Executable.v st2) ∧
  ∃(ss: List St), (lts.Execution st1 ls st2 ss) ∧ (ss.Pairwise (fun stL stR => (Data.v stL) = (Data.v stR)))

#print CanRunSameMany

end spec_s

section def_s

protected class abbrev State.Executable (St: Type us) := Executable St, Zero (Executable.T St)

class State (St: Type us) extends
  toInterpreterState: Interpreter.State St,
  toStateExecutable: State.Executable St,
  ExecutableIsProgram St,
  Language St

variable (St: Type us) [State St]





end def_s


end Compiler

end Nemonuri.Study.StanfordOnline.Compilers.Introduction

end expose_s
end public_s
