using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<(int Value, int Step)>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StepsToMakeArrayNonDecreasing;

// LeetCode 2289. Steps to Make Array Non-decreasing: each step simultaneously
// removes every element with a strictly greater element somewhere to its left;
// find how many steps until the array is non-decreasing. A monotonic
// (non-increasing) stack tracks, for each element, the step at which it gets
// removed - the answer is the max removal step seen - using this repo's own
// Stack<T> as the scratch structure (DailyTemperaturesTests' monotonic-stack
// precedent, generalized from "wait days" to "removal step").
public sealed partial class StepsToMakeArrayNonDecreasingTests
{
    [Fact]
    public void TotalSteps_LeetCodeExample_ReturnsThree()
    {
        int[] nums = [5, 3, 4, 4, 7, 3, 6, 11, 8, 5, 11];

        var steps = TotalSteps(nums);

        Assert.Equal(3, steps);
    }

    [Fact]
    public void TotalSteps_AlreadyNonDecreasing_ReturnsZero()
    {
        int[] nums = [4, 5, 7, 7, 13];

        var steps = TotalSteps(nums);

        Assert.Equal(0, steps);
    }

    [Fact]
    public void TotalSteps_StrictlyDecreasing_ReturnsOne()
    {
        int[] nums = [9, 7, 5, 3, 1];

        var steps = TotalSteps(nums);

        Assert.Equal(1, steps);
    }

    private static int TotalSteps(int[] nums)
    {
        var stack = new RepoStack();
        var maxSteps = 0;

        foreach (var value in nums)
        {
            var step = 0;

            while (stack.TryPeek(out var top) && top.Value <= value)
            {
                step = Math.Max(step, top.Step);
                stack.TryPop(out _);
            }

            step = stack.Count == 0 ? 0 : step + 1;
            maxSteps = Math.Max(maxSteps, step);
            stack.Push((value, step));
        }

        return maxSteps;
    }
}
