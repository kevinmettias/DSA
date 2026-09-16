using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfSetsOfKNonOverlappingLineSegments;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// NumberOfSetsOfKNonOverlappingLineSegmentsSolution's, the same methods
// NumberOfSetsOfKNonOverlappingLineSegmentsTests proves correct. The problem's whole
// input is two integers, so [GlobalSetup] only picks k from the point count.
[MemoryDiagnoser]
public class NumberOfSetsOfKNonOverlappingLineSegmentsBenchmarks
{
    private const int PointsToKDivisor = 4;

    private int _segmentCount;

    [Params(50, 500)]
    public int Points { get; set; }

    [GlobalSetup]
    public void Setup() => _segmentCount = Points / PointsToKDivisor;

    [Benchmark(Baseline = true)]
    public int Tabulation() =>
        NumberOfSetsOfKNonOverlappingLineSegmentsSolution.NumberOfSetsByTabulation(Points, _segmentCount);

    [Benchmark]
    public int Memoized() =>
        NumberOfSetsOfKNonOverlappingLineSegmentsSolution.NumberOfSetsByMemoizedPascal(Points, _segmentCount);
}
