using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumPairRemovalToSortArrayII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumPairRemovalToSortArrayIISolution's, the
// same methods MinimumPairRemovalToSortArrayIITests proves correct. A wide,
// uniformly random value range keeps the array genuinely out of order at every
// scale, so both arms have real merge work to do rather than finishing in the
// first few operations.
[MemoryDiagnoser]
public class MinimumPairRemovalToSortArrayIIBenchmarks
{
    private const int RandomSeed = 3510; // LeetCode problem number
    private const int MinValue = -1_000;
    private const int MaxValueExclusive = 1_000;

    private int[] _nums = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            _nums[i] = random.Next(MinValue, MaxValueExclusive);
        }
    }

    [Benchmark(Baseline = true)]
    public int BruteForceScan() => MinimumPairRemovalToSortArrayIISolution.MinOperationsByBruteForceScan(_nums);

    [Benchmark]
    public int LazyHeap() => MinimumPairRemovalToSortArrayIISolution.MinOperationsByLazyHeap(_nums);
}
