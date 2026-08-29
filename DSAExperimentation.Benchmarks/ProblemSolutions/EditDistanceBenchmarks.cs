using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class EditDistanceBenchmarks
{
    private string _first = null!; private string _second = null!;
    [Params(20, 80)] public int Length;
    [GlobalSetup] public void Setup() { _first = new string('a', Length); _second = new string('a', Length - 1) + "b"; }
    [Benchmark(Baseline = true)] public int Tabulation() { var dp = new int[_first.Length + 1, _second.Length + 1]; for (var i=0;i<=_first.Length;i++) dp[i, _second.Length] = _first.Length - i; for (var j=0;j<=_second.Length;j++) dp[_first.Length, j] = _second.Length - j; for (var i=_first.Length-1;i>=0;i--) for (var j=_second.Length-1;j>=0;j--) dp[i,j] = _first[i] == _second[j] ? dp[i+1,j+1] : 1 + Math.Min(dp[i+1,j], Math.Min(dp[i,j+1], dp[i+1,j+1])); return dp[0,0]; }
    [Benchmark] public int MemoizedRecurrence() { return Memoizer.Memoize<(int First,int Second), int>((0,0), DistanceFrom); int DistanceFrom((int First,int Second) state, Func<(int First,int Second), int> distance) { var (i,j)=state; if (i == _first.Length) return _second.Length - j; if (j == _second.Length) return _first.Length - i; if (_first[i] == _second[j]) return distance((i+1,j+1)); return 1 + Math.Min(distance((i+1,j)), Math.Min(distance((i,j+1)), distance((i+1,j+1)))); } }
}
