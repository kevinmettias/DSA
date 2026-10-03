using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindSumOfArrayProductOfMagicalSequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindSumOfArrayProductOfMagicalSequencesSolution's,
// the same methods FindSumOfArrayProductOfMagicalSequencesSolutionTests proves correct.
//
// nums.Length is fixed at 8 and only SlotCount (the sequence length) grows: the
// backtracking arm is O(nums.Length^m), so even a modest jump in the slot count
// already makes the branching-factor-8 tree considerably larger, in contrast with the
// carry-digit DP arm, whose memoized state space grows only polynomially in the slot
// count.
//
// Sizes are per arm. The backtracking arm stops at 6 slots; the carry-digit DP runs on
// to LC 3539's own bound of 30, and the two are compared at the slot counts both run.
public class FindSumOfArrayProductOfMagicalSequencesBenchmarks
{
    private const int IndexCount = 8;
    private const int Seed = 3539;

    private int[] _nums = [];

    private Dictionary<int, int> _requiredSetBitsBySlotCount = [];

    public static IEnumerable<int> BacktrackSizes => [4, 6];

    public static IEnumerable<int> CarryDigitDpSizes => [.. BacktrackSizes, 15, 30];

    // nums is the same for every slot count; the set-bit target is derived for every slot
    // count any arm runs, outside the timed region, and an arm looks its own up.
    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = SeededDraws.Values(IndexCount, 1, 100_000_000, random);
        _requiredSetBitsBySlotCount = CarryDigitDpSizes.ToDictionary(
            slotCount => slotCount,
            slotCount => Math.Max(1, slotCount / 2));
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BacktrackSizes))]
    public int BacktrackEnumeration(int slotCount) =>
        FindSumOfArrayProductOfMagicalSequencesSolution.SumOfProductsByBacktrackEnumeration(
            slotCount, _requiredSetBitsBySlotCount[slotCount], _nums);

    [Benchmark]
    [ArgumentsSource(nameof(CarryDigitDpSizes))]
    public int CarryDigitDp(int slotCount) =>
        FindSumOfArrayProductOfMagicalSequencesSolution.SumOfProductsByCarryDigitDp(
            slotCount, _requiredSetBitsBySlotCount[slotCount], _nums);
}
