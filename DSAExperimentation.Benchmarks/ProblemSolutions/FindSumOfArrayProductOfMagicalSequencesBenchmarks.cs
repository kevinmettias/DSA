using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindSumOfArrayProductOfMagicalSequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindSumOfArrayProductOfMagicalSequencesSolution's,
// the same methods FindSumOfArrayProductOfMagicalSequencesTests proves correct.
//
// nums.Length is fixed at 8 and only SlotCount (m) grows: the backtracking arm is
// O(nums.Length^m), so even a modest jump in m already makes the branching-factor-8
// tree considerably larger while staying fast enough to benchmark, in contrast with
// the carry-digit DP arm, whose memoized state space grows only linearly in m.
[MemoryDiagnoser]
public class FindSumOfArrayProductOfMagicalSequencesBenchmarks
{
    private const int IndexCount = 8;
    private const int Seed = 3539;

    private int[] _nums = [];

    private int _k;
    [Params(4, 6)]
    public int SlotCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, IndexCount).Select(_ => random.Next(1, 100_000_000)).ToArray();
        _k = Math.Max(1, SlotCount / 2);
    }

    [Benchmark(Baseline = true)]
    public int BacktrackEnumeration() =>
        FindSumOfArrayProductOfMagicalSequencesSolution.SumOfProductsByBacktrackEnumeration(SlotCount, _k, _nums);

    [Benchmark]
    public int CarryDigitDp() =>
        FindSumOfArrayProductOfMagicalSequencesSolution.SumOfProductsByCarryDigitDp(SlotCount, _k, _nums);
}
