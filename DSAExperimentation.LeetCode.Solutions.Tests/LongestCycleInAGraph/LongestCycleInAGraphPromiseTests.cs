using DSAExperimentation.LeetCode.Solutions.LongestCycleInAGraph;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestCycleInAGraph;

public sealed partial class LongestCycleInAGraphPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [3, 3, 4, 2, 3], true },
            { [2, -1, 3, 1], true },
            { [1, 1, 1, 1, 1], false },
            { [1, 5], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenNoEdgeLoopsOrLeavesTheGraph(int[] edges, bool expected)
        => Assert.Equal(expected, LongestCycleInAGraphPromise.IsKeptBy(edges));
}
