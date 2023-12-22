## .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
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
       sub       rsp,28
       xor       eax,eax
       xor       edx,edx
       mov       rcx,[rcx+8]
       cmp       dword ptr [rcx+8],0
       jle       short M00_L02
M00_L00:
       mov       r8,rcx
       cmp       edx,[r8+8]
       jae       short M00_L03
       mov       r10d,edx
       movzx     r8d,byte ptr [r8+r10+10]
       add       r8d,0FFFFFFD0
       movzx     r8d,r8b
       test      dl,1
       jne       short M00_L01
       add       r8d,r8d
       movzx     r8d,r8b
       cmp       r8d,9
       jle       short M00_L01
       add       r8d,0FFFFFFF7
       movzx     r8d,r8b
M00_L01:
       add       eax,r8d
       movzx     eax,al
       inc       edx
       cmp       [rcx+8],edx
       jg        short M00_L00
M00_L02:
       mov       edx,0CCCCCCCD
       mov       ecx,eax
       imul      rdx,rcx
       shr       rdx,23
       imul      edx,0A
       sub       eax,edx
       mov       ecx,eax
       neg       ecx
       add       ecx,0A
       mov       edx,66666667
       mov       eax,edx
       imul      ecx
       mov       eax,edx
       shr       eax,1F
       sar       edx,2
       add       eax,edx
       lea       eax,[rax+rax*4]
       add       eax,eax
       sub       ecx,eax
       movzx     eax,cl
       add       rsp,28
       ret
M00_L03:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 150
```

## .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
```assembly
; LuhnAlgorithmBenchmarking.LuhnCodeBenchmark.GenerateLuhnDigitSimdSse2()
; 			fixed (byte* cardCodeRef = _cardCode)
; 			       ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 				inputVector = Sse2.LoadVector128(cardCodeRef);
; 				^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 		var multiplyResult = Sse2.Add(
; 		^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 			substractResult,
; 			                
; 			MultiplyBytes(
; 			              
; 				Sse2.Subtract(
; 				              
; 					Vector128<byte>.Zero.AsSByte(),
; 					                               
; 					Vector128.Create(_mask).AsSByte()
; 					                                 
; 				).AsByte(),
; 				           
; 				substractResult
; 				               
; 			)
; 			 
; 		);
; 		  
; 		var nineVector = Vector128.Create((byte) 9);
; 		^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 		var vsum = Sse2.SumAbsoluteDifferences(
; 		^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 			Sse2.Subtract(
; 			              
; 				multiplyResult,
; 				               
; 				MultiplyBytes(
; 				              
; 					Sse2.Subtract(
; 					              
; 						Vector128<byte>.Zero.AsSByte(),
; 						                               
; 						Sse2.CompareGreaterThan(
; 						                        
; 							multiplyResult.AsSByte(),
; 							                         
; 							nineVector.AsSByte()
; 							                    
; 						)
; 						 
; 					).AsByte(),
; 					           
; 					nineVector
; 					          
; 				)
; 				 
; 			),
; 			  
; 			Vector128<byte>.Zero
; 			                    
; 		);
; 		  
; 		var sum = (byte) (Sse2.Extract(vsum, 0) + Sse2.Extract(vsum, 4));
; 		^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 		return (byte) ((10 - sum % 10) % 10);
; 		^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
       sub       rsp,28
       vzeroupper
       mov       rax,[rcx+8]
       mov       [rsp+20],rax
       test      rax,rax
       je        near ptr M00_L01
       mov       rax,[rsp+20]
       cmp       dword ptr [rax+8],0
       je        near ptr M00_L01
       mov       rax,[rsp+20]
       cmp       dword ptr [rax+8],0
       jbe       near ptr M00_L03
       mov       r8,[rsp+20]
       add       r8,10
