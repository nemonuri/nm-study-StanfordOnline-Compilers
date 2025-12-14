# 01-01: Introduction

## 컴파일러와 인터프리터 <span id="compiler-and-interpreter"></span>

1. 인터프리터 <span id="interpreter"></span>

![interpreter](image/interpreter.drawio.svg)

- 인터프리터는 프로그램과 데이터를 입력받아, 결과를 출력한다.
- 프로그램은 인터프리터와 항상 함께 실행된다. (online)

2. 컴파일러 <span id="compiler"></span>

![compiler](image/compiler.drawio.svg)

- 컴파일러는 일단 프로그램만 입력받아, 실행 가능한(executable) 파일을 생성한다.
  - executable 역시 프로그램이다.
- executable은 컴파일러와 별도로 실행된다. (offline)

## 컴파일러의 역사 <span id="history-of-compiler"></span>

1. 1954년, IBM이 704 를 출시

- 이 당시 컴퓨터(하드웨어) 제작 비용은 엄청 비쌌다.
- 문제는, 소프트웨어 제작 비용이 **훨씬** 더 비쌌다!
- 산업계의 고민: 
  - 어떻게 하면 소프트웨어를 더 잘, 생산성있게 만들 수 있을까?

