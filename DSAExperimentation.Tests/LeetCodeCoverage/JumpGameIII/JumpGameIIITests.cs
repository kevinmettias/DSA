using DSAExperimentation.LeetCode.JumpGameIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGameIII;

// Harness only. Both reachability strategies are JumpGameIIISolution's - the
// hand-rolled stack walk that used to live untested as the benchmark baseline, and
// the DepthFirstSearch.Traverse composition - pinned here to LeetCode's published
// examples plus the two boundary cases neither harness used to cover: a start that
// is already on a zero, and a zero that exists but sits outside the reachable
// component.
public sealed partial class JumpGameIIITests
{
    public static TheoryData<ZeroReachExample> Examples =>
        new()
        {
            { new ZeroReachExample(Arr: [4, 2, 3, 0, 3, 1, 2], Start: 5, Expected: true) },
            { new ZeroReachExample(Arr: [4, 2, 3, 0, 3, 1, 2], Start: 0, Expected: true) },
            { new ZeroReachExample(Arr: [4, 2, 3, 0, 3, 1, 2], Start: 6, Expected: true) },
            { new ZeroReachExample(Arr: [3, 0, 2, 1, 2], Start: 2, Expected: false) },
            { new ZeroReachExample(Arr: [0], Start: 0, Expected: true) },
            { new ZeroReachExample(Arr: [1, 2, 0], Start: 0, Expected: false) },
            { new ZeroReachExample(Arr: [1, 1, 1, 1, 1], Start: 2, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanReachByStackWalk_LeetCodeExamples_ReturnsWhetherAZeroIsReachable(ZeroReachExample example)
    {
        var actual = JumpGameIIISolution.CanReachByStackWalk(example.Arr, example.Start);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanReachByDepthFirstSearch_LeetCodeExamples_ReturnsWhetherAZeroIsReachable(
        ZeroReachExample example)
    {
        var actual = JumpGameIIISolution.CanReachByDepthFirstSearch(example.Arr, example.Start);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the jump array, the index to start from, and whether a zero
    // is reachable. The `bool` is the expected answer rather than a mode, so the row
    // names it instead of leaving a bare `true` in a position the reader has to decode.
    public readonly record struct ZeroReachExample(int[] Arr, int Start, bool Expected);
}
