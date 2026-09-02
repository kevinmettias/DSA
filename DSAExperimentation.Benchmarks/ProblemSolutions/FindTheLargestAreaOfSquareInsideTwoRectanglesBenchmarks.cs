using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindTheLargestAreaOfSquareInsideTwoRectangles;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// FindTheLargestAreaOfSquareInsideTwoRectanglesSolution's, the same methods
// FindTheLargestAreaOfSquareInsideTwoRectanglesTests proves correct.
// Rectangle construction is charged to [GlobalSetup]; the sort the pruned
// arm performs is part of the strategy being measured, not preparation for
// it.
[MemoryDiagnoser]
public class FindTheLargestAreaOfSquareInsideTwoRectanglesBenchmarks
{
    private const int Seed = 3047;

    [Params(50, 300)]
    public int RectangleCount;

    private int[][] _bottomLeft = null!;
    private int[][] _topRight = null!;

    [GlobalSetup]
    public void Setup() => (_bottomLeft, _topRight) = RectangleWorkloads.BuildRectangles(RectangleCount, seed: Seed);

    [Benchmark(Baseline = true)]
    public long BruteForcePairs() =>
        FindTheLargestAreaOfSquareInsideTwoRectanglesSolution.LargestSquareAreaByBruteForcePairs(_bottomLeft, _topRight);

    [Benchmark]
    public long SortedPrunedPairs() =>
        FindTheLargestAreaOfSquareInsideTwoRectanglesSolution.LargestSquareAreaBySortedPrunedPairs(_bottomLeft, _topRight);
}
