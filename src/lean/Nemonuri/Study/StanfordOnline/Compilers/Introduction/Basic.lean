module

public import Std.Tactic.Do

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


/-!

#### 1. Interpreter

So, what does an interpreter do?

1. You wrote a **Program**(*prog*).
2. You have a **Data**(*data*), whatever you want to run the program on.
3. An **Interpreter**(*s*) takes as input, your program(*prog*) and your data(*data*).
4. It(*s*) produces the **Output**(*output*) directly.
5. Meaning that it(*s*) *doesn't do any processing* of the program(*prog*) before it executes it on the input.
6. So you just write the program(*prog*), and you invoke the interpreter(*s*) on the data(*data*), and the program(*prog*) immediately begins running.

-/

section interpreter_s

universe u1 u2 u3
variable {TProgram: Type u1} {TData: Type u2} {TOutput: Type u3}

namespace Interpreter


inductive Index where
  | init
  | takeInput
  | produceOutput
  deriving DecidableEq, Hashable, BEq, ReflBEq, LawfulBEq

instance instInhabitedIndex : Inhabited Index where default := Index.init


inductive IndexedState (TProgram: Type u1) (TData: Type u2) (TOutput: Type u3) : Index → Type _ where
  | init : IndexedState TProgram TData TOutput Index.init
  | takeInput
    (pre: IndexedState TProgram TData TOutput Index.init)
    (prog: TProgram) (data: TData)
    : IndexedState TProgram TData TOutput Index.takeInput
  | produceOutput
    (pre: IndexedState TProgram TData TOutput Index.takeInput)
    (prog: TProgram)
    (output: TOutput)
    : IndexedState TProgram TData TOutput Index.produceOutput

instance instInhabitedIndexedState : Inhabited (IndexedState TProgram TData TOutput (Inhabited.default)) where
  default := .init



def invTakeInput
  (state: IndexedState TProgram TData TOutput Index.takeInput)
  :=
  match state with
  | .takeInput pre prog data => (pre, prog, data)


def invProduceOutput
  (state: IndexedState TProgram TData TOutput Index.produceOutput)
  :=
  match state with
  | .produceOutput pre prog output => (pre, prog, output)


structure State (TProgram: Type u1) (TData: Type u2) (TOutput: Type u3) where
  index: Index
  indexed: IndexedState TProgram TData TOutput index

instance instInhabitedState : Inhabited (State TProgram TData TOutput) where
  default := State.mk (Inhabited.default) (Inhabited.default)

namespace Predicates


def doesn't_do_any_processing (state: State TProgram TData TOutput) : Prop :=
  if index_eq : state.index = Index.produceOutput then
    match (index_eq ▸ state.indexed) |> invProduceOutput with
    | (pre, postProg, _) =>
    match invTakeInput pre with
    | (_, preProg, _) => preProg = postProg
  else
    True


end Predicates


structure LawfulState (TProgram: Type u1) (TData: Type u2) (TOutput: Type u3) where
  state: State TProgram TData TOutput
  doesn't_do_any_processing: Predicates.doesn't_do_any_processing state

instance instInhabitedLawfulState : Inhabited (LawfulState TProgram TData TOutput) where
  default := LawfulState.mk (Inhabited.default) (by unfold Predicates.doesn't_do_any_processing; trivial)

end Interpreter


structure Interpreter (TProgram: Type u1) (TData: Type u2) (TOutput: Type u3) where
  execute:
    {pre: Interpreter.LawfulState TProgram TData TOutput // pre.state.index = .takeInput} →
    {post: Interpreter.LawfulState TProgram TData TOutput // post.state.index = .produceOutput}
  state: Interpreter.LawfulState TProgram TData TOutput

@[reducible]
def index (x: Interpreter TProgram TData TOutput) : Interpreter.Index :=
  x.state.state.index


@[reducible]
def InterpreterM (TProgram: Type u1) (TData: Type u2) (TOutput: Type u3) :=
  StateM (Interpreter TProgram TData TOutput)


namespace InterpreterM

open Std.Do
open Interpreter

def init : (InterpreterM TProgram TData TOutput) PUnit := modify (fun s => { s with state := default })


@[spec]
theorem init_spec :
  ⦃fun (_: Interpreter TProgram TData TOutput) => ⌜True⌝⦄
  init
  ⦃⇓ _ s => ⌜index s = Index.init⌝⦄
  := by
  mvcgen [init] with grind

#print init_spec

/-
def invoke (prog: TProgram) (data: TData) : (InterpreterM TProgram TData TOutput) (ULift TOutput) := do
  ← init ()
  ← modify (fun self => Interpreter.IndexedState.takeInput self prog data |> )
-/



end InterpreterM



end interpreter_s




/-

inductive Interpreter (TProgram: Type u1) (TData: Type u2) (TOutput: Type u3) : Interpreter.Index → Type (max u1 u2) where
  | takeInput (prog: TProgram) (data: TData) : Interpreter TProgram TData TOutput Interpreter.Index.takeInput
  | produceOutput
    (preIndex: {x: Interpreter.Index // x = .takeInput})
    (pre: Interpreter TProgram TData TOutput preIndex.val)
    (output: TOutput)
-/


/-
@[expose]
abbrev anyProcessingInvoked (pre: TProgram) (post: TProgram) := pre ≠ post

structure Interpreter (TData: Type u2) (TOutput: Type u3) where
  beforeExecute : (pre: TProgram) → { post: TProgram // Not (anyProcessingInvoked pre post) }
  execute : (prog: TProgram) → (data: TData) → { r: TProgram × TOutput // Not (anyProcessingInvoked r.fst prog) }
-/


/-
structure Processed (p: Prop) [Decidable p] : Prop where
  intro :: (processed: p)

instance instDecidableProcessed {p: Prop} [dp: Decidable p] : Decidable (Processed p) :=
  match
    (motive := (Decidable p) → Decidable (Processed p))
    dp
  with
  | .isTrue (h: p) => .isTrue (.intro h)
  | .isFalse (h: ¬p) =>
    have notProcessed (h2: Processed p) : False :=
      match h2 with
      | .intro h2_1 => absurd h2_1 h
    .isFalse notProcessed
-/



end Nemonuri.Study.StanfordOnline.Compilers.Introduction

end expose_s
end public_s
