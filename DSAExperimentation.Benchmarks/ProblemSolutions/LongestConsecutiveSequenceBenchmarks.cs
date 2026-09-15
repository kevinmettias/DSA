using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.LongestConsecutiveSequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one arm is LongestConsecutiveSequenceSolution's, the
// same method LongestConsecutiveSequenceTests proves correct.
[MemoryDiagnoser]
public class LongestConsecutiveSequenceBenchmarks
{
    private int[] _nums = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _nums = LongestConsecutiveSequenceWorkloads.BuildArray(Length, seed: 128);

    [Benchmark]
    public int SetRunExpansion() => LongestConsecutiveSequenceSolution.LongestConsecutiveBySetRunExpansion(_nums);
}
