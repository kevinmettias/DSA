using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindPolygonWithTheLargestPerimeter;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindPolygonWithTheLargestPerimeterSolution's,
// the same methods FindPolygonWithTheLargestPerimeterTests proves correct.
// Neither strategy needs anything hoisted beyond the raw int[] LeetCode
// itself hands in, so [GlobalSetup] only sizes+seeds the workload.
[MemoryDiagnoser]
public class FindPolygonWithTheLargestPerimeterBenchmarks
{
    private const int SideSeed = 2971;

    private int[] _sides = [];

    [Params(16, 20)]
    public int SideCount { get; set; }

    [GlobalSetup]
    public void Setup() => _sides = PolygonWorkloads.BuildSides(SideCount, seed: SideSeed);

    [Benchmark(Baseline = true)]
    public long BruteForceSubsets() => FindPolygonWithTheLargestPerimeterSolution.LargestPerimeterByBruteForceSubsets(_sides);

    [Benchmark]
    public long SortedRunningSum() => FindPolygonWithTheLargestPerimeterSolution.LargestPerimeterBySortedRunningSum(_sides);
}
