using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;
using CreditCardValidator;
using SlxLuhnLibrary;

namespace LuhnAlgorithmBenchmarking;

[DisassemblyDiagnoser]
public class LuhnCodeBenchmark
{
	private const byte zeroChar = (byte) '0';
	private readonly byte[] _calculatedDigitForSum = new byte[140];
	private readonly byte[] _cardCode = new byte[16];
	private string _cardCodeStr;

	private static ReadOnlySpan<byte> Mask => [255, 0, 255, 0, 255, 0, 255, 0, 255, 0, 255, 0, 255, 0, 255, 0];

	[Benchmark(Baseline = true)]
	public byte GenDigit()
	{
		var cardCode = _cardCode;
		byte sum = 0;
		for (var i = 0; i < cardCode.Length; i++)
		{
			var digit = (byte) (cardCode[i] - zeroChar);
			if (i % 2 == 0)
			{
				digit *= 2;

				if (digit > 9)
				{
					digit -= 9;
				}
			}

			sum += digit;
		}

		return GetDigitForSum(sum);
	}

	[Benchmark]
	public byte GenDigitThirdPartyCreditCardValidator() =>
		(byte) (Luhn.CreateCheckDigit(_cardCodeStr)[0] - zeroChar);

	[Benchmark]
	public byte GenDigitThirdPartyLuhnNet() => LuhnNet.Luhn.CalculateCheckDigit(_cardCodeStr);

	[Benchmark]
	public byte GenDigitThirdPartySlxLuhnLibrary() => (byte) (ClsLuhnLibrary.GenerateCheckCharacter(
		_cardCodeStr, ClsLuhnLibrary.CharacterSet.Base10)!.Value - zeroChar);

	[Benchmark]
	public byte GenDigitUnrolledFourGroups()
	{
		var cardCode = _cardCode;
		var sum1 = 0;
		var sum2 = 0;
		var sum3 = 0;
		var sum4 = 0;

		var digit4 = cardCode[15] - zeroChar;
		if (digit4 > 9) digit4 -= 9;
		sum4 += digit4;

		var digit3 = (cardCode[14] - zeroChar) << 1;
		if (digit3 > 9) digit3 -= 9;
		sum3 += digit3;

		var digit2 = cardCode[13] - zeroChar;
		if (digit2 > 9) digit2 -= 9;
		sum2 += digit2;

		var digit1 = (cardCode[12] - zeroChar) << 1;
		if (digit1 > 9) digit1 -= 9;
		sum1 += digit1;

		digit4 = cardCode[11] - zeroChar;
		if (digit4 > 9) digit4 -= 9;
		sum4 += digit4;

		digit3 = (cardCode[10] - zeroChar) << 1;
		if (digit3 > 9) digit3 -= 9;
		sum3 += digit3;

		digit2 = cardCode[9] - zeroChar;
		if (digit2 > 9) digit2 -= 9;
		sum2 += digit2;

		digit1 = (cardCode[8] - zeroChar) << 1;
		if (digit1 > 9) digit1 -= 9;
		sum1 += digit1;

		digit4 = cardCode[7] - zeroChar;
		if (digit4 > 9) digit4 -= 9;
		sum4 += digit4;

		digit3 = (cardCode[6] - zeroChar) << 1;
		if (digit3 > 9) digit3 -= 9;
		sum3 += digit3;

		digit2 = cardCode[5] - zeroChar;
		if (digit2 > 9) digit2 -= 9;
		sum2 += digit2;

		digit1 = (cardCode[4] - zeroChar) << 1;
		if (digit1 > 9) digit1 -= 9;
		sum1 += digit1;

		digit4 = cardCode[3] - zeroChar;
		if (digit4 > 9) digit4 -= 9;
		sum4 += digit4;

		digit3 = (cardCode[2] - zeroChar) << 1;
		if (digit3 > 9) digit3 -= 9;
		sum3 += digit3;

		digit2 = cardCode[1] - zeroChar;
		if (digit2 > 9) digit2 -= 9;
		sum2 += digit2;

		digit1 = (cardCode[0] - zeroChar) << 1;
		if (digit1 > 9) digit1 -= 9;
		sum1 += digit1;

		return GetDigitForSum(sum1 + sum2 + sum3 + sum4);
	}

