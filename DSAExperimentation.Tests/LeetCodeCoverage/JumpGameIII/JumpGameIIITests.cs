using DSAExperimentation.LeetCode.JumpGameIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGameIII;

// Harness only. Both reachability strategies are JumpGameIIISolution's - the
// hand-rolled stack walk that used to live untested as the benchmark baseline, and
// the DepthFirstSearch.Traverse composition - pinned here to LeetCode's published
// examples plus the two boundary cases neither harness used to cover: a start that
// is already on a zero, and a zero that exists but sits outside the reachable
// component.
public sealed class JumpGameIIITests
{
    public static TheoryData<int[], int, bool> Examples =>
        new()
        {
            { [4, 2, 3, 0, 3, 1, 2], 5, true },
            { [4, 2, 3, 0, 3, 1, 2], 0, true },
            { [4, 2, 3, 0, 3, 1, 2], 6, true },
            { [3, 0, 2, 1, 2], 2, false },
            { [0], 0, true },
            { [1, 2, 0], 0, false },
            { [1, 1, 1, 1, 1], 2, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanReachByStackWalk_LeetCodeExamples_ReturnsWhetherAZeroIsReachable(
        int[] arr, int start, bool expected) =>
        Assert.Equal(expected, JumpGameIIISolution.CanReachByStackWalk(arr, start));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanReachByDepthFirstSearch_LeetCodeExamples_ReturnsWhetherAZeroIsReachable(
        int[] arr, int start, bool expected) =>
        Assert.Equal(expected, JumpGameIIISolution.CanReachByDepthFirstSearch(arr, start));
}
