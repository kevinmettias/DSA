using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ConcatenatedDivisibility;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ConcatenatedDivisibilitySolution's, the same
// methods ConcatenatedDivisibilityTests proves correct. No hoisted overload is
// needed - nums/k are already the cheap, plain-array shape [GlobalSetup] would
// produce either way, the same reasoning MedianOfTwoSortedArraysBenchmarks
// applies to its own nums1/nums2.
//
// NumberCount stays small - backtracking is O(n!) in the worst case - so both
// arms still finish in reasonable time; this is exactly the range where the
// bitmask-DP arm's polynomial cost should start pulling away from the baseline's
// factorial one.
[MemoryDiagnoser]
public class ConcatenatedDivisibilityBenchmarks
{
    // LC problem number, reused as the deterministic input seed.
    private const int Seed = 3533;

    private int[] _nums = [];

    private int _k;
    [Params(6, 9)]
    public int NumberCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed + NumberCount);
        _nums = [.. Enumerable.Range(0, NumberCount).Select(_ => random.Next(1, 100_000))];
        _k = random.Next(1, 101);
    }

    [Benchmark(Baseline = true)]
    public IList<int> Backtracking() => ConcatenatedDivisibilitySolution.SmallestPermutationByBacktracking(_nums, _k);

    [Benchmark]
    public IList<int> BitmaskMemo() => ConcatenatedDivisibilitySolution.SmallestPermutationByBitmaskMemo(_nums, _k);
}
