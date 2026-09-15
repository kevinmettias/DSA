using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MakeLexicographicallySmallestArrayBySwappingElements;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MakeLexicographicallySmallestArrayBySwappingElementsSolution's, the same
// methods MakeLexicographicallySmallestArrayBySwappingElementsTests proves
// correct. A small limit relative to the value range keeps most sorted-adjacent
// gaps above it, so both strategies see a realistic mix of small and large
// swappable groups rather than one group spanning the whole array.
[MemoryDiagnoser]
public class MakeLexicographicallySmallestArrayBySwappingElementsBenchmarks
{
    private const int MaxValueExclusive = 1_000;
    private const int Limit = 5;
    private const int Seed = 2948;

    private int[] _nums = [];

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] ContiguousGroups() =>
        MakeLexicographicallySmallestArrayBySwappingElementsSolution
            .LexicographicallySmallestArrayByContiguousGroups(_nums, Limit);

    [Benchmark]
    public int[] DisjointSet() =>
        MakeLexicographicallySmallestArrayBySwappingElementsSolution
            .LexicographicallySmallestArrayByDisjointSet(_nums, Limit);
}
