using DSAExperimentation.LeetCode.NextGreaterElementII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NextGreaterElementII;

// Harness only: both strategies live in NextGreaterElementIISolution and are
// asserted against the same examples - the O(n) monotonic-stack sweep (walking
// the circular array twice over this repo's own Stack<int>) and the O(n^2)
// scan-ahead baseline it has to justify itself against.
public sealed class NextGreaterElementIITests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [1, 2, 1], [2, -1, 2] },
            { [1, 1, 1], [-1, -1, -1] },
            { [5, 4, 3, 2, 1, 6], [6, 6, 6, 6, 6, -1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NextGreaterElementsByBruteForce_LeetCodeExamples_WrapsAroundTheArray(int[] nums, int[] expected) =>
        Assert.Equal(expected, NextGreaterElementIISolution.NextGreaterElementsByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NextGreaterElementsByMonotonicStack_LeetCodeExamples_WrapsAroundTheArray(int[] nums, int[] expected) =>
        Assert.Equal(expected, NextGreaterElementIISolution.NextGreaterElementsByMonotonicStack(nums));
}
