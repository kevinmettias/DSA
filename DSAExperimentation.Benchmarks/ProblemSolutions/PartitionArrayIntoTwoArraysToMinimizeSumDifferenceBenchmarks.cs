using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PartitionArrayIntoTwoArraysToMinimizeSumDifference;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// PartitionArrayIntoTwoArraysToMinimizeSumDifferenceSolution's, the same methods
// PartitionArrayIntoTwoArraysToMinimizeSumDifferenceTests proves correct - a
// direct O(2^(2n)) scan of every size-n bitmask over the whole array against
// meet-in-the-middle over each half's subset sums grouped by subset size.
[MemoryDiagnoser]
public class PartitionArrayIntoTwoArraysToMinimizeSumDifferenceBenchmarks
{
    // LC problem number, reused as the deterministic random seed.
    private const int RandomSeed = 2035;

    // Symmetric bound for the generated values' range: [-ValueBound, ValueBound).
    private const int ValueBound = 50;

    [Params(16, 20)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueBound, ValueBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceAllEqualSplits() =>
        PartitionArrayIntoTwoArraysToMinimizeSumDifferenceSolution
            .MinimumDifferenceByBruteForceEqualSplits(_nums);

    [Benchmark]
    public int MeetInTheMiddleGroupedBySize() =>
        PartitionArrayIntoTwoArraysToMinimizeSumDifferenceSolution
            .MinimumDifferenceByMeetInTheMiddle(_nums);
}
