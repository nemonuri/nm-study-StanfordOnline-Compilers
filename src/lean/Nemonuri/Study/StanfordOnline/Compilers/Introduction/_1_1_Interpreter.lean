module

--public import Std.Data.ExtDHashMap
public import Nemonuri.Study.StanfordOnline.Compilers.Introduction.Program

public section public_s
@[expose] section expose_s

namespace Nemonuri.Study.StanfordOnline.Compilers.Introduction

open Program Runtime State

namespace Interpreter

universe u

/-!

#### 1.1. Interpreter

So, what does an interpreter do?

1. You wrote a **Program**(*prog*).
-/
variable (TProgram: Type u) (prog: TProgram)

/-!
2. You have a **Data**(*data*), whatever you want to run the program on.
-/
variable (TData: Type u) (data: TData)

/-!
3. An **Interpreter**(*self*) takes as input, your program(*prog*) and your data(*data*).
-/
protected abbrev Specs.Aux.TakesAsInput := λ _₁ ↦ CanTakeInput (.mk TProgram TData _₁)

/-!
4. It(*self*) produces the **Output**(*output*) directly.
-/
variable (TOutput: Type u) (output: TOutput)

protected abbrev Specs.ProduceOutput := CanProduceOutput (.mk TProgram TData TOutput)
protected abbrev Specs.TakesAsInput := Specs.Aux.TakesAsInput TOutput


/-!
5. Meaning that it(*self*) *doesn't do any processing~* of the program(*prog*) *~before* it executes it on the input.
6. So you just write the program(*prog*), and you invoke the interpreter(*self*) on the data(*data*), and the program(*prog*) immediately begins running.
-/
protected abbrev Specs.doesn't_do_any_processing_before
  (self: Specs.TakesAsInput TProgram TData TOutput)
  (state: State (.mk TProgram TData TOutput) .init)
  (data: TData)
  : Prop :=
  match self.takeInput state data with
  | .takeInput (.init prog1) _ prog2 => prog1 = prog2


/-!
7. we can say that the interpreter(*self*) is, *is online*, meaning it the work that it does is all part of running your program.
-/
@[reducible]
def Specs.is_online
  (self: Specs.ProduceOutput TProgram TData TOutput)
  (state: State (.mk TProgram TData TOutput) .takeInput)
  : Prop :=
  match self.produceOutput state with
  | .produceOutput (.takeInput _ _ prog1) _ prog2 => prog1 = prog2


/-
structure Traces.Invoke (TProgram: Type u) (TData: Type u) (TInterpreter: type_of% TInterpreter) where
  takeAsInput: TInterpreter TProgram TData .takeAsInput
  produceOutput: TInterpreter TProgram TData .produceOutput

@[reducible]
def Specs.is_well_formed
  (label: StateLabel)
  (self: TInterpreter TProgram TData label)
  (prog: TProgram) (data: TData)
  : Prop :=
  match label with
  | .init => Specs.doesn't_do_any_processing_before self prog data
  | .takeAsInput => Specs.is_online TOutput self
  | .produceOutput => True

def invoke
  (wf: ∀label (self: TInterpreter TProgram TData label) prog data, Specs.is_well_formed TOutput label self prog data)
  (self: TInterpreter TProgram TData .init)
  (prog: TProgram) (data: TData) :
  { post: TOutput × (Traces.Invoke TProgram TData TInterpreter)
    // Tests.doesn't_do_any_processing_before prog post.snd.takeAsInput ∧
       Tests.is_online post.snd.takeAsInput post.snd.produceOutput }
  :=
  let trTakeAsInput := ctai.takeAsInput self prog data
  match meq1: CanProduceOutput.produceOutput trTakeAsInput with
  | ⟨(output: TOutput), trProduceOutput⟩ =>
    let postVal := Prod.mk output (Traces.Invoke.mk trTakeAsInput trProduceOutput)
    have lemma1 : Tests.doesn't_do_any_processing_before prog postVal.snd.takeAsInput := by
      unfold Specs.is_well_formed at wf
      unfold Specs.doesn't_do_any_processing_before at wf
      replace wf := wf _ self prog data
      simp at wf
      congr
    have lemma2 : Tests.is_online postVal.snd.takeAsInput postVal.snd.produceOutput := by
      unfold Specs.is_well_formed at wf
      unfold Specs.is_online at wf
      replace wf := wf _ trTakeAsInput prog data
      simp at wf
      subst postVal
      simp_all
    Subtype.mk postVal (And.intro lemma1 lemma2)
-/


end Interpreter

open Interpreter in
class Interpreter (tc: TypeContext)
  extends
    toProduceOutput: Specs.ProduceOutput tc.TProgram tc.TData tc.TOutput,
    toTakesAsInput: Specs.TakesAsInput tc.TProgram tc.TData tc.TOutput
  where
    doesn't_do_any_processing_before :
      ∀state data, Specs.doesn't_do_any_processing_before tc.TProgram tc.TData tc.TOutput toTakesAsInput state data
    is_online :
      ∀state, Specs.is_online tc.TProgram tc.TData tc.TOutput toProduceOutput state




end Nemonuri.Study.StanfordOnline.Compilers.Introduction

end expose_s
end public_s
