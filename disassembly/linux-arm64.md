## .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
```assembly
; LuhnAlgorithmBenchmarking.LuhnCodeBenchmark.GenerateLuhnDigit()
; 		byte sum = 0;
; 		^^^^^^^^^^^^^
; 		for (var i = 0; i < _cardCode.Length; i++)
; 		     ^^^^^^^^^
; 			var digit = (byte) (_cardCode[i] - zeroChar);
; 			^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 			if (i % 2 == 0)
; 			^^^^^^^^^^^^^^^
; 				digit *= 2;
; 				^^^^^^^^^^^
; 				if (digit > 9)
; 				^^^^^^^^^^^^^^
; 					digit -= 9;
; 					^^^^^^^^^^^
; 			sum += digit;
; 			^^^^^^^^^^^^^
; 		return (byte) ((10 - sum % 10) % 10);
; 		^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
       stp       x29, x30, [sp, #-0x10]!
       mov       x29, sp
       mov       w1, wzr
       mov       w2, wzr
       ldr       x0, [x0, #8]
       ldr       w3, [x0, #8]
       cmp       w3, #0
       b.le      M00_L02
M00_L00:
       mov       x3, x0
       ldr       w4, [x3, #8]
       cmp       w2, w4
       b.hs      M00_L03
       add       x3, x3, #0x10
       ldrb      w3, [x3, w2, uxtw #0]
       sub       w3, w3, #0x30
       uxtb      w3, w3
       tbnz      w2, #0, #0xffff5b0dfcfc
       lsl       w3, w3, #1
       uxtb      w3, w3
       cmp       w3, #9
       b.le      M00_L01
       sub       w3, w3, #9
       uxtb      w3, w3
M00_L01:
       add       w1, w1, w3
       uxtb      w1, w1
       add       w2, w2, #1
       ldr       w3, [x0, #8]
       cmp       w3, w2
       b.gt      M00_L00
M00_L02:
       movz      w0, #0xcccd
       movk      w0, #0xcccc, lsl #16
       umull     x0, w1, w0
       lsr       x0, x0, #0x23
       movz      w2, #0xa
       msub      w0, w0, w2, w1
       neg       w0, w0
       add       w0, w0, #0xa
       movz      w1, #0x6667
       movk      w1, #0x6666, lsl #16
       smull     x1, w1, w0
       asr       x1, x1, #0x20
       asr       w2, w1, #2
       add       w1, w2, w1, lsr #31
       movz      w2, #0xa
       msub      w0, w1, w2, w0
       uxtb      w0, w0
       ldp       x29, x30, [sp], #0x10
       ret
M00_L03:
       bl        #0xffff5b0b00f8
       brk       #0
; Total bytes of code 200
```

