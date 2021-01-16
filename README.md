# LuhnAlgorithmBenchmarking

Comparing Luhn algorithm implementation of popular C# libraries and own methods (includes naive approach, SSE2 vectorization and SSE4.1 vectorization methods).

Environment:

``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.1316 (1909/November2018Update/19H2)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.102
  [Host]        : .NET Core 5.0.2 (CoreCLR 5.0.220.61120, CoreFX 5.0.220.61120), X64 RyuJIT
  .NET Core 5.0 : .NET Core 5.0.2 (CoreCLR 5.0.220.61120, CoreFX 5.0.220.61120), X64 RyuJIT

Job=.NET Core 5.0  Runtime=.NET Core 5.0  

```

Results:

|                              Method |        Mean |     Error |    StdDev | Ratio | Code Size |
|------------------------------------ |------------:|----------:|----------:|------:|----------:|
| GenerateLuhnCodeCreditCardValidator | 1,752.98 ns | 23.908 ns | 22.364 ns | 1.000 |     870 B |
|             GenerateLuhnCodeLuhnNet |   385.25 ns |  3.249 ns |  2.881 ns | 0.220 |     956 B |
|      GenerateLuhnCodeSlxLuhnLibrary |   276.16 ns |  1.861 ns |  1.554 ns | 0.158 |     446 B |
|                    GenerateLuhnCode |    27.58 ns |  0.185 ns |  0.155 ns | 0.016 |     428 B |
|            GenerateLuhnCodeSimdSse2 |    22.76 ns |  0.194 ns |  0.162 ns | 0.013 |     690 B |
|           GenerateLuhnCodeSimdSse41 |    17.01 ns |  0.094 ns |  0.073 ns | 0.010 |     624 B |
