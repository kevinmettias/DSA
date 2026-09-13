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

    [Params(50, 500)]
    public int Points;

    private int _k;

    [GlobalSetup]
    public void Setup() => _k = Points / PointsToKDivisor;

    [Benchmark(Baseline = true)]
    public int Tabulation() =>
        NumberOfSetsOfKNonOverlappingLineSegmentsSolution.NumberOfSetsByTabulation(Points, _k);

    [Benchmark]
    public int Memoized() =>
        NumberOfSetsOfKNonOverlappingLineSegmentsSolution.NumberOfSetsByMemoizedPascal(Points, _k);
}
