using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.IntervalSet;
using DSAExperimentation.Tests.LeetCodeCatalog.Validation;

namespace DSAExperimentation.Tests.LeetCodeCatalog;

// End-to-end proof of the whole catalog: load a real, fetched-from-leetcode.com
// snapshot from the offline cache, inject a solution built from this repo's own
// primitives through ILeetCodeTestCaseAdapter, and validate it against every
// example test case LeetCode itself publishes for that question.
public sealed partial class LeetCodeSolutionValidatorTests
{
    [Fact]
    public void Validate_TwoSum_AgainstCachedFixture_AllExampleCasesPass()
    {
        var question = LeetCodeCatalog.Load("two-sum");

        var results = LeetCodeSolutionValidator.Validate(question, SolveTwoSum, new TwoSumAdapter());

        Assert.All(results, result => Assert.True(result.Passed, result.FailureReason));
    }

    private static int[] SolveTwoSum((int[] Nums, int Target) input)
    {
        var seen = new HashMap<int, int>();

        for (var i = 0; i < input.Nums.Length; i++)
        {
            if (seen.TryGetValue(input.Target - input.Nums[i], out var matchIndex))
            {
                return [matchIndex, i];
            }

            seen.Set(input.Nums[i], i);
        }

        throw new InvalidOperationException("Two Sum's own constraint guarantees exactly one solution, so this is unreachable.");
    }

    [Fact]
    public void Validate_ClimbingStairs_AgainstCachedFixture_AllExampleCasesPass()
    {
        var question = LeetCodeCatalog.Load("climbing-stairs");

        var results = LeetCodeSolutionValidator.Validate(question, SolveClimbingStairs, new ClimbingStairsAdapter());

        Assert.All(results, result => Assert.True(result.Passed, result.FailureReason));
    }

    private static int SolveClimbingStairs(int stepCount)
        => Memoizer.Memoize<int, int>(stepCount, new WaysFromPreviousTwoSteps());

    [Fact]
    public void Validate_MergeIntervals_AgainstCachedFixture_AllExampleCasesPass()
    {
        var question = LeetCodeCatalog.Load("merge-intervals");

        var results = LeetCodeSolutionValidator.Validate(question, SolveMergeIntervals, new MergeIntervalsAdapter());

        Assert.All(results, result => Assert.True(result.Passed, result.FailureReason));
    }

    private static int[][] SolveMergeIntervals(int[][] intervals)
    {
        var mergedIntervals = new IntervalSet<int>();

        foreach (var interval in intervals)
        {
            mergedIntervals.Add(interval[0], interval[1]);
        }

        return ToJaggedArray(mergedIntervals);
    }

    private static int[][] ToJaggedArray(IntervalSet<int> intervals)
    {
        var result = new int[intervals.Count][];

        for (var i = 0; i < intervals.Count; i++)
        {
            var (start, end) = intervals.Get(i);
            result[i] = [start, end];
        }

        return result;
    }

    // The Climbing Stairs recurrence, named: one step plus two steps back, with the
    // two base cases being the whole of the rule. The memo run passes this
    // implementation back to itself, so no delegate is handed around.
    private sealed class WaysFromPreviousTwoSteps : IRecurrence<int, int>
    {
        public int Replay(int state, IRecurrence<int, int> rest)
        {
            if (state <= 1)
            {
                return 1;
            }

            return rest.Replay(state - 1, rest) + rest.Replay(state - 2, rest);
        }
    }
}
