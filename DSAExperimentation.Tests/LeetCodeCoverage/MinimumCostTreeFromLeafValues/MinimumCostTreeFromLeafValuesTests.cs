using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostTreeFromLeafValues;

// LeetCode 1130. Minimum Cost Tree From Leaf Values: rather than the O(n^3)
// interval-DP formulation (dp[i][j] = min over every split k of dp[i][k] +
// dp[k+1][j] + max(arr[i..k]) * max(arr[k+1..j])), the optimal tree is built
// greedily with a single monotonic-decreasing pass over this repo's own
// Stack<int> (the same SumOfSubarrayMinimums precedent): whenever a leaf sits
// between two larger-or-equal neighbors it can only ever be optimal to combine
// it with its SMALLER neighbor first, so popping it as soon as the next leaf
// is >= it, and multiplying by min(newTop, current), reconstructs the optimal
// tree in one O(n) pass - a sentinel of int.MaxValue at the bottom of the
// stack stands in for "no neighbor on this side yet."
public sealed partial class MinimumCostTreeFromLeafValuesTests
{
    [Fact]
    public void MctFromLeafValues_LeetCodeExampleOne_ReturnsThirtyTwo()
        => Assert.Equal(32, MctFromLeafValues([6, 2, 4]));

    [Fact]
    public void MctFromLeafValues_LeetCodeExampleTwo_ReturnsFortyFour()
        => Assert.Equal(44, MctFromLeafValues([4, 11]));

    [Fact]
    public void MctFromLeafValues_AllEqualLeaves_CombinesPairwise()
        => Assert.Equal(2, MctFromLeafValues([1, 1, 1]));

    private static int MctFromLeafValues(int[] arr)
    {
        var stack = new RepoIntStack();
        stack.Push(int.MaxValue);
        long total = 0;

        foreach (var value in arr)
        {
            total += PushValue(stack, value);
        }

        total += DrainRemaining(stack);

        return (int)total;
    }

    private static long PushValue(RepoIntStack stack, int value)
    {
        long cost = 0;

        while (stack.TryPeek(out var top) && top <= value)
        {
            stack.TryPop(out var mid);
            stack.TryPeek(out var next);
            cost += (long)mid * Math.Min(next, value);
        }

        stack.Push(value);
        return cost;
    }

    private static long DrainRemaining(RepoIntStack stack)
    {
        long cost = 0;

        while (stack.Count > 2)
        {
            stack.TryPop(out var mid);
            stack.TryPeek(out var next);
            cost += (long)mid * next;
        }

        return cost;
    }
}
