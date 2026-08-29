using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class InsertIntervalBenchmarks
{
    private (int Start, int End)[] _intervals = null!;
    [Params(200, 5_000)] public int Length;
    [GlobalSetup] public void Setup() => _intervals = Enumerable.Range(0, Length).Select(i => (i * 3, (i * 3) + 1)).ToArray();
    [Benchmark(Baseline = true)] public int ListInsertAndMerge() { var list = _intervals.ToList(); list.Add((Length, Length * 2)); list.Sort((a,b) => a.Start.CompareTo(b.Start)); var merged = new List<(int Start,int End)>(); foreach (var interval in list) { if (merged.Count == 0 || interval.Start > merged[^1].End) merged.Add(interval); else merged[^1] = (merged[^1].Start, Math.Max(merged[^1].End, interval.End)); } return merged.Count; }
    [Benchmark] public int IntervalSetAdd() { var set = new IntervalSet<int>(); foreach (var (start,end) in _intervals) set.Add(start,end); set.Add(Length, Length * 2); return set.Count; }
}
