using DSAExperimentation.LeetCode.Solutions.JumpGameIII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.JumpGameIII;

// Harness only. Both reachability strategies are JumpGameIIISolution's - the
// hand-rolled stack walk that used to live untested as the benchmark baseline, and
// the DepthFirstSearch.Traverse composition - pinned here to a zero reached from the
// last index, the two boundary cases neither harness used to cover (a start that is
// already on a zero, and a zero that exists but sits outside the reachable
// component), and an array with no zero at all. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class JumpGameIIISolutionTests
{
    public static TheoryData<ZeroReachCase> Cases =>
        new()
        {
            { new ZeroReachCase(Arr: [4, 2, 3, 0, 3, 1, 2], Start: 6, Expected: true) },
            { new ZeroReachCase(Arr: [0], Start: 0, Expected: true) },
            { new ZeroReachCase(Arr: [1, 2, 0], Start: 0, Expected: false) },
            { new ZeroReachCase(Arr: [1, 1, 1, 1, 1], Start: 2, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanReachByStackWalk_Cases_ReturnsWhetherAZeroIsReachable(ZeroReachCase testCase)
    {
        var actual = JumpGameIIISolution.CanReachByStackWalk(testCase.Arr, testCase.Start);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanReachByDepthFirstSearch_Cases_ReturnsWhetherAZeroIsReachable(
        ZeroReachCase testCase)
    {
        var actual = JumpGameIIISolution.CanReachByDepthFirstSearch(testCase.Arr, testCase.Start);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the jump array, the index to start from, and whether a zero
    // is reachable. The `bool` is the expected answer rather than a mode, so the row
    // names it instead of leaving a bare `true` in a position the reader has to decode.
    public readonly record struct ZeroReachCase(int[] Arr, int Start, bool Expected);
}
