using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.UniquePaths;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are UniquePathsSolution's, the same methods
// UniquePathsTests proves correct, run on a square Size x Size grid.
[MemoryDiagnoser]
public class UniquePathsBenchmarks
{
    [Params(10, 18)] public int Size;

    [Benchmark(Baseline = true)]
    public int Combinatorics() => UniquePathsSolution.CountPathsByCombinatorics(Size, Size);

    [Benchmark]
    public int MemoizedRecurrence() => UniquePathsSolution.CountPathsByMemoizedRecurrence(Size, Size);
}
