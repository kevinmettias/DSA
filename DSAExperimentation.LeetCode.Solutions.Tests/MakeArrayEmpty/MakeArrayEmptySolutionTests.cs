using DSAExperimentation.LeetCode.Solutions.MakeArrayEmpty;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MakeArrayEmpty;

// Harness only. Both strategies are MakeArrayEmptySolution's - including the
// linear scan, which the benchmark used to own privately as its baseline and
// nothing asserted. The cases pin one boundary of the circular sweep: two strictly
// descending arrays, where the sweep wraps past the end on every step and so
// exercises the two-range branch on its own. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class MakeArrayEmptySolutionTests
{
    public static TheoryData<int[], long> Cases =>
        new()
        {
            // A descending pair and triple - the wrap-around branch on every step.
            { [2, 1], 3L },
            { [3, 2, 1], 6L },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountOperationsByLinearScan_Cases_ReturnsOperationCountUntilEmpty(
        int[] nums, long expected) =>
        Assert.Equal(expected, MakeArrayEmptySolution.CountOperationsByLinearScan(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountOperationsByFenwickTree_Cases_ReturnsOperationCountUntilEmpty(
        int[] nums, long expected) =>
        Assert.Equal(expected, MakeArrayEmptySolution.CountOperationsByFenwickTree(nums));
}
