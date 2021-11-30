
* 참고 
	* [ValueStringBuilder](https://github.com/dotnet/runtime/blob/a9c5eadd951dcba73167f72cc624eb790573663a/src/libraries/Common/src/System/Text/ValueStringBuilder.cs)
	* [ZString](https://github.com/Cysharp/ZString)
	* [ObjectPool](https://docs.microsoft.com/ko-kr/aspnet/core/performance/objectpool?view=aspnetcore-5.0)
	* 모든 StringBuilder는 'string.Length == 1' 이거나 char를 받는 경우에는 속도가 비슷하다
		* 따라서, testString이 긴 경우에만 테스트를 한다


* [MyWiki](https://github.com/junhun0106/CSharp/wiki/%5BOptimize%5D-StringBuilder)

---

#### .net 5.0 + .net standard 2.1
	
* **without capacity**

|                   Method |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
|------------------------- |---------:|---------:|---------:|-------:|----------:|
|      StringBuilderAppend | 79.22 ns | 0.559 ns | 0.495 ns | 0.0411 |     344 B |
|  StringBuilderPoolAppend | 33.84 ns | 0.277 ns | 0.246 ns |      - |         - |
|            ZStringAppend | 37.11 ns | 0.477 ns | 0.446 ns |      - |         - |
| ValueStringBuilderAppend | 95.14 ns | 1.035 ns | 0.969 ns |      - |         - |

* StringBuilder
	* .net 버전 업에 따라 StringBuilder 내부 코드도 많은 최적화가 이루어졌기 때문에 생각보다 좋은 성능을 내고 있다.
		* 단, StringBuilder는 class이기 때문에 할당이 일어나는 것을 볼 수 있다
	* 참고로 StringBuilder는 'default capacity = 16' 이다.
	
* StringBuilderPool vs ZString
	* 두 방법 모두 내부에 default capacity를 들고 있기 때문에 작은 단위에 string은 차이가 없는 것을 볼 수 있다
		* StringBuilderPool은 내부에서 StringBuilder(100), 최대 사이즈는 4k로 지정되어 있음.
		* ZString은 32768로 고정 되어 있음.

* ValueStringBuilder
	* capacity가 없기 때문에 내부에서 ArrayPool<char>.Rent 및 Glow가 계속 발생하여 성능 저하를 일으키고 있다.
		* struct는 매게 변수가 없는 생성자는 만들 수 없기 때문에 'new ValueStringBuilder()' 같은 구문은 컴파일 에러는 아니지만, 의도가 맞지 않는다

---
		
* **with capacity**

testString.Length = 150

|                   Method |     Mean |    Error |   StdDev |  Gen 0 |  Gen 1 | Allocated |
|------------------------- |---------:|---------:|---------:|-------:|-------:|----------:|
|      StringBuilderAppend | 45.64 ns | 0.238 ns | 0.222 ns | 0.0564 | 0.0001 |     472 B |
|  StringBuilderPoolAppend | 40.60 ns | 0.212 ns | 0.199 ns |      - |      - |         - |
|            ZStringAppend | 39.39 ns | 0.184 ns | 0.163 ns |      - |      - |         - |
| ValueStringBuilderAppend | 43.86 ns | 0.195 ns | 0.152 ns |      - |      - |         - |

testString.Length = 1000

|                   Method |       Mean |    Error |   StdDev |  Gen 0 |  Gen 1 | Allocated |
|------------------------- |-----------:|---------:|---------:|-------:|-------:|----------:|
|      StringBuilderAppend |   759.5 ns | 10.13 ns |  9.48 ns | 3.4475 |      - |  28,976 B |
|  StringBuilderPoolAppend | 1,449.4 ns | 10.78 ns | 10.09 ns | 4.7112 | 0.5875 |  39,488 B |
|            ZStringAppend |   408.4 ns |  7.94 ns | 11.89 ns |      - |      - |         - |
| ValueStringBuilderAppend |   417.7 ns |  8.21 ns | 11.51 ns |      - |      - |         - |


* 위 결과에서 알 수 있듯이, StringBuilderPool은 default length를 넘어가기 시작하면 엄청나게 느려지는 것을 확인 할 수 있다
	* MSDN에서도 ObjectPool을 사용 할 때는 제한 된 리소스와 예측 가능하고 자주 사용하는 경우에만 사용하라고 되어 있다
	* 물론, StringBuilderPool은 default를 바꿀 수 있다.

* ZString과 ValueStringBuilder는 capacity만 잘 쓴다면 둘이 비슷한 성능을 내는 것으로 보인다
	* 단, ZString에 경우는 내부에서 ArrayPool에 '32768'을 Rent 받고 있으므로 제법 큰 숫자를 Rent하는 차이가 있다
	* 단, ValueStringBuilder에 경우는 .NET 버전이 높아야 컴파일 에러 없이 사용이 가능하다.(using 키워드 사용으로 간편해진다)

---
	
* testString.Length = 1000, .net core 3.1 + .net standard 2.0

|                   Method |       Mean |    Error |  StdDev |  Gen 0 |  Gen 1 | Allocated |
|------------------------- |-----------:|---------:|--------:|-------:|-------:|----------:|
|      StringBuilderAppend | 1,253.8 ns |  3.60 ns | 3.37 ns | 3.4466 | 0.3128 |  28,976 B |
|  StringBuilderPoolAppend | 1,725.2 ns |  7.44 ns | 6.60 ns | 4.7169 | 0.5875 |  39,488 B |
|            ZStringAppend |   566.9 ns | 10.85 ns | 9.61 ns |      - |      - |         - |
| ValueStringBuilderAppend |   565.9 ns |  1.61 ns | 1.26 ns |      - |      - |         - |	

* 테스트 환경은 똑같지만, .net 버전을 낮춰서 테스트해보자.
	* .net 버전이 낮은 경우는 StringBuilder가 최적화가 덜 되었기 때문에 속도가 더 느려지는 걸 확인 할 수 있다
	* ZString, ValueStringBuilder 역시 ArrayPool 등에 최적화 처리가 없기 때문에 더 느려지는 걸 확인 할 수 있다
	* ZString, ValueStringBuilder vs StringBuilder
		* 압도적으로 전자승
	
---

* 결론
	* string.Length가 예측이 가능하고, capacity의 최소, 최대의 차이가 크지 않고 자주 사용한다면 StringBuilderPool도 나쁘지 않은 선택이다
	* capacity를 정확히 알 수 있다면 ValueStringBuilder로 선택해보자.
	* capacity가 예측이 불가능하다면(범용적으로 사용하고 싶다면), ZString를 사용하는 것을 고려해보자.
		* ZString에 기본으로 ArrayPool에 Rent해오는 값은 '32768'이며, const라 변경도 불가능하다.
