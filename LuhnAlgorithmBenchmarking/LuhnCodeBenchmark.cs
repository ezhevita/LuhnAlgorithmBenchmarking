using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace LuhnAlgorithmBenchmarking;

[DisassemblyDiagnoser(printSource: true)]
[SimpleJob(RuntimeMoniker.Net80)]
public class LuhnCodeBenchmark
{
	private const byte zeroChar = (byte) '0';
	private byte[] _cardCode;
	private string _cardCodeStr;

	[Benchmark]
	public byte GenerateLuhnDigitCreditCardValidator() => (byte) (CreditCardValidator.Luhn.CreateCheckDigit(_cardCodeStr)[0] - '0');

	[Benchmark]
	public byte GenerateLuhnDigitLuhnNet() => LuhnNet.Luhn.CalculateCheckDigit(_cardCodeStr);

	[Benchmark]
	public byte GenerateLuhnDigitSlxLuhnLibrary() => (byte) (SlxLuhnLibrary.ClsLuhnLibrary.GenerateCheckCharacter(
		_cardCodeStr, SlxLuhnLibrary.ClsLuhnLibrary.CharacterSet.Base10)!.Value - '0');

	[Benchmark(Baseline = true)]
	public byte GenerateLuhnDigit()
	{
		byte sum = 0;
		for (var i = 0; i < _cardCode.Length; i++)
		{
			var digit = (byte) (_cardCode[i] - zeroChar);
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

		return (byte) ((10 - sum % 10) % 10);
	}

	private readonly byte[] _mask = [255, 0, 255, 0, 255, 0, 255, 0, 255, 0, 255, 0, 255, 0, 255, 0];

	[Benchmark]
	public byte GenerateLuhnDigitSimdSse2()
	{
		if (!Sse2.IsSupported)
			throw new NotSupportedException();

		Vector128<byte> inputVector;
		unsafe
		{
			fixed (byte* cardCodeRef = _cardCode)
			{
				inputVector = Sse2.LoadVector128(cardCodeRef);
			}
		}

		var substractResult = Sse2.Subtract(
			inputVector,
			Vector128.Create(zeroChar)
		);

		var multiplyResult = Sse2.Add(
			substractResult,
			MultiplyBytes(
				Sse2.Subtract(
					Vector128<byte>.Zero.AsSByte(),
					Vector128.Create(_mask).AsSByte()
				).AsByte(),
				substractResult
			)
		);

		var nineVector = Vector128.Create((byte) 9);

		var vsum = Sse2.SumAbsoluteDifferences(
			Sse2.Subtract(
				multiplyResult,
				MultiplyBytes(
					Sse2.Subtract(
						Vector128<byte>.Zero.AsSByte(),
						Sse2.CompareGreaterThan(
							multiplyResult.AsSByte(),
							nineVector.AsSByte()
						)
					).AsByte(),
					nineVector
				)
			),
			Vector128<byte>.Zero
		);

		// ref: https://stackoverflow.com/a/36998778
		var sum = (byte) (Sse2.Extract(vsum, 0) + Sse2.Extract(vsum, 4));

		return (byte) ((10 - sum % 10) % 10);
	}

	[Benchmark]
	public byte GenerateLuhnDigitSimdSse41()
	{
		if (!Sse41.IsSupported)
			throw new NotSupportedException();

		Vector128<byte> inputVector;
		unsafe
		{
			fixed (byte* cardCodeRef = _cardCode)
			{
				inputVector = Sse2.LoadVector128(cardCodeRef);
			}
		}

		var substractResult = Sse2.Subtract(
			inputVector,
			Vector128.Create(zeroChar)
		);

		var multiplyResult = Sse2.Add(
			substractResult,
			Sse41.BlendVariable(
				Vector128<byte>.Zero,
				substractResult,
				Vector128.Create(_mask)
			)
		);

		var nineVector = Vector128.Create((byte) 9);

		var vsum = Sse2.SumAbsoluteDifferences(
			Sse2.Subtract(
				multiplyResult,
				Sse41.BlendVariable(
					Vector128<byte>.Zero,
					nineVector,
					Sse2.CompareGreaterThan(
						multiplyResult.AsSByte(),
						nineVector.AsSByte()
					).AsByte()
				)
			),
			Vector128<byte>.Zero
		);

		// ref: https://stackoverflow.com/a/36998778
		var sum = (byte) (Sse2.Extract(vsum, 0) + Sse2.Extract(vsum, 4));

		return (byte) ((10 - sum % 10) % 10);
	}

	[Benchmark]
	public byte GenerateLuhnDigitSimdArm64()
	{
		if (!AdvSimd.IsSupported)
			throw new NotSupportedException();

		Vector128<byte> inputVector;
		unsafe
		{
			fixed (byte* cardCodeRef = _cardCode)
			{
				inputVector = AdvSimd.LoadVector128(cardCodeRef);
			}
		}

		var substractResult = AdvSimd.Subtract(
			inputVector,
			Vector128.Create(zeroChar)
		);

		var multiplyResult = AdvSimd.Add(
			substractResult,
			AdvSimd.BitwiseSelect(
				Vector128.Create(_mask),
				substractResult,
				Vector128<byte>.Zero
			)
		);

		var nineVector = Vector128.Create((byte) 9);

		var vsum = AdvSimd.Arm64.AddAcross(
			AdvSimd.Subtract(
				multiplyResult,
				AdvSimd.BitwiseSelect(
					AdvSimd.CompareGreaterThan(
						multiplyResult,
						nineVector
					),
					nineVector,
					Vector128<byte>.Zero
				)
			)
		);

		var sum = AdvSimd.Extract(vsum, 0);

		return (byte) ((10 - sum % 10) % 10);
	}

	[Benchmark]
	public byte GenerateLuhnDigitSimdVector128()
	{
		if (!Vector128.IsHardwareAccelerated)
			throw new NotSupportedException();

		var inputVector = Vector128.Create(_cardCode);

		var digitVector = Vector128.Subtract(inputVector, Vector128.Create(zeroChar));

		var multipliedVector = Vector128.Add(
			digitVector,
			Vector128.ConditionalSelect(
				Vector128.Create(_mask),
				digitVector,
				Vector128<byte>.Zero
			)
		);

		var nineVector = Vector128.Create((byte) 9);

		var sum = Vector128.Sum(
			Vector128.Subtract(
				multipliedVector,
				Vector128.ConditionalSelect(
					Vector128.GreaterThan(
						multipliedVector,
						nineVector
					),
					nineVector,
					Vector128<byte>.Zero
				)
			)
		);

		return (byte) ((10 - sum % 10) % 10);
	}

	// ref: https://stackoverflow.com/a/29155682
	// AVX optimization is not used here, since we fallback to this method only if anything higher then SSE2 is not supported
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector128<byte> MultiplyBytes(Vector128<byte> left, Vector128<byte> right) => Sse2.Or(
		Sse2.ShiftLeftLogical(Sse2.MultiplyLow(Sse2.ShiftRightLogical(left.AsUInt16(), 8), Sse2.ShiftRightLogical(right.AsUInt16(), 8)), 8),
		Sse2.ShiftRightLogical(Sse2.ShiftLeftLogical(Sse2.MultiplyLow(left.AsUInt16(), right.AsUInt16()), 8), 8)
	).AsByte();

	[GlobalSetup]
	public void Setup()
	{
		Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
		var random = new Random(123);
#pragma warning disable CA5394
		_cardCodeStr = string.Join("", Enumerable.Range(0, 15).Select(_ => random.Next(0, 10).ToString(CultureInfo.InvariantCulture)));
#pragma warning restore CA5394
		_cardCode = Encoding.ASCII.GetBytes(_cardCodeStr + "0");
	}
}
