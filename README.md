# LuhnAlgorithmBenchmarking

Comparing Luhn algorithm implementation of popular C# libraries and own methods (includes naive approach, platform-specific and platform-agnostic vectorization approaches).

Disassembly for naive and vectorized approaches is available in the `disassembly` directory. Currently Windows x86_64 and Linux ARM64 disassembly listings are provided.

## x86_64 results

```
BenchmarkDotNet v0.13.11, Windows 10 (10.0.19045.3803/22H2/2022Update)
AMD Ryzen 5 3600, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0
```

| Method                               | Mean         | Error     | StdDev    | Ratio | RatioSD | Code Size |
|------------------------------------- |-------------:|----------:|----------:|------:|--------:|----------:|
| GenerateLuhnDigitCreditCardValidator | 1,038.352 ns | 3.0367 ns | 2.8406 ns | 62.11 |    0.29 |   1,074 B |
| GenerateLuhnDigitLuhnNet             |   197.936 ns | 2.1991 ns | 2.0570 ns | 11.84 |    0.15 |     319 B |
| GenerateLuhnDigitSlxLuhnLibrary      |   151.931 ns | 0.3132 ns | 0.2777 ns |  9.09 |    0.06 |     326 B |
| GenerateLuhnDigit                    |    16.717 ns | 0.1046 ns | 0.0978 ns |  1.00 |    0.00 |     150 B |
| GenerateLuhnDigitSimdSse2            |     4.782 ns | 0.0120 ns | 0.0101 ns |  0.29 |    0.00 |     316 B |
| GenerateLuhnDigitSimdSse41           |     3.730 ns | 0.0108 ns | 0.0096 ns |  0.22 |    0.00 |     251 B |
| GenerateLuhnDigitSimdVector128       |    12.885 ns | 0.0300 ns | 0.0281 ns |  0.77 |    0.00 |     221 B |

## ARM64 results - macOS

```
BenchmarkDotNet v0.13.11, macOS Sonoma 14.1.1 (23B81) [Darwin 23.1.0]
Apple M1 Pro, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD

Job=.NET 8.0  Runtime=.NET 8.0
```

| Method                               | Mean       | Error     | StdDev    | Ratio | RatioSD |
|------------------------------------- |-----------:|----------:|----------:|------:|--------:|
| GenerateLuhnDigitCreditCardValidator | 690.063 ns | 1.4416 ns | 1.2779 ns | 46.66 |    0.09 |
| GenerateLuhnDigitLuhnNet             | 133.107 ns | 0.3315 ns | 0.2939 ns |  9.00 |    0.03 |
| GenerateLuhnDigitSlxLuhnLibrary      |  75.508 ns | 0.0411 ns | 0.0344 ns |  5.11 |    0.01 |
| GenerateLuhnDigit                    |  14.789 ns | 0.0244 ns | 0.0204 ns |  1.00 |    0.00 |
| GenerateLuhnDigitSimdArm64           |   1.512 ns | 0.0025 ns | 0.0024 ns |  0.10 |    0.00 |
| GenerateLuhnDigitSimdVector128       |   1.142 ns | 0.0034 ns | 0.0030 ns |  0.08 |    0.00 |

## ARM64 results - Linux in Docker container (OrbStack runtime) under macOS

```
BenchmarkDotNet v0.13.11, Debian GNU/Linux 12 (bookworm) (container)
Unknown processor
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD

Job=.NET 8.0  Runtime=.NET 8.0
```

| Method                               | Mean       | Error     | StdDev    | Ratio | RatioSD | Code Size |
|------------------------------------- |-----------:|----------:|----------:|------:|--------:|----------:|
| GenerateLuhnDigitCreditCardValidator | 697.941 ns | 2.5455 ns | 2.3810 ns | 46.94 |    0.13 |   1,420 B |
| GenerateLuhnDigitLuhnNet             | 115.922 ns | 0.0747 ns | 0.0663 ns |  7.80 |    0.01 |     468 B |
| GenerateLuhnDigitSlxLuhnLibrary      |  77.040 ns | 0.6014 ns | 0.5332 ns |  5.18 |    0.04 |     556 B |
| GenerateLuhnDigit                    |  14.862 ns | 0.0083 ns | 0.0073 ns |  1.00 |    0.00 |     200 B |
| GenerateLuhnDigitSimdArm64           |   1.506 ns | 0.0013 ns | 0.0011 ns |  0.10 |    0.00 |     252 B |
| GenerateLuhnDigitSimdVector128       |   1.108 ns | 0.0010 ns | 0.0009 ns |  0.07 |    0.00 |     200 B |
