using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class CombinationSumBenchmarks
{
    private int[] _candidates = null!;
    [Params(30, 60)] public int Target;
    [GlobalSetup] public void Setup() => _candidates = [2, 3, 5, 7];
    [Benchmark(Baseline = true)] public int SpecializedRecursive() { var count = 0; void Search(int start, int sum) { if (sum == Target) { count++; return; } for (var i = start; i < _candidates.Length && sum + _candidates[i] <= Target; i++) Search(i, sum + _candidates[i]); } Search(0, 0); return count; }
    [Benchmark] public int Backtracking() { var count = 0; var state = new State(); Backtrack.Search<State, int>(state, s => s.Sum == Target, s => s.Sum == Target ? [] : Enumerable.Range(s.Start, _candidates.Length - s.Start).Where(i => s.Sum + _candidates[i] <= Target), (s, i) => { s.Starts.Push(s.Start); s.Values.Add(_candidates[i]); s.Sum += _candidates[i]; s.Start = i; }, (s, i) => { s.Start = s.Starts.Pop(); s.Sum -= _candidates[i]; s.Values.RemoveAt(s.Values.Count - 1); }, _ => count++); return count; }
    private sealed class State { public Stack<int> Starts { get; } = new(); public List<int> Values { get; } = []; public int Sum { get; set; } public int Start { get; set; } }
}

