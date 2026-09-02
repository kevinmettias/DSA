using NextGreaterStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NextGreaterElementIV;

// LeetCode 2454. Next Greater Element IV: the classic two-monotonic-stack sweep
// over this repo's own Stack<int> - the same NextGreaterElementII precedent.
// waitingForFirst holds indices still waiting for their FIRST greater value;
// waitingForSecond holds indices that already found their first and are waiting
// for their SECOND. Popping an index out of waitingForFirst promotes it into
// waitingForSecond through a small "promoted" stack (rather than resolving it
// outright) so its relative order - and waitingForSecond's own decreasing
// invariant - survives the move, since a value can only resolve waitingForSecond's
// entries, never waitingForFirst's, on the same pass.
public sealed class NextGreaterElementIVTests
{
    [Fact]
    public void SecondGreaterElement_LeetCodeExample1_ReturnsExpectedSequence()
    {
        int[] nums = [2, 4, 0, 9, 6];

        var result = SecondGreaterElement(nums);

        Assert.Equal([9, 6, 6, -1, -1], result);
    }

    [Fact]
    public void SecondGreaterElement_LeetCodeExample2_ReturnsAllMinusOne()
    {
        int[] nums = [3, 3];

        var result = SecondGreaterElement(nums);

        Assert.Equal([-1, -1], result);
    }

    [Fact]
    public void SecondGreaterElement_StrictlyIncreasing_EachAnswerIsTwoStepsAhead()
    {
        // Every later value is greater, so index i's second greater is simply nums[i+2].
        int[] nums = [1, 2, 3, 4];

        var result = SecondGreaterElement(nums);

        Assert.Equal([3, 4, -1, -1], result);
    }

    private static int[] SecondGreaterElement(int[] nums)
    {
        var result = new int[nums.Length];
        Array.Fill(result, -1);
        var waitingForFirst = new NextGreaterStack();
        var waitingForSecond = new NextGreaterStack();
        var promoted = new NextGreaterStack();

        for (var i = 0; i < nums.Length; i++)
        {
            while (waitingForSecond.TryPeek(out var second) && nums[second] < nums[i])
            {
                waitingForSecond.TryPop(out _);
                result[second] = nums[i];
            }

            while (waitingForFirst.TryPeek(out var first) && nums[first] < nums[i])
            {
                waitingForFirst.TryPop(out _);
                promoted.Push(first);
            }

            while (promoted.TryPop(out var index))
            {
                waitingForSecond.Push(index);
            }

            waitingForFirst.Push(i);
        }

        return result;
    }
}
