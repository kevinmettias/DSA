using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaximumAndSumOfArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumAndSumOfArraySolution's, the same methods
// MaximumAndSumOfArraySolutionTests proves correct - the textbook unmemoized recursion over
// (which slot is being filled, which elements are already placed) against the same
// recursion routed through this repo's own Memoizer, keyed on that same tuple.
//
// nums always fills every slot to capacity (Length == 2 * SlotCount) to maximize both
// the per-slot branching factor (every remaining element is a candidate single or
// pairing) and how often different paths converge on the same (Slot, UsedMask) - the
// worst case for an unmemoized walk and the best case for memoization.
//
// Sizes are per arm. The unmemoized side's branching is quadratic in the *remaining*
// element count at every one of SlotCount levels, so it grows far faster than
// MaximumStudentsTakingExamBenchmarks' per-row linear-in-columns branching and stops at
// 3 slots; the memoized side's SlotCount * 4^SlotCount states run on to 6 slots - LC 2172
// allows 9, but each state also tries every remaining pair, so 6 keeps a call near a
// benchmark's budget. The two are compared at the slot counts both run.
public class MaximumAndSumOfArrayBenchmarks
{
    private const int MaxValueExclusive = 1 << 20;
    private const int RandomSeed = 2172; // LC problem number
    private const int ElementsPerSlot = 2;

    private Dictionary<int, int[]> _numsBySlotCount = [];

    public static IEnumerable<int> BruteForceSizes => [2, 3];

    public static IEnumerable<int> MemoizedSizes => [.. BruteForceSizes, 5, 6];

    // Every slot count any arm runs is drawn here, outside the timed region, each from its
    // own generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _numsBySlotCount = MemoizedSizes.ToDictionary(
            slotCount => slotCount,
            slotCount => SeededDraws.Values(slotCount * ElementsPerSlot, 1, MaxValueExclusive, new Random(RandomSeed)));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public int BruteForceRecursion(int slotCount) =>
        MaximumAndSumOfArraySolution.MaximumAndSumByBruteForceRecursion(_numsBySlotCount[slotCount], slotCount);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public int MemoizedRecursion(int slotCount) =>
        MaximumAndSumOfArraySolution.MaximumAndSumByMemoizedBitmask(_numsBySlotCount[slotCount], slotCount);
}
