using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.LargestPerimeterTriangle;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LargestPerimeterTriangleSolution's, the same methods
// LargestPerimeterTriangleTests proves correct. Neither strategy needs anything
// hoisted beyond the raw int[] LeetCode itself hands in, so [GlobalSetup] only
// sizes+seeds the workload - the cubic baseline is what caps the sizes at 80/300.
[MemoryDiagnoser]
public class LargestPerimeterTriangleBenchmarks
{
    private const int RandomSeed = 976; // LC problem number

    [Params(80, 300)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup() => _values = PolygonWorkloads.BuildSides(Length, seed: RandomSeed);

    [Benchmark(Baseline = true)]
    public int BruteForce() => LargestPerimeterTriangleSolution.LargestPerimeterByBruteForceTriples(_values);

    [Benchmark]
    public int SortThenScan() => LargestPerimeterTriangleSolution.LargestPerimeterBySortedScan(_values);
}
