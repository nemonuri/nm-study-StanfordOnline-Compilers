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

variable {TInterpreter: (TProgram: Type u) → (TData: Type u) → StateLabel → Type u}
variable {itp: (st: StateLabel) → (TInterpreter TProgram TData st)}

class CanTakeAsInput (TSelf: type_of% (TInterpreter TProgram TData)) where
  takeAsInput (self: TSelf .init) (prog: TProgram) (data: TData) : TSelf .takeAsInput

/-!
4. It(*self*) produces the **Output**(*output*) directly.
-/
variable {TOutput: Type u} {output: TOutput}

class CanProduceOutput (TOutput: Type u) (TSelf: type_of% (TInterpreter TProgram TData)) where
  produceOutput (self: TSelf .takeAsInput) : TOutput × TSelf .produceOutput

/-!
5. Meaning that it(*self*) *doesn't do any processing~* of the program(*prog*) *~before* it executes it on the input.
-/
class HasProgram (TProgram: Type u) (TSource: Type u) where
  toProgram (self: TSource) : TProgram

section
variable
  [ctai: CanTakeAsInput (TInterpreter TProgram TData)]
  [hp1: HasProgram TProgram (TInterpreter TProgram TData .takeAsInput)]

@[reducible]
def Specs.doesn't_do_any_processing_before
  (self1: TInterpreter TProgram TData .init) (prog1: TProgram) (data1: TData) : Prop :=
  prog1 = (ctai.takeAsInput self1 prog1 data1 |> hp1.toProgram)

@[reducible]
def Tests.doesn't_do_any_processing_before
  (prog1: TProgram) (self2: TInterpreter TProgram TData .takeAsInput) : Prop :=
  prog1 = (self2 |> hp1.toProgram)

/-!
6. So you just write the program(*prog*), and you invoke the interpreter(*self*) on the data(*data*), and the program(*prog*) immediately begins running.
-/

structure Traces.Invoke (TProgram TData: Type u) (TInterpreter: type_of% TInterpreter) where
  takeAsInput: TInterpreter TProgram TData .takeAsInput
  produceOutput: TInterpreter TProgram TData .produceOutput

variable
  (TOutput: Type u)
  [cpo: CanProduceOutput TOutput (TInterpreter TProgram TData)]
  [hp2: HasProgram TProgram (TInterpreter TProgram TData .produceOutput)]


set_option pp.explicit true in
set_option trace.Meta.synthInstance true in
def invoke
  (self: TInterpreter TProgram TData .init)
  (wf: ∀prog data, Specs.doesn't_do_any_processing_before self prog data)
  (prog: TProgram) (data: TData) :
  { post: TOutput × (Traces.Invoke TProgram TData TInterpreter)
    // Tests.doesn't_do_any_processing_before prog post.snd.takeAsInput }
  :=
  let trTakeAsInput := ctai.takeAsInput self1 prog data
  match meq1: CanProduceOutput.produceOutput trTakeAsInput with
  | ⟨output, trProduceOutput⟩ =>
  sorry



end




end interpreter



end Nemonuri.Study.StanfordOnline.Compilers.Introduction

end expose_s
end public_s
