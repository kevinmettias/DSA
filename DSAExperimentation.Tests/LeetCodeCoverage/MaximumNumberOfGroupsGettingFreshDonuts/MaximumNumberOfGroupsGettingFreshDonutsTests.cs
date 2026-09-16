using DSAExperimentation.LeetCode.MaximumNumberOfGroupsGettingFreshDonuts;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfGroupsGettingFreshDonuts;

// Harness only. Both strategies are MaximumNumberOfGroupsGettingFreshDonutsSolution's
// - this file pins them to LeetCode's two published examples, the hand-verified case
// the original coverage carried, and two edges the original left untested: a
// batchSize of 1 (every group is happy, and the remainder table is empty) and groups
// that are all whole batches (the memoized search never runs at all).
public sealed class MaximumNumberOfGroupsGettingFreshDonutsTests
{
    public static TheoryData<int, int[], int> Examples =>
        new()
        {
            // LeetCode example 1. Remainders are {0,0,1,1,2,2}: the two whole-batch
            // groups are happy for free, and the best ordering of the remaining four
            // (two 1s, two 2s) adds two more - 2+2=4.
            { 3, [1, 2, 3, 4, 5, 6], 4 },
            // LeetCode example 2.
            { 4, [1, 3, 2, 5, 2, 2, 1, 6], 4 },
            // Remainders are {1,3,0,2}: the whole-batch group is happy for free, and
            // no ordering of remainders 1, 2 and 3 makes more than two of them happy
            // (1,3,2 makes the first happy by starting fresh and the last happy
            // because 1+3 completes a batch) - 1+2=3.
            { 4, [1, 3, 4, 6], 3 },
            // Every running total is a multiple of 1, so every group is happy.
            { 1, [1, 2, 3, 4], 4 },
            // Every group is already a whole batch, so the running residue never
            // leaves zero.
            { 3, [3, 6, 9], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxHappyGroupsByAllPermutations_LeetCodeExamples_ReturnsHappyGroupCount(
        int batchSize, int[] groups, int expected)
    {
        var actual = MaximumNumberOfGroupsGettingFreshDonutsSolution.MaxHappyGroupsByAllPermutations(batchSize, groups);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxHappyGroupsByMemoizedRecurrence_LeetCodeExamples_ReturnsHappyGroupCount(
        int batchSize, int[] groups, int expected)
    {
        var actual = MaximumNumberOfGroupsGettingFreshDonutsSolution.MaxHappyGroupsByMemoizedRecurrence(
            batchSize, groups);

        Assert.Equal(expected, actual);
    }
}
