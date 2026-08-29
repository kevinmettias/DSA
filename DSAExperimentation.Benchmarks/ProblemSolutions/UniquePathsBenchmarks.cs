using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class UniquePathsBenchmarks
{
    [Params(10, 18)] public int Size;
    [Benchmark(Baseline = true)] public long Combinatorics() { long result = 1; for (var i = 1; i <= Size - 1; i++) result = result * (Size - 1 + i) / i; return result; }
    [Benchmark] public int MemoizedRecurrence() { var n = Size; return Memoizer.Memoize<(int Row,int Col), int>((0,0), WaysFrom); int WaysFrom((int Row,int Col) state, Func<(int Row,int Col), int> ways) { var (r,c)=state; if (r == n - 1 && c == n - 1) return 1; var total = 0; if (r + 1 < n) total += ways((r + 1,c)); if (c + 1 < n) total += ways((r,c + 1)); return total; } }
}
