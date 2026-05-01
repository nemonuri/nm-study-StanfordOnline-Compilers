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
5. Meaning that it(*self*) doesn't do any **Processing**(*pc*) of the program(*code*) before it executes it on the input.
6. So you just write the program(*code*), and you invoke the interpreter(*self*) on the data(*data*), and the program(*code*) immediately begins running.

-/

section _1_1_Interpreter

universe u
variable (α: Type u) --[Hashable α] [Repr α] [BEq α] [LawfulBEq α]


structure ProgramCode where
  val : α
  deriving DecidableEq, Hashable, Repr, BEq, ReflBEq, LawfulBEq







end _1_1_Interpreter


end Nemonuri.Study.StanfordOnline.Compilers.Introduction

end --public
