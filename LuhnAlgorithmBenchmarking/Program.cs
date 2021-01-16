using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using CreditCardValidator;
using SlxLuhnLibrary;

namespace LuhnAlgorithmBenchmarking {
	public static class Program {
		public static void Main() {
			BenchmarkRunner.Run<LuhnCodeTests>();
		}
	}

	[DisassemblyDiagnoser]
	[SimpleJob(RuntimeMoniker.NetCoreApp50)]
	public class LuhnCodeTests {
		private byte[] CardCode;

		[Benchmark(Baseline = true)]
		public byte[] GenerateLuhnCodeCreditCardValidator() {
			string text = Encoding.ASCII.GetString(CardCode);

			return Encoding.ASCII.GetBytes(text + Luhn.CreateCheckDigit(text));
		}

		[Benchmark]
		public byte[] GenerateLuhnCodeLuhnNet() {
			string text = Encoding.ASCII.GetString(CardCode);

			return Encoding.ASCII.GetBytes(text + LuhnNet.Luhn.CalculateCheckDigit(text));
		}

		[Benchmark]
		public byte[] GenerateLuhnCodeSlxLuhnLibrary() => Encoding.ASCII.GetBytes(ClsLuhnLibrary.WithLuhn_Base10(Encoding.ASCII.GetString(CardCode)));

		[Benchmark]
		public byte[] GenerateLuhnCode() {
			byte sum = 0;
			for (int i = 0; i < CardCode.Length; i++) {
				byte digit = (byte) (CardCode[i] - '0');
				if (i % 2 == 0) {
					digit *= 2;

					if (digit > 9) {
						digit -= 9;
					}
				}

				sum += digit;
			}

			byte[] arr = new byte[16];
			Array.Copy(CardCode, arr, 15);
			arr[15] = (byte) (sum % 10 == 0 ? '0' : 10 - sum % 10 + '0');

			return arr;
		}

		[Benchmark]
		public byte[] GenerateLuhnCodeSimdSse2() {
			if (!Sse2.IsSupported) {
				throw new NotSupportedException();
			}

			byte[] arr = new byte[16];

			Vector128<byte> inputVector;
			unsafe {
				fixed (byte* arrRef = arr) {
					fixed (byte* cardCodeRef = CardCode) {
						Buffer.MemoryCopy(cardCodeRef, arrRef, 16, 15);
					}

					inputVector = Sse2.LoadVector128(arrRef);
				}
			}

			arr[15] = (byte) '0';
			Vector128<byte> substractResult = Sse2.Subtract(
				inputVector,
				Vector128.Create((byte) '0')
			);

			Vector128<byte> zeroVector = Vector128<byte>.Zero;
			Vector128<byte> multiplyResult = Sse2.Add(
				substractResult,
				MultiplyBytes(
					Sse2.Subtract(
						zeroVector.AsSByte(),
						Vector128.Create(255, 0, 255, 0, 255, 0, 255, 0, 255, 0, 255, 0, 255, 0, 255, 0).AsSByte()
					).AsByte(),
					substractResult
				)
			);

			Vector128<byte> nineVector = Vector128.Create((byte) 9);

			Vector128<ushort> vsum = Sse2.SumAbsoluteDifferences(
				Sse2.Subtract(
					multiplyResult,
					MultiplyBytes(
						Sse2.Subtract(
							zeroVector.AsSByte(),
							Sse2.CompareGreaterThan(
								multiplyResult.AsSByte(),
								nineVector.AsSByte()
							)
						).AsByte(),
						nineVector
					)
				),
				zeroVector
			);

			// ref: https://stackoverflow.com/a/36998778
			byte sum = (byte) (Sse2.Extract(vsum, 0) + Sse2.Extract(vsum, 4));
			arr[15] = (byte) (sum % 10 == 0 ? '0' : 10 - sum % 10 + '0');

			return arr;
		}

		[Benchmark]
		public byte[] GenerateLuhnCodeSimdSse41() {
			if (!Sse41.IsSupported) {
				throw new NotSupportedException();
			}

			byte[] arr = new byte[16];

			Vector128<byte> inputVector;
			unsafe {
				fixed (byte* arrRef = arr) {
					fixed (byte* cardCodeRef = CardCode) {
						Buffer.MemoryCopy(cardCodeRef, arrRef, 16, 15);
					}

					inputVector = Sse2.LoadVector128(arrRef);
				}
			}

			arr[15] = (byte) '0';

			Vector128<byte> substractResult = Sse2.Subtract(
				inputVector,
				Vector128.Create((byte) '0')
			);

			Vector128<byte> zeroVector = Vector128<byte>.Zero;
			Vector128<byte> multiplyResult = Sse2.Add(
				substractResult,
				Sse41.BlendVariable(
					zeroVector,
					substractResult,
					Vector128.Create(255, 0, 255, 0, 255, 0, 255, 0, 255, 0, 255, 0, 255, 0, 255, 0)
				)
			);

			Vector128<byte> nineVector = Vector128.Create((byte) 9);

			Vector128<ushort> vsum = Sse2.SumAbsoluteDifferences(
				Sse2.Subtract(
					multiplyResult,
					Sse41.BlendVariable(
						zeroVector,
						nineVector,
						Sse2.CompareGreaterThan(
							multiplyResult.AsSByte(),
							nineVector.AsSByte()
						).AsByte()
					)
				),
				zeroVector
			);

			// ref: https://stackoverflow.com/a/36998778
			byte sum = (byte) (Sse2.Extract(vsum, 0) + Sse2.Extract(vsum, 4));
			arr[15] = (byte) (sum % 10 == 0 ? '0' : 10 - sum % 10 + '0');

			return arr;
		}

		// ref: https://stackoverflow.com/a/29155682
		// AVX optimization is not used here, since we fallback to this method only if anything higher then SSE2 is not supported
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector128<byte> MultiplyBytes(Vector128<byte> left, Vector128<byte> right) => Sse2.Or(
			Sse2.ShiftLeftLogical(Sse2.MultiplyLow(Sse2.ShiftRightLogical(left.AsUInt16(), 8), Sse2.ShiftRightLogical(right.AsUInt16(), 8)), 8),
			Sse2.ShiftRightLogical(Sse2.ShiftLeftLogical(Sse2.MultiplyLow(left.AsUInt16(), right.AsUInt16()), 8), 8)
		).AsByte();

		[GlobalSetup]
		public void Setup() {
			Random random = new();
			CardCode = Encoding.ASCII.GetBytes(string.Join("", Enumerable.Range(0, 15).Select(_ => random.Next(0, 10).ToString(CultureInfo.InvariantCulture))));
		}
	}
}
