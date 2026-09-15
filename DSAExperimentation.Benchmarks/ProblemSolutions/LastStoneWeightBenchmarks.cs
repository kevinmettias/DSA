using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LastStoneWeight;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LastStoneWeightSolution's. Random weights over a wide
// range keep differences non-zero most of the time, so stones are re-inserted
// rather than annihilating and the smash sequence stays long.
[MemoryDiagnoser]
public class LastStoneWeightBenchmarks
{
    private const int RandomSeed = 1046; // LC problem number
    private const int MaxStoneWeightExclusive = 1_000;

    private int[] _stones = [];

    [Params(200, 5_000)]
    public int StoneCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _stones = Enumerable.Range(0, StoneCount).Select(_ => random.Next(1, MaxStoneWeightExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearRescanEachSmash() => LastStoneWeightSolution.LastStoneWeightByLinearRescan(_stones);

    [Benchmark]
    public int MaxHeapSmash() => LastStoneWeightSolution.LastStoneWeightByMaxHeap(_stones);
}
