# LuhnAlgorithmBenchmarking

Comparing Luhn algorithm implementation of popular C# libraries and own methods (includes naive approach, platform-specific and platform-agnostic vectorization approaches).

## ARM64 results

``` ini
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

# Old results, TODO
## x86 results

``` ini

BenchmarkDotNet=v0.13.4, OS=macOS 13.2 (22D49) [Darwin 22.3.0]
Intel Core i9-9880H CPU 2.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.102
  [Host]   : .NET 7.0.2 (7.0.222.60605), X64 RyuJIT AVX2
  .NET 7.0 : .NET 7.0.2 (7.0.222.60605), X64 RyuJIT AVX2

Job=.NET 7.0  Runtime=.NET 7.0  

```
|                               Method |         Mean |      Error |     StdDev |  Ratio | RatioSD |
|------------------------------------- |-------------:|-----------:|-----------:|-------:|--------:|
| GenerateLuhnDigitCreditCardValidator | 1,438.000 ns | 27.8563 ns | 39.9507 ns | 100.85 |    4.35 |
|             GenerateLuhnDigitLuhnNet |   275.385 ns |  5.0087 ns |  6.8560 ns |  19.40 |    0.68 |
|      GenerateLuhnDigitSlxLuhnLibrary |   185.823 ns |  3.4347 ns |  3.2128 ns |  13.00 |    0.49 |
|                    GenerateLuhnDigit |    14.222 ns |  0.3056 ns |  0.3973 ns |   1.00 |    0.00 |
|            GenerateLuhnDigitSimdSse2 |     6.891 ns |  0.1664 ns |  0.2044 ns |   0.48 |    0.02 |
|           GenerateLuhnDigitSimdSse41 |     4.527 ns |  0.0732 ns |  0.0649 ns |   0.32 |    0.01 |
|       GenerateLuhnDigitSimdVector128 |    13.770 ns |  0.2011 ns |  0.1679 ns |   0.96 |    0.02 |