M00_L00:
       vmovups   xmm0,[r8]
       xor       eax,eax
       mov       [rsp+20],rax
       vpsubb    xmm0,xmm0,[7FF8ABE278E0]
       mov       rax,[rcx+18]
       mov       edx,[rax+8]
       cmp       edx,10
       jl        near ptr M00_L02
       vxorps    xmm1,xmm1,xmm1
       vpsubb    xmm1,xmm1,[rax+10]
       vpsrlw    xmm2,xmm1,8
       vpsrlw    xmm3,xmm0,8
       vpmullw   xmm2,xmm2,xmm3
       vpsllw    xmm2,xmm2,8
       vpmullw   xmm1,xmm1,xmm0
       vpsllw    xmm1,xmm1,8
       vpsrlw    xmm1,xmm1,8
       vpor      xmm1,xmm2,xmm1
       vpaddb    xmm0,xmm0,xmm1
       vpcmpgtb  xmm1,xmm0,[7FF8ABE278F0]
       vxorps    xmm2,xmm2,xmm2
       vpsubb    xmm1,xmm2,xmm1
       vpsrlw    xmm2,xmm1,8
       vpmullw   xmm2,xmm2,[7FF8ABE27900]
       vpsllw    xmm2,xmm2,8
       vpmullw   xmm1,xmm1,[7FF8ABE278F0]
       vpsllw    xmm1,xmm1,8
       vpsrlw    xmm1,xmm1,8
       vpor      xmm1,xmm2,xmm1
       vpsubb    xmm0,xmm0,xmm1
       vxorps    xmm1,xmm1,xmm1
       vpsadbw   xmm0,xmm0,xmm1
       vpextrw   eax,xmm0,0
       vpextrw   edx,xmm0,4
       add       eax,edx
       movzx     eax,al
       mov       edx,0CCCCCCCD
       mov       r8d,eax
       imul      rdx,r8
       shr       rdx,23
       imul      edx,0A
       sub       eax,edx
       mov       ecx,eax
       neg       ecx
       add       ecx,0A
       mov       edx,66666667
       mov       eax,edx
       imul      ecx
       mov       eax,edx
       shr       eax,1F
       sar       edx,2
       add       eax,edx
       lea       eax,[rax+rax*4]
       add       eax,eax
       sub       ecx,eax
       movzx     eax,cl
       add       rsp,28
       ret
M00_L01:
       xor       r8d,r8d
       jmp       near ptr M00_L00
M00_L02:
       call      qword ptr [7FF8ABF8EA48]
       int       3
M00_L03:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 316
```

## .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
```assembly
; LuhnAlgorithmBenchmarking.LuhnCodeBenchmark.GenerateLuhnDigitSimdSse41()
; 			fixed (byte* cardCodeRef = _cardCode)
; 			       ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 				inputVector = Sse2.LoadVector128(cardCodeRef);
; 				^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 		var multiplyResult = Sse2.Add(
; 		^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 			substractResult,
; 			                
; 			Sse41.BlendVariable(
; 			                    
; 				Vector128<byte>.Zero,
; 				                     
; 				substractResult,
; 				                
; 				Vector128.Create(_mask)
; 				                       
; 			)
; 			 
; 		);
; 		  
; 		var nineVector = Vector128.Create((byte) 9);
; 		^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 		var vsum = Sse2.SumAbsoluteDifferences(
; 		^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 			Sse2.Subtract(
; 			              
; 				multiplyResult,
; 				               
; 				Sse41.BlendVariable(
; 				                    
; 					Vector128<byte>.Zero,
; 					                     
; 					nineVector,
; 					           
; 					Sse2.CompareGreaterThan(
; 					                        
; 						multiplyResult.AsSByte(),
; 						                         
; 						nineVector.AsSByte()
; 						                    
; 					).AsByte()
; 					          
; 				)
; 				 
; 			),
; 			  
; 			Vector128<byte>.Zero
; 			                    
; 		);
; 		  
; 		var sum = (byte) (Sse2.Extract(vsum, 0) + Sse2.Extract(vsum, 4));
; 		^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 		return (byte) ((10 - sum % 10) % 10);
; 		^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
       sub       rsp,28
       vzeroupper
       mov       rax,[rcx+8]
       mov       [rsp+20],rax
       test      rax,rax
       je        near ptr M00_L01
       mov       rax,[rsp+20]
       cmp       dword ptr [rax+8],0
       je        near ptr M00_L01
       mov       rax,[rsp+20]
       cmp       dword ptr [rax+8],0
       jbe       near ptr M00_L03
       mov       r8,[rsp+20]
       add       r8,10
