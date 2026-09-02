using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfVisiblePeopleInAQueue;

// LeetCode 1944. Number of Visible People in a Queue: a monotonic non-increasing
// Stack<int> of heights swept right to left (DailyTemperatures/NextGreaterElementI
// precedent for this repo's own Stack) - every shorter person still on top gets
// popped and counted as visible, one more visible person is counted for the
// taller-or-equal blocker left on top afterward (if any), and any heights tied with
// the current person are popped before pushing it so a later, even taller person
// can never see past more than one of a run of equal heights.
public sealed partial class NumberOfVisiblePeopleInAQueueTests
{
    [Fact]
    public void CountVisiblePeople_ClassicExample_ReturnsVisibleCountPerPerson()
    {
        int[] heights = [10, 6, 8, 5, 11, 9];

        var result = CountVisiblePeople(heights);

        Assert.Equal([3, 1, 2, 1, 1, 0], result);
    }

    [Fact]
    public void CountVisiblePeople_StrictlyIncreasing_EveryoneSeesOnlyTheNextPerson()
    {
        int[] heights = [1, 2, 3, 4, 5];

        var result = CountVisiblePeople(heights);

        Assert.Equal([1, 1, 1, 1, 0], result);
    }

    [Fact]
    public void CountVisiblePeople_AllEqualHeights_EveryoneSeesOnlyTheNextPerson()
    {
        int[] heights = [5, 5, 5, 5];

        var result = CountVisiblePeople(heights);

        Assert.Equal([1, 1, 1, 0], result);
    }

    private static int[] CountVisiblePeople(int[] heights)
    {
        var n = heights.Length;
        var result = new int[n];
        var stack = new RepoIntStack();

        for (var i = n - 1; i >= 0; i--)
        {
            result[i] = CountVisibleFrom(heights[i], stack);
        }

        return result;
    }

    private static int CountVisibleFrom(int height, RepoIntStack stack)
    {
        var count = 0;

        while (stack.TryPeek(out var top) && top < height)
        {
            stack.TryPop(out _);
            count++;
        }

        if (stack.Count > 0)
        {
            count++;
        }

        while (stack.TryPeek(out var top) && top == height)
        {
            stack.TryPop(out _);
        }

        stack.Push(height);
        return count;
    }
}
