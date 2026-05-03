module


public section public_s
@[expose] section expose_s

namespace Nemonuri.Study.StanfordOnline.Compilers.Introduction

/-!

## 01-01: Introduction

### 1. Intro to Compilers

There are two major approaches to implementing programming languages

1. compilers
2. interpreters

-/

inductive ImplementationApproach
  | compiler
  | interpreter

namespace interpreter

universe u

/-!

#### 1. Interpreter

So, what does an interpreter do?

1. You wrote a **Program**(*prog*).
-/
variable {TProgram: Type u} {prog: TProgram}

/-!
2. You have a **Data**(*data*), whatever you want to run the program on.
-/
variable {TData: Type u} {data: TData}

/-!
3. An **Interpreter**(*self*) takes as input, your program(*prog*) and your data(*data*).
-/
inductive StateLabel where
  | init
  | takeAsInput
  | produceOutput
  deriving DecidableEq

variable {TInterpreter: (TProgram: Type u) → (TData: Type u) → StateLabel → Type u}
variable {itp: (st: StateLabel) → (TInterpreter TProgram TData st)}

class CanTakeAsInput (TProgram TData: Type u) (TInterpreter: type_of% TInterpreter) where
  takeAsInput (self: TInterpreter TProgram TData .init) (prog: TProgram) (data: TData) : TInterpreter TProgram TData .takeAsInput

/-!
4. It(*self*) produces the **Output**(*output*) directly.
-/
variable {TOutput: Type u} {output: TOutput}

class CanProduceOutput (TOutput: Type u) (TSelf: type_of% (TInterpreter TProgram TData)) where
  produceOutput (self: TSelf .takeAsInput) : TOutput × TSelf .produceOutput

/-!
5. Meaning that it(*self*) *doesn't do any processing~* of the program(*prog*) *~before* it executes it on the input.
6. So you just write the program(*prog*), and you invoke the interpreter(*self*) on the data(*data*), and the program(*prog*) immediately begins running.
-/
class HasProgram (TProgram: Type u) (TSource: Type u) where
  toProgram (self: TSource) : TProgram


variable
  [ctai: CanTakeAsInput TProgram TData TInterpreter]
  [hp1: HasProgram TProgram (TInterpreter TProgram TData .takeAsInput)]


@[reducible]
def Tests.doesn't_do_any_processing_before
  (prog1: TProgram) (self2: TInterpreter TProgram TData .takeAsInput) : Prop :=
  prog1 = (self2 |> HasProgram.toProgram)

@[reducible]
def Specs.doesn't_do_any_processing_before
  (self1: TInterpreter TProgram TData .init) (prog1: TProgram) (data1: TData) : Prop :=
  Tests.doesn't_do_any_processing_before prog1 (CanTakeAsInput.takeAsInput self1 prog1 data1)

/-!
7. we can say that the interpreter(*self*) is, *is online*, meaning it the work that it does is all part of running your program.
-/
variable
  (TOutput: Type u)
  [cpo: CanProduceOutput TOutput (TInterpreter TProgram TData)]
  [hp2: HasProgram TProgram (TInterpreter TProgram TData .produceOutput)]

@[reducible]
def Tests.is_online
  (self1: TInterpreter TProgram TData .takeAsInput)
  (self2: TInterpreter TProgram TData .produceOutput) : Prop :=
  hp1.toProgram self1 = hp2.toProgram self2

@[reducible]
def Specs.is_online
  (self1: TInterpreter TProgram TData .takeAsInput) : Prop :=
  self1
  |> cpo.produceOutput
  |> (·.snd)
  |> Tests.is_online self1



structure Traces.Invoke (TProgram TData: Type u) (TInterpreter: type_of% TInterpreter) where
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
      exact wf
    Subtype.mk postVal (And.intro lemma1 lemma2)



end interpreter



end Nemonuri.Study.StanfordOnline.Compilers.Introduction

end expose_s
end public_s
