using DSAExperimentation.LeetCode.ClimbingStairs;
using DSAExperimentation.LeetCode.MergeIntervals;
using DSAExperimentation.LeetCode.TwoSum;
using DSAExperimentation.LeetCode.Tests.LeetCodeCatalog.Validation;

namespace DSAExperimentation.LeetCode.Tests.LeetCodeCatalog;

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
            LeetCodeCatalog.Load("two-sum"), TwoSumByBruteForce, new TwoSumAdapter());

        Assert.All(results, result => Assert.True(result.Passed, result.FailureReason));
    }

    [Fact]
    public void Validate_TwoSumByHashMap_AgainstCachedFixture_AllExampleCasesPass()
    {
        var results = LeetCodeSolutionValidator.Validate(
            LeetCodeCatalog.Load("two-sum"), TwoSumByHashMap, new TwoSumAdapter());

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
            MergeByBatchSortAndMerge,
            new MergeIntervalsAdapter());

        Assert.All(results, result => Assert.True(result.Passed, result.FailureReason));
    }

    [Fact]
    public void Validate_MergeIntervalsByIntervalSet_AgainstCachedFixture_AllExampleCasesPass()
    {
        var results = LeetCodeSolutionValidator.Validate(
            LeetCodeCatalog.Load("merge-intervals"),
            MergeByIntervalSet,
            new MergeIntervalsAdapter());

        Assert.All(results, result => Assert.True(result.Passed, result.FailureReason));
    }

    // Two Sum's strategies report the pair through out parameters, while LeetCode's
    // answer is the pair itself. Two Sum's own constraint guarantees exactly one
    // solution, so a strategy that finds none answers with an empty pair, which no
    // published expected output matches.
    private static int[] TwoSumByBruteForce((int[] Nums, int Target) input) =>
        TwoSumSolution.TryFindIndicesByBruteForce(input.Nums, input.Target, out var first, out var second)
            ? [first, second]
            : [];

    private static int[] TwoSumByHashMap((int[] Nums, int Target) input) =>
        TwoSumSolution.TryFindIndicesByHashMap(input.Nums, input.Target, out var first, out var second)
            ? [first, second]
            : [];

    // Merge Intervals' strategies speak in (start, end) pairs, LeetCode in two-element
    // rows, so each side is translated at the boundary and nothing in between.
    private static int[][] MergeByBatchSortAndMerge(int[][] intervals) =>
        AsRows(MergeIntervalsSolution.MergeByBatchSortAndMerge(AsPairs(intervals)));

    private static int[][] MergeByIntervalSet(int[][] intervals) =>
        AsRows(MergeIntervalsSolution.MergeByIntervalSet(AsPairs(intervals)));

    private static IEnumerable<(int Start, int End)> AsPairs(int[][] intervals) =>
        intervals.Select(interval => (interval[0], interval[1]));

    private static int[][] AsRows(List<(int Start, int End)> merged) =>
        [.. merged.Select(interval => new[] { interval.Start, interval.End })];
}
