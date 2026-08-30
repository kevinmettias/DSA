using NextGreaterStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NextGreaterElementII;

// LeetCode 503. Next Greater Element II: the classic monotonic-stack sweep over
// this repo's own Stack<int> - the same OneThreeTwoPatternTests/
// LargestRectangleInHistogramTests precedent - holding indices whose next-greater
// element hasn't been found yet, walked twice around the array (i % n) so every
// index gets a chance to see a greater value that wraps around from the front.
public sealed partial class NextGreaterElementIITests
{
    [Fact]
    public void NextGreaterElements_ClassicExample_WrapsAroundTheArray()
    {
        int[] nums = [1, 2, 1];

        var result = NextGreaterElements(nums);

        Assert.Equal([2, -1, 2], result);
    }

    [Fact]
    public void NextGreaterElements_AllEqualValues_ReturnsAllMinusOne()
    {
        int[] nums = [1, 1, 1];

        var result = NextGreaterElements(nums);

        Assert.Equal([-1, -1, -1], result);
    }

    [Fact]
    public void NextGreaterElements_DescendingThenPeak_FindsWrapAroundGreater()
    {
        int[] nums = [5, 4, 3, 2, 1, 6];

        var result = NextGreaterElements(nums);

        Assert.Equal([6, 6, 6, 6, 6, -1], result);
    }

    private static int[] NextGreaterElements(int[] nums)
    {
        var n = nums.Length;
        var result = new int[n];
        Array.Fill(result, -1);
        var pendingIndices = new NextGreaterStack();

        for (var i = 0; i < 2 * n; i++)
        {
            var value = nums[i % n];

            while (pendingIndices.TryPeek(out var top) && nums[top] < value)
            {
                pendingIndices.TryPop(out _);
                result[top] = value;
            }

            if (i < n)
            {
                pendingIndices.Push(i);
            }
        }

        return result;
    }
}
