using DSAExperimentation.LeetCode.Solutions.FindEventualSafeStates;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindEventualSafeStates;

public sealed partial class FindEventualSafeStatesPromiseTests
{
    public static TheoryData<int[][], bool> Inputs =>
        new()
        {
            { [[1, 2], [2, 3], [5], [0], [5], [], []], true },
            { [[1], [2]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryEdgeLeadsToANode(int[][] graph, bool expected)
        => Assert.Equal(expected, FindEventualSafeStatesPromise.IsKeptBy(graph));
}
