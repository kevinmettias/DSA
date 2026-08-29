using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class WildcardMatchingBenchmarks
{
    private string _text = null!; private string _pattern = null!;
    [Params(20, 80)] public int Length;
    [GlobalSetup] public void Setup() { _text = new string('a', Length) + "b"; _pattern = "*a*b"; }
    [Benchmark(Baseline = true)] public bool GreedyTwoPointer() { var s = 0; var p = 0; var star = -1; var match = 0; while (s < _text.Length) { if (p < _pattern.Length && (_pattern[p] == '?' || _pattern[p] == _text[s])) { s++; p++; } else if (p < _pattern.Length && _pattern[p] == '*') { star = p++; match = s; } else if (star != -1) { p = star + 1; s = ++match; } else return false; } while (p < _pattern.Length && _pattern[p] == '*') p++; return p == _pattern.Length; }
    [Benchmark] public bool MemoizedDp() { return Memoizer.Memoize<(int Text, int Pattern), bool>((0, 0), MatchFrom); bool MatchFrom((int Text, int Pattern) state, Func<(int Text, int Pattern), bool> match) { var (i, j) = state; if (j == _pattern.Length) return i == _text.Length; if (_pattern[j] == '*') return match((i, j + 1)) || (i < _text.Length && match((i + 1, j))); return i < _text.Length && (_pattern[j] == '?' || _pattern[j] == _text[i]) && match((i + 1, j + 1)); } }
}
