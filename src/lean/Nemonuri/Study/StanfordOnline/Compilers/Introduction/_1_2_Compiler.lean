module

public import Nemonuri.Study.StanfordOnline.Compilers.Introduction.Program
public import Nemonuri.Study.StanfordOnline.Compilers.Introduction._1_1_Interpreter

public section public_s
@[expose] section expose_s

namespace Nemonuri.Study.StanfordOnline.Compilers.Introduction

open Program Runtime State

namespace Compiler

universe u
set_option autoImplicit false

/-!

#### 1.2. Compiler

Now a compiler is structured differently.

1. The **Compiler**(*self*) takes as input just **Your Program**(*yourProg*).
-/
variable (TYourProgram: Type u) (yourProg: TYourProgram)

protected abbrev Specs.TakeYourProgram := λ _₁ ↦ TakeInput (.mk TYourProgram PUnit _₁)

/-!
2. And then it produces an **Executable**(*exec*).
-/
variable (TExecutable: Type u) (exec: TExecutable)

protected abbrev Specs.ProduceExecutable := ProduceOutput (.mk TYourProgram PUnit TExecutable)

/-!
3. And this executable(*exec*) is another **Program**, might be assembly language, it might be bytecode.
4. It could be in any number of different implementation languages.
-/
protected inductive Program (TYourProgram TExecutable: Type u) where
  | yourProgram (yourProg: TYourProgram)
  | executable (exec: TExecutable)

protected structure TypeContext : Type (u+1) where
  TYourProgram : Type u
  TExecutable : Type u
  TData : Type u
  TOutput : Type u

protected abbrev TypeContext.normalize (tc: Compiler.TypeContext) : TypeContext :=
  .mk (Compiler.Program tc.TYourProgram tc.TExecutable) tc.TData tc.TOutput

protected instance TypeContext.instCoe : Coe Compiler.TypeContext TypeContext where
  coe := Compiler.TypeContext.normalize

protected class Specs.HasOtherLanguageType (TExecutable: Type u) where
  otherLanguageType : Type u

inductive LanguageOfExecutable (TExecutable: Type u) [inst: Specs.HasOtherLanguageType TExecutable] where
  | assemblyLanguage
  | bytecode
  | otherLanguage (lang: inst.otherLanguageType)


/-!
5. But now this can be run separately on your data. And that will produce the output. Okay?
-/
protected class SeparateRuntime (tc: TypeContext)
  extends
    Runtime tc
  where
  program : tc.TProgram
  init_spec2 {label: Label} (pre: State tc label) :
    ∀prog, match init pre prog with | .init prog2 => program = prog2
  takeInput_spec2 :
    ∀state data, Interpreter.Specs.doesn't_do_any_processing_before tc.TProgram tc.TData tc.TOutput toTakeInput state data
  produceOutput_spec2 :
    ∀state, Interpreter.Specs.is_online tc.TProgram tc.TData tc.TOutput toProduceOutput state


protected class Specs.RunSeparately (TExecutable TData TOutput: Type u) where
  runSeparately (exec: TExecutable) : (Compiler.SeparateRuntime (.mk TExecutable TData TOutput))

/-!
6. And so in this structure the compiler **is offline**, Meaning that we **pre-process** the program first.
-/
protected class PreProcess (TYourProgram TExecutable: Type u) where
  preProcess (yourProg: TYourProgram) : TExecutable

/-!
7. The compiler is **essentially~** a **~pre-processing** step that produces the executable,
-/
protected def Specs.is_offline
  (tc: Compiler.TypeContext)
  (self: TakeInput ↑tc)
  (state: State ↑tc .init)
  (data: tc.TData)
  : Prop :=
  match self.takeInput state data with
  | .takeInput (.init prog1) _ prog2 =>
  match prog1, prog2 with
  | .yourProgram _, .executable _ => True
  | _, _ => False


variable (TData TOutput: Type u)

protected class Specs.EssentiallyPreProcess --(TYourProgram TExecutable: Type u)
  extends
    TakeInput ↑(Compiler.TypeContext.mk TYourProgram TExecutable TData TOutput)
  where
  is_offline state data : Specs.is_offline _ toTakeInput state data

/-!
8. we can run that same executable on many, many different inputs,
on many different data sets without having to recompile or do any other processing of
the program
-/



end Compiler

end Nemonuri.Study.StanfordOnline.Compilers.Introduction

end expose_s
end public_s
