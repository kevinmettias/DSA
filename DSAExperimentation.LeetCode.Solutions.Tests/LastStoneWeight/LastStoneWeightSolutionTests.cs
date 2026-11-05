using DSAExperimentation.LeetCode.Solutions.LastStoneWeight;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LastStoneWeight;

// Harness only: both strategies live in LastStoneWeightSolution and are asserted
// against the same cases - LeetCode's own, the mutual-annihilation case, a
// single stone, and two smash sequences where a difference stone has to be smashed
// again after being put back.
public sealed partial class LastStoneWeightSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [2, 2], 0 },
            { [5], 5 },
            { [3, 7, 2], 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LastStoneWeightByLinearRescan_Cases_ReturnsLastStoneWeight(int[] stones, int expected) =>
        Assert.Equal(expected, LastStoneWeightSolution.LastStoneWeightByLinearRescan(stones));

    [Theory]
    [MemberData(nameof(Cases))]
    public void LastStoneWeightByMaxHeap_Cases_ReturnsLastStoneWeight(int[] stones, int expected) =>
        Assert.Equal(expected, LastStoneWeightSolution.LastStoneWeightByMaxHeap(stones));
}
