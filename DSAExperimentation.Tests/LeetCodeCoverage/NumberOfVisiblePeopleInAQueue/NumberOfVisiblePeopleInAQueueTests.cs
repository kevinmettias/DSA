using DSAExperimentation.LeetCode.NumberOfVisiblePeopleInAQueue;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfVisiblePeopleInAQueue;

// Harness only. Both strategies are NumberOfVisiblePeopleInAQueueSolution's - this
// file just pins them to LeetCode's published examples plus the tie-heavy shapes
// that separate a correct visibility rule from a sloppy one: a run of equal heights,
// and equal heights sandwiched between two taller people.
public sealed partial class NumberOfVisiblePeopleInAQueueTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [10, 6, 8, 5, 11, 9], [3, 1, 2, 1, 1, 0] },
            { [5, 1, 2, 3, 10], [4, 1, 1, 1, 0] },
            { [1, 2, 3, 4, 5], [1, 1, 1, 1, 0] },
            { [5, 5, 5, 5], [1, 1, 1, 0] },
            { [5, 3, 3, 5], [2, 1, 1, 0] },
            { [7], [0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountVisibleByBruteForceScan_LeetCodeExamples_ReturnsVisibleCountPerPerson(
        int[] heights, int[] expected) =>
        Assert.Equal(expected, NumberOfVisiblePeopleInAQueueSolution.CountVisibleByBruteForceScan(heights));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountVisibleByMonotonicStackSweep_LeetCodeExamples_ReturnsVisibleCountPerPerson(
        int[] heights, int[] expected) =>
        Assert.Equal(expected, NumberOfVisiblePeopleInAQueueSolution.CountVisibleByMonotonicStackSweep(heights));
}