M00_L00:
       vmovups   xmm0,[r8]
       xor       eax,eax
       mov       [rsp+20],rax
       vpsubb    xmm0,xmm0,[7FF8ABE17690]
       mov       rax,[rcx+18]
       mov       edx,[rax+8]
       cmp       edx,10
       jl        near ptr M00_L02
       vxorps    xmm1,xmm1,xmm1
       vmovups   xmm2,[rax+10]
       vpblendvb xmm1,xmm1,xmm0,xmm2
       vpaddb    xmm0,xmm0,xmm1
       vxorps    xmm1,xmm1,xmm1
       vpcmpgtb  xmm2,xmm0,[7FF8ABE176A0]
       vpblendvb xmm1,xmm1,[7FF8ABE176A0],xmm2
       vpsubb    xmm0,xmm0,xmm1
       vxorps    xmm1,xmm1,xmm1
       vpsadbw   xmm0,xmm0,xmm1
       vpextrw   eax,xmm0,0
       vpextrw   edx,xmm0,4
       add       eax,edx
       movzx     eax,al
       mov       edx,0CCCCCCCD
       mov       r8d,eax
       imul      rdx,r8
       shr       rdx,23
       imul      edx,0A
       sub       eax,edx
       mov       ecx,eax
       neg       ecx
       add       ecx,0A
       mov       edx,66666667
       mov       eax,edx
       imul      ecx
       mov       eax,edx
       shr       eax,1F
       sar       edx,2
       add       eax,edx
       lea       eax,[rax+rax*4]
       add       eax,eax
       sub       ecx,eax
       movzx     eax,cl
       add       rsp,28
       ret
M00_L01:
       xor       r8d,r8d
       jmp       near ptr M00_L00
M00_L02:
       call      qword ptr [7FF8ABF7EA48]
       int       3
M00_L03:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 251
```

## .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
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
       sub       rsp,38
       vzeroupper
       mov       rax,[rcx+8]
       mov       edx,[rax+8]
       cmp       edx,10
       jl        near ptr M00_L01
       vmovups   xmm0,[rax+10]
       vpsubb    xmm0,xmm0,[7FF8ABDF7880]
       mov       rax,[rcx+18]
       mov       edx,[rax+8]
       cmp       edx,10
       jl        near ptr M00_L01
       vmovups   xmm1,[rax+10]
       vpand     xmm2,xmm1,xmm0
       vxorps    xmm3,xmm3,xmm3
       vpandn    xmm1,xmm1,xmm3
       vpor      xmm1,xmm2,xmm1
       vpaddb    xmm0,xmm0,xmm1
       vmovups   xmm1,[7FF8ABDF7890]
       vpsubb    xmm1,xmm0,xmm1
       vpcmpgtb  xmm1,xmm1,[7FF8ABDF78A0]
       vpblendvb xmm1,xmm3,[7FF8ABDF78B0],xmm1
       vpsubb    xmm0,xmm0,xmm1
       vmovaps   [rsp+20],xmm0
       xor       eax,eax
       xor       edx,edx
       nop       dword ptr [rax]
M00_L00:
       lea       rcx,[rsp+20]
       movsxd    r8,edx
       movzx     ecx,byte ptr [rcx+r8]
       add       eax,ecx
       movzx     eax,al
       inc       edx
       cmp       edx,10
       jl        short M00_L00
       mov       edx,0CCCCCCCD
       mov       ecx,eax
       imul      rdx,rcx
       shr       rdx,23
       imul      edx,0A
       sub       eax,edx
       mov       ecx,eax
       neg       ecx
       add       ecx,0A
       mov       edx,66666667
       mov       eax,edx
       imul      ecx
       mov       eax,edx
       shr       eax,1F
       sar       edx,2
       add       eax,edx
       lea       eax,[rax+rax*4]
       add       eax,eax
       sub       ecx,eax
       movzx     eax,cl
       add       rsp,38
       ret
M00_L01:
       call      qword ptr [7FF8ABF5EA48]
       int       3
; Total bytes of code 221
```

