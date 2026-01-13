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

## 컴파일러의 역사 

1. 1954년, IBM이 704 를 출시 <span id="history-of-compiler-ibm704"></span>

- 이 당시 컴퓨터(하드웨어) 제작 비용은 엄청 비쌌다.
- 문제는, 소프트웨어 제작 비용이 **훨씬** 더 비쌌다!
- 산업계의 고민: 
  - 어떻게 하면 소프트웨어를 더 잘, 생산성있게 만들 수 있을까?

2. 1953년, John Backs, "Speedcoding" 도입 <span id="history-of-compiler-speedcoding"></span>

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