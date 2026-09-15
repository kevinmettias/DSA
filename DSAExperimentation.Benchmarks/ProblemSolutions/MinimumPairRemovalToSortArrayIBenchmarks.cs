using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumPairRemovalToSortArrayI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumPairRemovalToSortArrayISolution's, the
// same methods MinimumPairRemovalToSortArrayITests proves correct. Sizes stay
// within LC 3507 "I"'s own n <= 50 bound (unlike its "II" sibling), so this
// times the two strategies' constant factors rather than a regime "I" never
// actually runs in.
[MemoryDiagnoser]
public class MinimumPairRemovalToSortArrayIBenchmarks
{
    private const int Seed = 3507;
    private const int MinValueInclusive = -1_000;
    private const int MaxValueExclusive = 1_001;

    private int[] _nums = [];

    [Params(10, 50)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(MinValueInclusive, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => MinimumPairRemovalToSortArrayISolution.MinOperationsByBruteForce(_nums);

    [Benchmark]
    public int LazyPairHeap() => MinimumPairRemovalToSortArrayISolution.MinOperationsByLazyPairHeap(_nums);
}
