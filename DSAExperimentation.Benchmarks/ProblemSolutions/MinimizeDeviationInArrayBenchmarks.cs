using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimizeDeviationInArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimizeDeviationInArraySolution's, the same methods
// MinimizeDeviationInArrayTests proves correct. Values spread over a wide range
// keep the halving sequence long, so the cost is dominated by how the current
// largest value is found - the one thing the two arms differ in.
[MemoryDiagnoser]
public class MinimizeDeviationInArrayBenchmarks
{
    // LC problem number, used as the deterministic setup seed.
    private const int RandomSeed = 1675;
    private const int RandomValueUpperBound = 1_000_000;

    private int[] _nums = [];

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, RandomValueUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearRescanEachStep() =>
        MinimizeDeviationInArraySolution.MinimumDeviationByLinearRescan(_nums);

    [Benchmark]
    public int MaxHeapReduce() =>
        MinimizeDeviationInArraySolution.MinimumDeviationByMaxHeap(_nums);
}
