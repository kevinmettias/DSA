using DSAExperimentation.LeetCode.Solutions.NextGreaterElementII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NextGreaterElementII;

// Harness only: both strategies live in NextGreaterElementIISolution and are
// asserted against the same cases - the O(n) monotonic-stack sweep (walking
// the circular array twice over this repo's own Stack<int>) and the O(n^2)
// scan-ahead baseline it has to justify itself against.
public sealed partial class NextGreaterElementIISolutionTests
{
    public static TheoryData<int[], int[]> Cases =>
        new()
        {
            { [1, 1, 1], [-1, -1, -1] },
            { [5, 4, 3, 2, 1, 6], [6, 6, 6, 6, 6, -1] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void NextGreaterElementsByBruteForce_Cases_WrapsAroundTheArray(int[] nums, int[] expected) =>
        Assert.Equal(expected, NextGreaterElementIISolution.NextGreaterElementsByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void NextGreaterElementsByMonotonicStack_Cases_WrapsAroundTheArray(int[] nums, int[] expected) =>
        Assert.Equal(expected, NextGreaterElementIISolution.NextGreaterElementsByMonotonicStack(nums));
}
