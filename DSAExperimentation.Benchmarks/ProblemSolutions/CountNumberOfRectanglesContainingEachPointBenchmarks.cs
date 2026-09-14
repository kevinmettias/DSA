using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountNumberOfRectanglesContainingEachPoint;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountNumberOfRectanglesContainingEachPointSolution's,
// the same methods CountNumberOfRectanglesContainingEachPointTests proves correct.
// [GlobalSetup] builds the rectangle and point arrays - LeetCode's own input shape,
// so nothing further is prepared for the measured methods.
//
// The grouped arm pays a fixed one-time cost up front (building the HashMap,
// MergeSort-ing every bucket) that brute force never pays, so at RectangleCount=500
// it loses to brute force's simple O(n*m) scan; it only wins once RectangleCount
// grows large enough that O((n+m) log n) undercuts O(n*m) by more than that fixed
// setup cost covers, which is what RectangleCount=4_000 demonstrates.
[MemoryDiagnoser]
public class CountNumberOfRectanglesContainingEachPointBenchmarks
{
    private const int RandomSeed = 2250; // LC problem number
    private const int MaxHeightExclusive = 101;
    private const int CoordinateBoundExclusive = 1_000_000_000;

    [Params(500, 4_000)]
    public int RectangleCount;

    private int[][] _rectangles = null!;
    private int[][] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _rectangles = Enumerable.Range(0, RectangleCount)
            .Select(_ => new[] { random.Next(1, CoordinateBoundExclusive), random.Next(1, MaxHeightExclusive) })
            .ToArray();
        _points = Enumerable.Range(0, RectangleCount)
            .Select(_ => new[] { random.Next(1, CoordinateBoundExclusive), random.Next(1, MaxHeightExclusive) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() =>
        CountNumberOfRectanglesContainingEachPointSolution.CountRectanglesByBruteForce(_rectangles, _points);

    [Benchmark]
    public int[] GroupedSortedBinarySearch() =>
        CountNumberOfRectanglesContainingEachPointSolution.CountRectanglesByGroupedLowerBound(_rectangles, _points);
}
