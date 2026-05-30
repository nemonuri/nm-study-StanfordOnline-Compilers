module

public import Nemonuri.Study.StanfordOnline.Compilers.Lemma

@[expose] public section public_s

namespace Nemonuri.Study.StanfordOnline.Compilers

/-!
#### 2.1. Major phase

Recall that a compiler has five major phases,
1. lexical analysis
2. parsing
3. semantic analysis
4. optimization
5. code generation.
-/

inductive MajorPhase where
  | lexicalAnalysis
  | parsing
  | semanticAnalysis
  | optimization
  | codeGeneration
  deriving Repr, DecidableEq, Ord

namespace MajorPhase

protected def all : List MajorPhase := [.lexicalAnalysis, .parsing, .semanticAnalysis, .optimization, .codeGeneration]

open Cslib

def IsValidLts {St} (lts: Cslib.LTS St MajorPhase) : Prop :=
  ∃stF stL sts, lts.Execution stF MajorPhase.all stL sts

end MajorPhase


end Nemonuri.Study.StanfordOnline.Compilers

end public_s
