# 01-01: Introduction

## 컴파일러와 인터프리터 <span id="compiler-and-interpreter"></span>

### 1. 인터프리터 <span id="interpreter"></span>

![interpreter](image/interpreter.drawio.svg)

- 인터프리터는 프로그램과 데이터를 입력받아, 결과를 출력한다.
- 프로그램은 인터프리터와 항상 함께 실행된다. (online)

### 2. 컴파일러 <span id="compiler"></span>

![compiler](image/compiler.drawio.svg)

- 컴파일러는 일단 프로그램만 입력받아, 실행 가능한(executable) 파일을 생성한다.
  - executable 역시 프로그램이다.
- executable은 컴파일러와 별도로 실행된다. (offline)

## 컴파일러의 역사 

### 1. 1954년, IBM이 704 를 출시 <span id="history-of-compiler-ibm704"></span>

- 이 당시 컴퓨터(하드웨어) 제작 비용은 엄청 비쌌다.
- 문제는, 소프트웨어 제작 비용이 **훨씬** 더 비쌌다!
- 산업계의 고민: 
  - 어떻게 하면 소프트웨어를 더 잘, 생산성있게 만들 수 있을까?

### 2. 1953년, John Backs, "Speedcoding" 도입 <span id="history-of-compiler-speedcoding"></span>

- "Speedcoding" 이란, 초기 형태의 인터프리터
  - 장점: 프로그램 개발이 빠르다
    - 프로그래머의 생산성 증가
  - 단점: speed code 프로그램은, 직접 짠(handwritten) 다른 프로그램보다 10~20배 정도 느렸다.
    - 그리고, 오늘날의 인터프리터 프로그램도 마찬가지.

- Speed code 인터프리터는 300 바이트의 메모리 공간을 차지한다.
  - 오늘날이야 300 바이트는 정말 사소한 용량이다.
  - 하지만, 당시 300 바이트는, 704 컴퓨터 메모리의 30% 나 차지하는 용량이다!
    - 그래서, Speed code 인터프리터가 차지하는 용량 역시 골칫덩어리였다.

- 위와 같은 이유로, 오늘날 `SpeedCoding` 은 잘 알려져 있지 않다.
  - 그래도, 프로그래밍 언어 중 최초의 High-level programming language 로 인정받는다.
  - 사담: SpeedCoding 예시를 찾아봤는데, '펀치 카드'로 작성하는 프로그래밍 언어라, '텍스트로 된 코드'가 전혀 남아있지 않다...!

### 3. FORTRAN I

- 당시 컴퓨터의 주된 용도는 과학적 계산
  - 그래서 당시 프로그래머들은, 식을 어떻게 기계어로 옮길지 주로 생각했다.

- John Back 의 새로운 아이디어
  - `SpeedCoding` 언어는 식과 유사하지만, 인터프리터 방식이라 너무 느린 게 문제다.
  - 그럼, 식(Formular)을 맨 처음 실행하기 전에 기계어로 번역(Translate)하면 어떨까?
    - 프로그래머들은 프로그램을 수식(High-level)처럼 작성하고, 컴퓨터는 기계어로 번역된 프로그램을 빠르게 실행할 수 있다!

- Formular Translation Project
  - 약칭 **FORTRAN**
  - 작업 기간은 3년(1954~1957)
    - 예상 작업 기간은 1년이었다고 한다.
    - 소프트웨어 프로젝트 기간 산정 못하는 건 그때나 지금이나 같다 ㅎㅎ
  - 결과는 대성공
    - 1958년, 작성된 코드의 50%가 FORTRAN이었다.

### 4. FORTRAN I 의 의의

- 당시 사람들은 신기술에 열광했다 (요즘 사람들처럼!)
  - 추상화 수준(Level of abstraction) 증가
  - 프로그래머 생산성 증가
  - 모두가 컴퓨터를 더 유용하게 쓸 수 있게 됨

- **최초의 컴파일러**
  - 컴퓨터 과학에 막대한 영향을 남김
  - 막대한 이론적 연구(theoretical work)를 이끌어냄
    - 컴퓨터 과학의 매력은, 이론(Theory)이 실제(Practice)로 이루어진다는 것!
  - 현대 컴파일러들도 FORTRAN I에 기반(outline)하고 있다.

### 5. FORTRAN I 컴파일러의 구조

- FORTRAN I 컴파일러 5단계 구조
  1. 낱말 분석(Lexical analysis)
  2. 구문 분석(Parsing)
      - 1,2 를 합쳐 언어의 통사론적 성질(Syntactic aspect) 이라고 한다.
  3. 의미 분석(Semantic analysis)
      - 타입(Type), 범위 규칙(Scope rule)
  4. 최적화(Optimization)
      - 프로그램 변형(transformation) 규칙들의 모음
      - 더 빠른 실행 또는 더 적은 메모리 사용을 위해
  5. 코드 생성(Code generation)
      - 번역된 코드를 생성하는 것
      - 어떤 언어로 번역하는가는, 목적에 따라 다르다.
        - 기계어일 수도, 가상머신의 바이트코드일 수도, 또 다른 고수준 프로그래밍 언어일 수도 있다.



