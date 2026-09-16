using DSAExperimentation.LeetCode.OddEvenJump;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OddEvenJump;

// Harness only: both strategies live in OddEvenJumpSolution and are asserted against
// the same examples, including the single-index array where the base case is the whole
// answer and an all-equal array where every index qualifies in both directions.
public sealed partial class OddEvenJumpTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [10, 13, 12, 14, 15], 2 },
            { [2, 3, 1, 1, 4], 3 },
            { [5, 1, 3, 4, 2], 3 },
            { [5], 1 },
            { [7, 7, 7, 7], 4 },
            { [3, 2, 1], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void OddEvenJumpsByBruteForceScan_LeetCodeExamples_CountsGoodStartingIndices(
        int[] arr, int expected) =>
        Assert.Equal(expected, OddEvenJumpSolution.OddEvenJumpsByBruteForceScan(arr));

    [Theory]
    [MemberData(nameof(Examples))]
    public void OddEvenJumpsByMergeSortStackSweep_LeetCodeExamples_CountsGoodStartingIndices(
        int[] arr, int expected) =>
        Assert.Equal(expected, OddEvenJumpSolution.OddEvenJumpsByMergeSortStackSweep(arr));
}
