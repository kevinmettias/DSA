using DSAExperimentation.LeetCode.Solutions.OddEvenJump;

namespace DSAExperimentation.LeetCode.Solutions.Tests.OddEvenJump;

// Harness only: both strategies live in OddEvenJumpSolution and are asserted against
// the same cases, among them the single-index array where the base case is the whole
// answer and an all-equal array where every index qualifies in both directions.
public sealed partial class OddEvenJumpSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [5], 1 },
            { [7, 7, 7, 7], 4 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void OddEvenJumpsByBruteForceScan_Cases_CountsGoodStartingIndices(
        int[] arr, int expected) =>
        Assert.Equal(expected, OddEvenJumpSolution.OddEvenJumpsByBruteForceScan(arr));

    [Theory]
    [MemberData(nameof(Cases))]
    public void OddEvenJumpsByMergeSortStackSweep_Cases_CountsGoodStartingIndices(
        int[] arr, int expected) =>
        Assert.Equal(expected, OddEvenJumpSolution.OddEvenJumpsByMergeSortStackSweep(arr));
}
