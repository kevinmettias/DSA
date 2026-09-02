using RepoLongStack = DSAExperimentation.DataStructures.Stack.Stack<long>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReplaceNonCoprimeNumbersInArray;

// LeetCode 2197. Replace Non-Coprime Numbers in Array: this repo's own Stack<long>
// (DailyTemperatures/NextGreaterElement precedent) holding the merged prefix so far
// - each new number keeps merging into the top of the stack via LCM while it shares
// a factor with it, which correctly cascades merges backward exactly as far as
// needed, since the stack's top is always the most recently finalized element.
public sealed partial class ReplaceNonCoprimeNumbersInArrayTests
{
    [Fact]
    public void ReplaceNonCoprimes_ClassicExample_MergesCascadingFactors()
    {
        int[] nums = [6, 4, 3, 2, 7, 6, 2];

        var result = ReplaceNonCoprimes(nums);

        Assert.Equal([12, 7, 6], result);
    }

    [Fact]
    public void ReplaceNonCoprimes_RepeatedFactorsOfOne_LeavesOnesUnmerged()
    {
        int[] nums = [2, 2, 1, 1, 3, 3, 3];

        var result = ReplaceNonCoprimes(nums);

        Assert.Equal([2, 1, 1, 3], result);
    }

    private static int[] ReplaceNonCoprimes(int[] nums)
    {
        var stack = new RepoLongStack();

        foreach (var num in nums)
        {
            long current = num;

            while (stack.TryPeek(out var top) && Gcd(top, current) > 1)
            {
                stack.TryPop(out _);
                current = current / Gcd(top, current) * top;
            }

            stack.Push(current);
        }

        return DrainToArray(stack);
    }

    private static int[] DrainToArray(RepoLongStack stack)
    {
        var values = new List<long>();

        while (stack.TryPop(out var value))
        {
            values.Add(value);
        }

        values.Reverse();
        return values.Select(v => (int)v).ToArray();
    }

    private static long Gcd(long a, long b) => b == 0 ? a : Gcd(b, a % b);
}
