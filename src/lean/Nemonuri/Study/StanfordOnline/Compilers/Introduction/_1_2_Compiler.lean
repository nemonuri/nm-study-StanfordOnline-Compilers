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
protected class Specs.ExecutableAsProgram (TExecutable: Type u) where
  executableAsProgram : Program TExecutable


protected inductive LanguageOfExecutable.Raw (TExecutable TOtherLanguage: Type u) where
  | assemblyLanguage
  | bytecode
  | otherLanguage (lang: TOtherLanguage)

structure LanguageOfExecutable (TExecutable: Type u) where
  TOtherLanguage: Type u
  raw: LanguageOfExecutable.Raw TExecutable TOtherLanguage



/-!
5. But now this can be run separately on your data. And that will produce the output. Okay?
-/
protected class SeparateRuntime (tc: TypeContext)
  extends
    Runtime tc
  where
  program : tc.TProgram
  takeInput_spec2 label state data :
    Interpreter.Specs.doesn't_do_any_processing_before toTakeInput label state data
  produceOutput_spec2 state :
    Interpreter.Specs.is_online toProduceOutput state


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
  (state: self.ReqState .load)
  (data: tc.TData)
  : Prop :=
  match self.takeInput state data with
  | .takeInput (.load prog1) _ prog2 =>
    match prog1, prog2 with
    | .yourProgram _, .executable _ => True
    | _, _ => False
  | _ => False


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
