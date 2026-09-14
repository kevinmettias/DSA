using DSAExperimentation.LeetCode.MakeArrayEmpty;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MakeArrayEmpty;

// Harness only. Both strategies are MakeArrayEmptySolution's - including the
// linear scan, which the benchmark used to own privately as its baseline and
// nothing asserted. Beyond LeetCode's three published examples the cases pin the
// circular sweep's boundaries: a one-element array (no rotation at all), an
// already-ascending array (every step is a removal), and two strictly descending
// arrays, where the sweep wraps past the end on every step and so exercises the
// two-range branch on its own.
public sealed class MakeArrayEmptyTests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            // LC example 1.
            { [3, 4, -1], 5L },

            // LC example 2.
            { [1, 2, 4, 3], 5L },

            // LC example 3: already ascending, so nothing is ever moved to the back.
            { [1, 2, 3], 3L },

            // One element: a single removal.
            { [42], 1L },

            // Descending pairs and triples - the wrap-around branch on every step.
            { [2, 1], 3L },
            { [3, 2, 1], 6L },

            // A general permutation, neither sorted nor reversed.
            { [5, 1, 4, 2, 3], 8L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountOperationsByLinearScan_LeetCodeExamples_ReturnsOperationCountUntilEmpty(
        int[] nums, long expected) =>
        Assert.Equal(expected, MakeArrayEmptySolution.CountOperationsByLinearScan(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountOperationsByFenwickTree_LeetCodeExamples_ReturnsOperationCountUntilEmpty(
        int[] nums, long expected) =>
        Assert.Equal(expected, MakeArrayEmptySolution.CountOperationsByFenwickTree(nums));
}
