using DSAExperimentation.LeetCode.LastStoneWeight;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LastStoneWeight;

// Harness only: both strategies live in LastStoneWeightSolution and are asserted
// against the same examples - LeetCode's own, the mutual-annihilation case, a
// single stone, and two smash sequences where a difference stone has to be smashed
// again after being put back.
public sealed partial class LastStoneWeightTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 7, 4, 1, 8, 1], 1 },
            { [2, 2], 0 },
            { [5], 5 },
            { [3, 7, 2], 2 },
            { [10, 4, 2, 10], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LastStoneWeightByLinearRescan_LeetCodeExamples_ReturnsLastStoneWeight(int[] stones, int expected) =>
        Assert.Equal(expected, LastStoneWeightSolution.LastStoneWeightByLinearRescan(stones));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LastStoneWeightByMaxHeap_LeetCodeExamples_ReturnsLastStoneWeight(int[] stones, int expected) =>
        Assert.Equal(expected, LastStoneWeightSolution.LastStoneWeightByMaxHeap(stones));
}
