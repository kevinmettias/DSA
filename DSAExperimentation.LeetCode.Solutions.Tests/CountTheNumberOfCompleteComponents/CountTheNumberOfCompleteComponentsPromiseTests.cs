using DSAExperimentation.LeetCode.Solutions.CountTheNumberOfCompleteComponents;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountTheNumberOfCompleteComponents;

public sealed partial class CountTheNumberOfCompleteComponentsPromiseTests
{
    public static TheoryData<int, int[][], bool> Inputs =>
        new()
        {
            { 6, [[0, 1], [0, 2], [1, 2], [3, 4]], true },
            { 5, [[3, 4], [4, 3]], false },
            { 3, [[0, 1], [2, 2]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenNoEdgeLoopsOrRepeats(int nodeCount, int[][] edges, bool expected)
        => Assert.Equal(expected, CountTheNumberOfCompleteComponentsPromise.IsKeptBy(nodeCount, edges));
}
