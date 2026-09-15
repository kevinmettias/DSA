using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PoorPigs;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PoorPigsSolution's, the same methods PoorPigsTests
// proves correct. Basis is fixed at 2 (minutesToDie == minutesToTest) so Buckets
// alone drives how many pigs are needed.
[MemoryDiagnoser]
public class PoorPigsBenchmarks
{
    private const int MinutesToDie = 15;
    private const int MinutesToTest = 15;

    [Params(100, 1_000)]
    public int Buckets { get; set; }

    [Benchmark(Baseline = true)]
    public int LinearRecompute() =>
        PoorPigsSolution.MinPigsByLinearRecompute(Buckets, MinutesToDie, MinutesToTest);

    [Benchmark]
    public int BinarySearchOverPowers() =>
        PoorPigsSolution.MinPigsByBinarySearch(Buckets, MinutesToDie, MinutesToTest);
}
