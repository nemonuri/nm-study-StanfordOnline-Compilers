module

public import Nemonuri.Study.StanfordOnline.Compilers.Introduction._1_1_Interpreter

public section public_s
@[expose] section expose_s

namespace Nemonuri.Study.StanfordOnline.Compilers.Introduction

namespace Compiler

universe u

/-!

#### 1.2. Compiler

Now a compiler is structured differently.

1. The **Compiler**(*self*) takes as input just your **Program**(*prog*).
-/
variable {TCompiler: Type u} {self: TCompiler}
variable {TProgram: Type u} {prog: TProgram}

abbrev CanTakeAsInput := Interpreter.CanTakeAsInput

/-!
2. And then it produces an **Executable**(*exec*).
-/
variable {TExecutable: Type u} {exec: TExecutable}

class CanProduceExecutable (TExecutable: Type u) (TSource: Type u) where
  produceExecutable (self: TSource) : TExecutable

/-!
3. And this executable(*exec*) is another **Program**, might be assembly language, it might be bytecode.
4. It could be in any number of different implementation languages.
-/
variable [e2p: Coe TExecutable TProgram]
variable {TDil: Type u}

inductive ProgramLanguage (TDil: type_of% TDil) where
  | assemblyLanguage
  | bytecode
  | differentImplementationLanguage (x: TDil)

class HasProgramLanguage (TProgram: type_of% TProgram) (TDil: type_of% TDil) where
  toProgramLanguage (self: TProgram) : ProgramLanguage TDil

variable [hpl: HasProgramLanguage TProgram TDil]

/-!
5. But now this can be run separately on your data. And that will produce the output. Okay?
-/
protected inductive OtherWriter where
  | compiler


end Compiler

end Nemonuri.Study.StanfordOnline.Compilers.Introduction

end expose_s
end public_s
