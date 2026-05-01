module

public section

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
  deriving DecidableEq, Hashable, Repr, BEq, ReflBEq, LawfulBEq

/-!

#### 1. Interpreter

So, what does an interpreter do?

1. You wrote a **Program code**(*code*).
2. You have a **Data**(*data*), whatever you want to run the program on.
3. An **Interpreter**(*self*) takes as input, your program(*code*) and your data(*data*).
4. It(*self*) produces the output directly.
5. Meaning that it(*self*) doesn't do any processing of the program(*code*) before it executes it on the input.
6. So you just write the program(*code*), and you invoke the interpreter(*self*) on the data(*data*), and the program(*code*) immediately begins running.

-/


structure ProgramCode (α: Type u) where
  val : α
  deriving DecidableEq, Hashable, Repr, BEq, ReflBEq, LawfulBEq

structure Data (α: Type u) where
  val : α
  deriving DecidableEq, Hashable, Repr, BEq, ReflBEq, LawfulBEq

structure Processed (p: Prop) [Decidable p] : Prop where
  intro :: (processed: p)

instance instDecidableProcessed {p: Prop} [dp: Decidable p] : Decidable (Processed p) :=
  match
    (motive := (Decidable p) → Decidable (Processed p))
    dp
  with
  | .isTrue (h: p) => .isTrue (.intro h)
  | .isFalse (h: ¬p) =>
    let notProcessed (h2: Processed p) : False :=
      match h2 with
      | .intro h2_1 => absurd h2_1 h
    .isFalse notProcessed




end Nemonuri.Study.StanfordOnline.Compilers.Introduction

instance optionToMonad : Monad Option where
  pure x := some x
  bind opt next :=
    match opt with
    | none => none
    | some x => next x

#print optionToMonad

end --public
