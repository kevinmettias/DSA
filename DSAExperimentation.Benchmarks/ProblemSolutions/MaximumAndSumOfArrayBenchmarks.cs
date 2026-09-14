using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumAndSumOfArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumAndSumOfArraySolution's, the same methods
// MaximumAndSumOfArrayTests proves correct - the textbook unmemoized recursion over
// (which slot is being filled, which elements are already placed) against the same
// recursion routed through this repo's own Memoizer, keyed on that same tuple.
//
// nums always fills every slot to capacity (Length == 2 * NumSlots) to maximize both
// the per-slot branching factor (every remaining element is a candidate single or
// pairing) and how often different paths converge on the same (Slot, UsedMask) - the
// worst case for an unmemoized walk and the best case for memoization. NumSlots stays
// small: the unmemoized side's branching is quadratic in the *remaining* element count
// at every one of NumSlots levels, so it grows far faster than
// MaximumStudentsTakingExamBenchmarks' per-row linear-in-columns branching.
[MemoryDiagnoser]
public class MaximumAndSumOfArrayBenchmarks
{
    private const int MaxValueExclusive = 1 << 20;
    private const int RandomSeed = 2172; // LC problem number
    private const int ElementsPerSlot = 2;

    [Params(2, 3)]
    public int NumSlots;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, NumSlots * ElementsPerSlot).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRecursion() =>
        MaximumAndSumOfArraySolution.MaximumAndSumByBruteForceRecursion(_nums, NumSlots);

    [Benchmark]
    public int MemoizedRecursion() =>
        MaximumAndSumOfArraySolution.MaximumAndSumByMemoizedBitmask(_nums, NumSlots);
}
