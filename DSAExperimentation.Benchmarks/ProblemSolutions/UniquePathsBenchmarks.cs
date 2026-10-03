using DSAExperimentation.LeetCode.UniquePaths;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are UniquePathsSolution's, the same methods
// UniquePathsSolutionTests proves correct, run on a square Size x Size grid. 17 is the
// largest square grid whose path count, C(32, 16) = 601,080,390, stays inside LC 62's
// guarantee that the answer is at most 2 * 10^9; at 18 the count passes int.MaxValue and
// both arms would wrap to the same wrong number.
public class UniquePathsBenchmarks
{
    [Params(10, 17)]
    public int Size { get; set; }

    [Benchmark(Baseline = true)]
    public int Combinatorics() => UniquePathsSolution.CountPathsByCombinatorics(Size, Size);

    [Benchmark]
    public int MemoizedRecurrence() => UniquePathsSolution.CountPathsByMemoizedRecurrence(Size, Size);
}
