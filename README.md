# CSharp-Benchmark

* CSharp Benchmark storage

---

### C Sharp 벤치마크 모음

* [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) 활용
* 자주 사용하고 이론으로 알고는 있지만 실수하거나 애매한 부분들에 대해서 벤치마크

---

### Benchmark list

#### [Array](https://github.com/junhun0106/CSharp-Benchmark/blob/main/Benchmarks/Benchmark/ArrayBenchmark.cs)
* [Wiki](https://github.com/junhun0106/CSharp/wiki/%5BBenchmark%5D-Array)
* Array.Find vs Array.FirstOrDefault(linq)
* Array.FindAll vs Array.Where(linq)
* Array.BinarySearch vs foreach match vs for match vs Array.Contains(linq)
* Array.Copy vs Buffer.BlockCopy

#### [Dictionary](https://github.com/junhun0106/CSharp-Benchmark/blob/main/Benchmarks/Benchmark/DictionaryBenchmark.cs)

* [Wiki](https://github.com/junhun0106/CSharp/wiki/%5BBenchmark%5D-Dictionary)
* Dictionry.ContainsKey + Dictionry[key] vs Dictionry.TryGetValue
  * .NET 7 Pref Analisys : [CA1836](https://learn.microsoft.com/ko-kr/dotnet/fundamentals/code-analysis/quality-rules/ca1836) 
* Dictionary.TryGetValue in struct(with IEquatable) key, without IEqualityComparer vs with IEqualityComparer
* Dictionary find value.field foreach vs Dictionary.where
* Dictionary ElementAt vs ElementAtOrDefault vs First vs FirstOrDefault vs Foreach(1 loop break)
* Dictionary foreach all value vs foreach .Values

#### [Enum]

#### Enumerable

#### Linq

#### List

#### Local function

#### Queue

