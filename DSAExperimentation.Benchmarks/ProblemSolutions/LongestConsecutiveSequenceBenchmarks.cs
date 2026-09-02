using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.LongestConsecutiveSequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one arm is LongestConsecutiveSequenceSolution's, the
// same method LongestConsecutiveSequenceTests proves correct.
[MemoryDiagnoser]
public class LongestConsecutiveSequenceBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup() => _nums = LongestConsecutiveSequenceWorkloads.BuildArray(Length, seed: 128);

    [Benchmark]
    public int SetRunExpansion() => LongestConsecutiveSequenceSolution.LongestConsecutiveBySetRunExpansion(_nums);
}