	[Benchmark]
	public byte GenDigitUnrolledTwoGroups()
	{
		var cardCode = _cardCode;
		var sum1 = 0;
		var sum2 = 0;

		var digit1 = cardCode[15] - zeroChar;
		if (digit1 > 9) digit1 -= 9;
		sum1 += digit1;

		var digit2 = (cardCode[14] - zeroChar) << 1;
		if (digit2 > 9) digit2 -= 9;
		sum2 += digit2;

		digit1 = cardCode[13] - zeroChar;
		if (digit1 > 9) digit1 -= 9;
		sum1 += digit1;

		digit2 = (cardCode[12] - zeroChar) << 1;
		if (digit2 > 9) digit2 -= 9;
		sum2 += digit2;

		digit1 = cardCode[11] - zeroChar;
		if (digit1 > 9) digit1 -= 9;
		sum1 += digit1;

		digit2 = (cardCode[10] - zeroChar) << 1;
		if (digit2 > 9) digit2 -= 9;
		sum2 += digit2;

		digit1 = cardCode[9] - zeroChar;
		if (digit1 > 9) digit1 -= 9;
		sum1 += digit1;

		digit2 = (cardCode[8] - zeroChar) << 1;
		if (digit2 > 9) digit2 -= 9;
		sum2 += digit2;

		digit1 = cardCode[7] - zeroChar;
		if (digit1 > 9) digit1 -= 9;
		sum1 += digit1;

		digit2 = (cardCode[6] - zeroChar) << 1;
		if (digit2 > 9) digit2 -= 9;
		sum2 += digit2;

		digit1 = cardCode[5] - zeroChar;
		if (digit1 > 9) digit1 -= 9;
		sum1 += digit1;

		digit2 = (cardCode[4] - zeroChar) << 1;
		if (digit2 > 9) digit2 -= 9;
		sum2 += digit2;

		digit1 = cardCode[3] - zeroChar;
		if (digit1 > 9) digit1 -= 9;
		sum1 += digit1;

		digit2 = (cardCode[2] - zeroChar) << 1;
		if (digit2 > 9) digit2 -= 9;
		sum2 += digit2;

		digit1 = cardCode[1] - zeroChar;
		if (digit1 > 9) digit1 -= 9;
		sum1 += digit1;

		digit2 = (cardCode[0] - zeroChar) << 1;
		if (digit2 > 9) digit2 -= 9;
		sum2 += digit2;

		return GetDigitForSum(sum1 + sum2);
	}

	[Benchmark]
	public byte GenDigitVectorizedAdvSimd()
	{
		if (!AdvSimd.IsSupported)
			throw new NotSupportedException();

		var inputVector = Vector128.Create(_cardCode);

		var subtractResult = AdvSimd.Subtract(inputVector, Vector128.Create(zeroChar));

		var multiplyResult = AdvSimd.Add(
			subtractResult,
			AdvSimd.BitwiseSelect(
				Vector128.Create(Mask),
				subtractResult,
				Vector128<byte>.Zero));

		var nineVector = Vector128.Create<byte>(9);

		var vsum = AdvSimd.Arm64.AddAcross(
			AdvSimd.Subtract(
				multiplyResult,
				AdvSimd.BitwiseSelect(
					AdvSimd.CompareGreaterThan(
						multiplyResult,
						nineVector),
					nineVector,
					Vector128<byte>.Zero)));

		var sum = AdvSimd.Extract(vsum, 0);

		return GetDigitForSum(sum);
	}

	[Benchmark]
	public byte GenDigitVectorizedSse2()
	{
		if (!Sse2.IsSupported)
			throw new NotSupportedException();

		var inputVector = Vector128.Create(_cardCode);

		var subtractResult = Sse2.Subtract(inputVector, Vector128.Create(zeroChar));

		var multiplyResult = Sse2.Add(
			subtractResult,
			Sse2.And(Vector128.Create(Mask), subtractResult));

		var nineVector = Vector128.Create<byte>(9);

		var results = Sse2.Subtract(
			multiplyResult,
			Sse2.And(
				Sse2.CompareGreaterThan(
					multiplyResult.AsSByte(),
					nineVector.AsSByte()).AsByte(),
				nineVector));

		var sum = SumByteVector(results);

		return GetDigitForSum(sum);
	}

