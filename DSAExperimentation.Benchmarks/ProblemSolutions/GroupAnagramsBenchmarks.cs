using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class GroupAnagramsBenchmarks
{
    private string[] _values = null!; [Params(200, 5_000)] public int Length; [GlobalSetup] public void Setup() => _values = Enumerable.Range(0, Length).Select(i => i % 2 == 0 ? "eat" : "tea").ToArray();
    [Benchmark(Baseline = true)] public int DictionaryGroup() => _values.GroupBy(Key).Count();
    [Benchmark] public int HashMapGroup() { var map = new HashMap<string, List<string>>(); foreach (var value in _values) { var key = Key(value); if (!map.TryGetValue(key, out var group)) { group = []; map.Set(key, group); } group.Add(value); } return map.Count; }
    private static string Key(string value) { var chars = value.ToCharArray(); Array.Sort(chars); return new string(chars); }
}
