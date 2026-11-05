using DSAExperimentation.LeetCode.Solutions.PathWithMaximumProbability;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PathWithMaximumProbability;

public sealed partial class PathWithMaximumProbabilityPromiseTests
{
    public static TheoryData<int, int[][], double[], int, int, bool> Inputs =>
        new()
        {
            { 3, [[0, 1], [1, 2], [0, 2]], [0.5, 0.5, 0.2], 0, 2, true },
            { 3, [[0, 1]], [0.5], 0, 2, true },
            { 3, [[0, 1], [1, 3]], [0.5, 0.5], 0, 2, false },
            { 3, [[0, 1], [1, 2]], [0.5], 0, 2, false },
            { 3, [[0, 1]], [0.5], 0, 3, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryEdgeAndEndpointIsANodeAndEachEdgeHasAProbability(int nodeCount, int[][] edges, double[] successProbabilities, int start, int end, bool expected)
        => Assert.Equal(expected, PathWithMaximumProbabilityPromise.IsKeptBy(nodeCount, edges, successProbabilities, (start, end)));
}
