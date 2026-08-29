using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class PermutationsBenchmarks
{
    private int[] _values = null!; [Params(6, 8)] public int Length; [GlobalSetup] public void Setup() => _values = Enumerable.Range(1, Length).ToArray();
    [Benchmark(Baseline = true)] public int SpecializedRecursive() { var used = new bool[_values.Length]; var count = 0; void Search(int depth) { if (depth == _values.Length) { count++; return; } for (var i = 0; i < _values.Length; i++) if (!used[i]) { used[i] = true; Search(depth + 1); used[i] = false; } } Search(0); return count; }
    [Benchmark] public int Backtracking() { var count = 0; var state = new State(_values.Length); Backtrack.Search<State, int>(state, s => s.Depth == _values.Length, s => s.Depth == _values.Length ? [] : Enumerable.Range(0, _values.Length).Where(i => !s.Used[i]), (s, i) => { s.Used[i] = true; s.Depth++; }, (s, i) => { s.Used[i] = false; s.Depth--; }, _ => count++); return count; }
    private sealed class State(int length) { public bool[] Used { get; } = new bool[length]; public int Depth { get; set; } }
}
