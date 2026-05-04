module

public import Std.Data.ExtDHashMap
public import Nemonuri.Study.StanfordOnline.Compilers.Introduction.Programs

public section public_s
@[expose] section expose_s

namespace Nemonuri.Study.StanfordOnline.Compilers.Introduction


namespace Interpreter

open Programs

universe u

/-!

#### 1.1. Interpreter

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

class CanTakeAsInput (TSelf: type_of% TInterpreter) (TProgram TData: Type u) where
  takeAsInput (self: TSelf TProgram TData .init) (prog: TProgram) (data: TData) : TSelf TProgram TData .takeAsInput

/-!
4. It(*self*) produces the **Output**(*output*) directly.
-/
variable {TOutput: Type u} {output: TOutput}

class CanProduceOutput (TSelf: type_of% (TInterpreter TProgram TData)) (TOutput: Type u) where
  produceOutput (self: TSelf .takeAsInput) : TOutput × TSelf .produceOutput

/-!
5. Meaning that it(*self*) *doesn't do any processing~* of the program(*prog*) *~before* it executes it on the input.
6. So you just write the program(*prog*), and you invoke the interpreter(*self*) on the data(*data*), and the program(*prog*) immediately begins running.
-/
abbrev Key := Writer PUnit

class abbrev HasProgramMap (TSelf: Type u) (TProgram: Type u) :=
  Programs.HasProgramMap TSelf Key (Function.const Key TProgram)

variable
  [ctai: CanTakeAsInput TInterpreter TProgram TData]
  [hp1: HasProgramMap (TInterpreter TProgram TData .takeAsInput) TProgram]


@[reducible]
def Tests.doesn't_do_any_processing_before
  (prog1: TProgram) (self2: TInterpreter TProgram TData .takeAsInput) : Prop :=
  let maybeProg := (self2 |> hp1.toHasProgramMap.toProgramMap).get? (Writer.you)
  match maybeProg with
  | .none => False
  | .some p => p = prog1

@[reducible]
def Specs.doesn't_do_any_processing_before
  (self1: TInterpreter TProgram TData .init) (prog1: TProgram) (data1: TData) : Prop :=
  Tests.doesn't_do_any_processing_before prog1 (CanTakeAsInput.takeAsInput self1 prog1 data1)

/-!
7. we can say that the interpreter(*self*) is, *is online*, meaning it the work that it does is all part of running your program.
-/
variable
  (TOutput: Type u)
  [cpo: CanProduceOutput (TInterpreter TProgram TData) TOutput]
  [hp2: HasProgramMap (TInterpreter TProgram TData .produceOutput) TProgram]

@[reducible]
def Tests.is_online
  (self1: TInterpreter TProgram TData .takeAsInput)
  (self2: TInterpreter TProgram TData .produceOutput) : Prop :=
  let tryGet
    (dmap: Std.ExtDHashMap Key (Function.const Key TProgram))
    : Option _ :=
    if dmap.size = 1 then
      dmap.get? (Writer.you)
    else
      .none
  match tryGet (hp1.toProgramMap self1), tryGet (hp2.toProgramMap self2) with
  | .some p1, .some p2 => p1 = p2
  | _, _ => False


@[reducible]
def Specs.is_online
  (self1: TInterpreter TProgram TData .takeAsInput) : Prop :=
  self1
  |> cpo.produceOutput
  |> (·.snd)
  |> Tests.is_online self1


--set_option trace.Meta.synthInstance true in
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



end Interpreter



end Nemonuri.Study.StanfordOnline.Compilers.Introduction

end expose_s
end public_s
