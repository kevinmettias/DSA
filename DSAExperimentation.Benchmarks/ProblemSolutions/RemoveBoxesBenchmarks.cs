using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.RemoveBoxes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveBoxesSolution's, the same methods
// RemoveBoxesTests proves correct. BoxCount stays well under LeetCode's own limit
// (<=24, vs. LC's 100) specifically because the un-memoized baseline's blowup is
// real - see RemoveBoxesWorkloads for why.
[MemoryDiagnoser]
public class RemoveBoxesBenchmarks
{
    private const int RandomSeed = 1;

    [Params(16, 24)]
    public int BoxCount;

    private int[] _boxes = null!;

    [GlobalSetup]
    public void Setup() => _boxes = RemoveBoxesWorkloads.BuildBoxes(BoxCount, seed: RandomSeed);

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => RemoveBoxesSolution.MaxPointsByUnmemoizedRecursion(_boxes);

    [Benchmark]
    public int MemoizedRecursion() => RemoveBoxesSolution.MaxPointsByMemoizedRecursion(_boxes);
}
