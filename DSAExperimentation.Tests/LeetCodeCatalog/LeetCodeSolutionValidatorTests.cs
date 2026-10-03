using DSAExperimentation.LeetCode.ClimbingStairs;
using DSAExperimentation.LeetCode.MergeIntervals;
using DSAExperimentation.LeetCode.TwoSum;
using DSAExperimentation.Tests.LeetCodeCatalog.Validation;

namespace DSAExperimentation.Tests.LeetCodeCatalog;

// End-to-end proof of the whole catalog: load a real, fetched-from-leetcode.com
// snapshot from the offline cache, inject each strategy of the problem's own
// solution class through ILeetCodeTestCaseAdapter, and validate it against every
// example test case LeetCode itself publishes for that question. The strategies
// are the tier 4 ones the per-problem tests assert, never a copy written here, so
// a pass says the code the repo ships agrees with leetcode.com's own examples.
public sealed partial class LeetCodeSolutionValidatorTests
{
    [Fact]
    public void Validate_TwoSumByBruteForce_AgainstCachedFixture_AllExampleCasesPass()
    {
        var results = LeetCodeSolutionValidator.Validate(
            LeetCodeCatalog.Load("two-sum"), TwoSumIndices(TwoSumSolution.TryFindIndicesByBruteForce), new TwoSumAdapter());

        Assert.All(results, result => Assert.True(result.Passed, result.FailureReason));
    }

    [Fact]
    public void Validate_TwoSumByHashMap_AgainstCachedFixture_AllExampleCasesPass()
    {
        var results = LeetCodeSolutionValidator.Validate(
            LeetCodeCatalog.Load("two-sum"), TwoSumIndices(TwoSumSolution.TryFindIndicesByHashMap), new TwoSumAdapter());

        Assert.All(results, result => Assert.True(result.Passed, result.FailureReason));
    }

    [Fact]
    public void Validate_ClimbingStairsByIterativeRollingTotals_AgainstCachedFixture_AllExampleCasesPass()
    {
        var results = LeetCodeSolutionValidator.Validate(
            LeetCodeCatalog.Load("climbing-stairs"),
            ClimbingStairsSolution.CountWaysByIterativeRollingTotals,
            new ClimbingStairsAdapter());

        Assert.All(results, result => Assert.True(result.Passed, result.FailureReason));
    }

    [Fact]
    public void Validate_ClimbingStairsByMemoizedRecurrence_AgainstCachedFixture_AllExampleCasesPass()
    {
        var results = LeetCodeSolutionValidator.Validate(
            LeetCodeCatalog.Load("climbing-stairs"),
            ClimbingStairsSolution.CountWaysByMemoizedRecurrence,
            new ClimbingStairsAdapter());

        Assert.All(results, result => Assert.True(result.Passed, result.FailureReason));
    }

    [Fact]
    public void Validate_MergeIntervalsByBatchSortAndMerge_AgainstCachedFixture_AllExampleCasesPass()
    {
        var results = LeetCodeSolutionValidator.Validate(
            LeetCodeCatalog.Load("merge-intervals"),
            JaggedIntervals(MergeIntervalsSolution.MergeByBatchSortAndMerge),
            new MergeIntervalsAdapter());

        Assert.All(results, result => Assert.True(result.Passed, result.FailureReason));
    }

    [Fact]
    public void Validate_MergeIntervalsByIntervalSet_AgainstCachedFixture_AllExampleCasesPass()
    {
        var results = LeetCodeSolutionValidator.Validate(
            LeetCodeCatalog.Load("merge-intervals"),
            JaggedIntervals(MergeIntervalsSolution.MergeByIntervalSet),
            new MergeIntervalsAdapter());

        Assert.All(results, result => Assert.True(result.Passed, result.FailureReason));
    }

    // Two Sum's strategies report the pair through out parameters, while LeetCode's
    // answer is the pair itself. Two Sum's own constraint guarantees exactly one
    // solution, so a strategy that finds none answers with an empty pair, which no
    // published expected output matches.
    private static Func<(int[] Nums, int Target), int[]> TwoSumIndices(TwoSumStrategy strategy) =>
        input => strategy(input.Nums, input.Target, out var first, out var second) ? [first, second] : [];

    // Merge Intervals' strategies speak in (start, end) pairs, LeetCode in two-element
    // rows, so each side is translated at the boundary and nothing in between.
    private static Func<int[][], int[][]> JaggedIntervals(
        Func<IEnumerable<(int Start, int End)>, List<(int Start, int End)>> strategy) =>
        intervals =>
        [
            .. strategy(intervals.Select(interval => (interval[0], interval[1])))
                .Select(merged => new[] { merged.Start, merged.End }),
        ];

    private delegate bool TwoSumStrategy(int[] nums, int target, out int first, out int second);
}
