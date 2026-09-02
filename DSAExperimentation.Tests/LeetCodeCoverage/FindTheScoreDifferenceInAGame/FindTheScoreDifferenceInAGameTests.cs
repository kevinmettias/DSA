using DSAExperimentation.LeetCode.FindTheScoreDifferenceInAGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheScoreDifferenceInAGame;

// Harness only. Both strategies are FindTheScoreDifferenceInAGameSolution's - this
// file just pins them to hand-simulated examples, including a game that ends after
// a single turn (marking a neighbor finishes the array) and a fully-tied one.
public sealed class FindTheScoreDifferenceInAGameTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 1, 3, 4, 5, 2], 3 },
            { [1, 1], 1 },
            { [5, 5, 5, 5], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ScoreDifferenceByLinearScan_LeetCodeExamples_ReturnsPlayerOneMinusPlayerTwoScore(
        int[] nums, int expected) =>
        Assert.Equal(expected, FindTheScoreDifferenceInAGameSolution.ScoreDifferenceByLinearScan(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ScoreDifferenceByMinHeap_LeetCodeExamples_ReturnsPlayerOneMinusPlayerTwoScore(
        int[] nums, int expected) =>
        Assert.Equal(expected, FindTheScoreDifferenceInAGameSolution.ScoreDifferenceByMinHeap(nums));
}