	[Benchmark]
	public byte GenDigitVectorizedSse41()
	{
		if (!Sse41.IsSupported)
			throw new NotSupportedException();

		var inputVector = Vector128.Create(_cardCode);

		var subtractResult = Sse2.Subtract(inputVector, Vector128.Create(zeroChar));

		var multiplyResult = Sse2.Add(
			subtractResult, Sse41.BlendVariable(Vector128<byte>.Zero, subtractResult, Vector128.Create(Mask)));

		var nineVector = Vector128.Create<byte>(9);

		var results = Sse2.Subtract(
			multiplyResult,
			Sse41.BlendVariable(
				Vector128<byte>.Zero,
				nineVector,
				Sse2.CompareGreaterThan(
					multiplyResult.AsSByte(),
					nineVector.AsSByte()).AsByte()));

		var sum = SumByteVector(results);

		return GetDigitForSum(sum);
	}

	[Benchmark]
	public byte GenDigitVectorizedVector128()
	{
		if (!Vector128.IsHardwareAccelerated)
			throw new NotSupportedException();

		var inputVector = Vector128.Create(_cardCode);

		var digitVector = Vector128.Subtract(inputVector, Vector128.Create(zeroChar));

		var multipliedVector = Vector128.Add(
			digitVector,
			Vector128.ConditionalSelect(
				Vector128.Create(Mask),
				digitVector,
				Vector128<byte>.Zero));

		var nineVector = Vector128.Create<byte>(9);

		var sum = Vector128.Sum(
			Vector128.Subtract(
				multipliedVector,
				Vector128.ConditionalSelect(
					Vector128.GreaterThan(multipliedVector, nineVector),
					nineVector,
					Vector128<byte>.Zero)));

		return GetDigitForSum(sum);
	}

	[Benchmark]
	public byte GenDigitVectorizedVector128WithSse41()
	{
		if (!Sse41.IsSupported)
			throw new NotSupportedException();

		var inputVector = Vector128.Create(_cardCode);

		var subtractResult = Vector128.Subtract(inputVector, Vector128.Create(zeroChar));

		var multiplyResult = Vector128.Add(
			subtractResult,
			// TODO: replace with Vector128.ConditionalSelect when my optimization lands in .NET
			Sse41.BlendVariable(
				Vector128<byte>.Zero,
				subtractResult,
				Vector128.Create(Mask)));

		var nineVector = Vector128.Create<byte>(9);

		var results = Vector128.Subtract(
			multiplyResult,
			Vector128.ConditionalSelect(
				GreaterThanUnsigned(multiplyResult, nineVector),
				nineVector,
				Vector128<byte>.Zero));

		// TODO: Vector128.Sum got significantly optimized in .NET 9 but the referenced implementation is still faster.
		// Probably possible to optimize?
		var sum = SumByteVector(results);

		return GetDigitForSum(sum);
	}

	[GlobalSetup]
	public void Setup()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
		}

		var random = new Random(123);
		var cardCode = random.NextInt64(1000000000000000);
		_cardCodeStr = cardCode.ToString("D15", CultureInfo.InvariantCulture);
		for (var i = 0; i < _cardCode.Length - 1; i++)
		{
			_cardCode[i] = (byte) _cardCodeStr[i];
		}

		_cardCode[^1] = zeroChar;

		var calculatedDigitForSumRem = Enumerable.Range(0, 10)
			.Select(static digit => (byte) ((10 - digit % 10) % 10))
			.ToArray();

		// Max 15 digits, 9 * 15 = 135, rounding up to 140
		for (var i = 0; i < Math.Ceiling(9 * 15 / 10.0); i++)
		{
			calculatedDigitForSumRem.CopyTo(_calculatedDigitForSum, i * 10);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private byte GetDigitForSum(int sum)
	{
		// avoiding _calculatedDigitForSum[sum] bound check
		return Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_calculatedDigitForSum), sum);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector128<byte> GreaterThanUnsigned(Vector128<byte> left, Vector128<byte> right)
	{
		// bypass signed comparison, we don't care about it
		// https://github.com/dotnet/runtime/blob/53f461449ff996884087910b97ec4509a558b070/src/coreclr/jit/gentree.cpp#L22629-L22641
		return Vector128.GreaterThan(left.AsSByte(), right.AsSByte()).AsByte();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte SumByteVector(Vector128<byte> input)
	{
		// ref: https://stackoverflow.com/a/36998778
		var vsum = Sse2.SumAbsoluteDifferences(input, Vector128<byte>.Zero);

		return (byte) (Sse2.Extract(vsum, 0) + Sse2.Extract(vsum, 4));
	}
}
