using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumAbsoluteDifferenceBetweenElementsWithConstraint;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MinimumAbsoluteDifferenceBetweenElementsWithConstraintSolution's, the same methods
// MinimumAbsoluteDifferenceBetweenElementsWithConstraintTests proves correct - the
// O(n^2) scan of every admissible pair against the O(n log n) BinarySearchTree<int>
// sliding window queried by FindClosest.TryFind. Values are drawn from a range wide
// enough that no early pair collapses the answer to zero, so both arms do their full
// work; the index gap is a quarter of the array, leaving the eligible prefix large
// enough for the tree to matter. int[] plus an int gap is already LeetCode's own
// input shape, so [GlobalSetup] hands it straight in and no hoisted overload is
// needed.
[MemoryDiagnoser]
public class MinimumAbsoluteDifferenceBetweenElementsWithConstraintBenchmarks
{
    private const int RandomSeed = 2817; // LeetCode problem number
    private const int MaxValueExclusive = 1_000_000;

    private int[] _nums = [];

    private int _minimumIndexDistance;
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _minimumIndexDistance = Length / 4;
    }

    [Benchmark(Baseline = true)]
    public int BruteForcePairScan() =>
        MinimumAbsoluteDifferenceBetweenElementsWithConstraintSolution
            .MinAbsoluteDifferenceByBruteForcePairScan(_nums, _minimumIndexDistance);

    [Benchmark]
    public int BstSlidingWindow() =>
        MinimumAbsoluteDifferenceBetweenElementsWithConstraintSolution
            .MinAbsoluteDifferenceByBstSlidingWindow(_nums, _minimumIndexDistance);
}