## .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
```assembly
; LuhnAlgorithmBenchmarking.LuhnCodeBenchmark.GenerateLuhnDigitSimdArm64()
; 			fixed (byte* cardCodeRef = _cardCode)
; 			       ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 				inputVector = AdvSimd.LoadVector128(cardCodeRef);
; 				^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 		var multiplyResult = AdvSimd.Add(
; 		^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 			substractResult,
;
; 			AdvSimd.BitwiseSelect(
;
; 				Vector128.Create(_mask),
;
; 				substractResult,
;
; 				Vector128<byte>.Zero
;
; 			)
;
; 		);
;
; 		return (byte) ((10 - sum % 10) % 10);
; 		^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
       stp       x29, x30, [sp, #-0x20]!
       mov       x29, sp
       ldr       x1, [x0, #8]
       str       x1, [x29, #0x18]
       cbz       x1, M00_L01
       ldr       x1, [x29, #0x18]
       ldr       w1, [x1, #8]
       cbz       w1, M00_L01
       ldr       x1, [x29, #0x18]
       ldr       w1, [x1, #8]
       cmp       w1, #0
       b.ls      M00_L03
       ldr       x1, [x29, #0x18]
       add       x1, x1, #0x10
M00_L00:
       ldr       q16, [x1]
       str       xzr, [x29, #0x18]
       ldr       q17, #0xffff6a0bfea0
       sub       v16.16b, v16.16b, v17.16b
       ldr       x0, [x0, #0x18]
       ldr       w1, [x0, #8]
       cmp       w1, #0x10
       b.lt      M00_L02
       ldr       q17, [x0, #0x10]
       movi      v18.4s, #0
       bsl       v17.16b, v16.16b, v18.16b
       add       v16.16b, v16.16b, v17.16b
       ldr       q17, #0xffff6a0bfeb0
       cmhi      v17.16b, v16.16b, v17.16b
       ldr       q18, #0xffff6a0bfeb0
       movi      v19.4s, #0
       bsl       v17.16b, v18.16b, v19.16b
       sub       v16.16b, v16.16b, v17.16b
       addv      b16, v16.16b
       umov      w0, v16.b[0]
       movz      w1, #0xcccd
       movk      w1, #0xcccc, lsl #16
       umull     x1, w0, w1
       lsr       x1, x1, #0x23
       movz      w2, #0xa
       msub      w0, w1, w2, w0
       neg       w0, w0
       add       w0, w0, #0xa
       movz      w1, #0x6667
       movk      w1, #0x6666, lsl #16
       smull     x1, w1, w0
       asr       x1, x1, #0x20
       asr       w2, w1, #2
       add       w1, w2, w1, lsr #31
       movz      w2, #0xa
       msub      w0, w1, w2, w0
       uxtb      w0, w0
       ldp       x29, x30, [sp], #0x20
       ret
M00_L01:
       mov       x1, xzr
       b         M00_L00
M00_L02:
       movz      x0, #0xf030
       movk      x0, #0x6a31, lsl #16
       movk      x0, #0xffff, lsl #32
       ldr       x0, [x0]
       blr       x0
       brk       #0
M00_L03:
       bl        #0xffff6a0900f8
       brk       #0
; Total bytes of code 252
```

## .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
```assembly
; LuhnAlgorithmBenchmarking.LuhnCodeBenchmark.GenerateLuhnDigitSimdVector128()
; 		var inputVector = Vector128.Create(_cardCode);
; 		^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 		var multipliedVector = Vector128.Add(
; 		^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 			digitVector,
;
; 			Vector128.ConditionalSelect(
;
; 				Vector128.Create(_mask),
;
; 				digitVector,
;
; 				Vector128<byte>.Zero
;
; 			)
;
; 		);
;
; 		return (byte) ((10 - sum % 10) % 10);
; 		^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
       stp       x29, x30, [sp, #-0x10]!
       mov       x29, sp
       ldr       x1, [x0, #8]
       ldr       w2, [x1, #8]
       cmp       w2, #0x10
       b.lt      M00_L00
       ldr       q16, [x1, #0x10]
       ldr       q17, #0xffff659cfe00
       sub       v16.16b, v16.16b, v17.16b
       ldr       x0, [x0, #0x18]
       ldr       w1, [x0, #8]
       cmp       w1, #0x10
       b.lt      M00_L00
       ldr       q17, [x0, #0x10]
       movi      v18.4s, #0
       bsl       v17.16b, v16.16b, v18.16b
       add       v16.16b, v16.16b, v17.16b
       ldr       q17, #0xffff659cfe10
       cmhi      v17.16b, v16.16b, v17.16b
       ldr       q18, #0xffff659cfe10
       movi      v19.4s, #0
       bsl       v17.16b, v18.16b, v19.16b
       sub       v16.16b, v16.16b, v17.16b
       addv      b16, v16.16b
       umov      w0, v16.b[0]
       movz      w1, #0xcccd
       movk      w1, #0xcccc, lsl #16
       umull     x1, w0, w1
       lsr       x1, x1, #0x23
       movz      w2, #0xa
       msub      w0, w1, w2, w0
       neg       w0, w0
       add       w0, w0, #0xa
       movz      w1, #0x6667
       movk      w1, #0x6666, lsl #16
       smull     x1, w1, w0
       asr       x1, x1, #0x20
       asr       w2, w1, #2
       add       w1, w2, w1, lsr #31
       movz      w2, #0xa
       msub      w0, w1, w2, w0
       uxtb      w0, w0
       ldp       x29, x30, [sp], #0x10
       ret
M00_L00:
       movz      x0, #0xf030
       movk      x0, #0x65c2, lsl #16
       movk      x0, #0xffff, lsl #32
       ldr       x0, [x0]
       blr       x0
       brk       #0
; Total bytes of code 200
```