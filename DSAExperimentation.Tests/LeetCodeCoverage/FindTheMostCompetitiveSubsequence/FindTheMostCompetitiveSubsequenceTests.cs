using CompetitiveStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheMostCompetitiveSubsequence;

// LeetCode 1673. Find the Most Competitive Subsequence: the same monotonic-stack
// greedy RemoveKDigits already applies, over this repo's own Stack<int> - pop any
// still-poppable, strictly-greater element as long as enough elements remain
// afterward to still reach length k, then push the current element only while
// there's still room left for it.
public sealed partial class FindTheMostCompetitiveSubsequenceTests
{
    [Theory]
    [InlineData(new[] { 3, 5, 2, 6 }, 2, new[] { 2, 6 })]
    [InlineData(new[] { 2, 4, 3, 3, 5, 4, 9, 6 }, 4, new[] { 2, 3, 3, 4 })]
    public void MostCompetitive_LeetCodeExamples_ReturnsLexicographicallySmallestSubsequence(int[] nums, int k, int[] expected)
    {
        var mostCompetitive = MostCompetitive(nums, k);
        Assert.Equal(expected, mostCompetitive);
    }

    private static int[] MostCompetitive(int[] nums, int k)
    {
        var stack = new CompetitiveStack();

        BuildCompetitiveStack(stack, nums, k);

        return DrainStackToArray(stack);
    }

    private static void BuildCompetitiveStack(CompetitiveStack stack, int[] nums, int k)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            while (stack.Count > 0 && stack.TryPeek(out var top) && top > nums[i]
                   && stack.Count - 1 + (nums.Length - i) >= k)
            {
                stack.TryPop(out _);
            }

            if (stack.Count < k)
            {
                stack.Push(nums[i]);
            }
        }
    }

    private static int[] DrainStackToArray(CompetitiveStack stack)
    {
        var result = new int[stack.Count];
        for (var i = result.Length - 1; i >= 0; i--)
        {
            stack.TryPop(out result[i]);
        }

        return result;
    }
}
