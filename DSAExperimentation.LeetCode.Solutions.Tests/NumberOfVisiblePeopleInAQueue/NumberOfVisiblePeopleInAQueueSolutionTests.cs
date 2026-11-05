using DSAExperimentation.LeetCode.Solutions.NumberOfVisiblePeopleInAQueue;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfVisiblePeopleInAQueue;

// Harness only. Both strategies are NumberOfVisiblePeopleInAQueueSolution's - this
// file just pins them to the tie-heavy shapes
// that separate a correct visibility rule from a sloppy one: a run of equal heights,
// and equal heights sandwiched between two taller people.
public sealed partial class NumberOfVisiblePeopleInAQueueSolutionTests
{
    public static TheoryData<int[], int[]> Cases =>
        new()
        {
            { [5, 5, 5, 5], [1, 1, 1, 0] },
            { [5, 3, 3, 5], [2, 1, 1, 0] },
            { [7], [0] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountVisibleByBruteForceScan_Cases_ReturnsVisibleCountPerPerson(
        int[] heights, int[] expected) =>
        Assert.Equal(expected, NumberOfVisiblePeopleInAQueueSolution.CountVisibleByBruteForceScan(heights));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountVisibleByMonotonicStackSweep_Cases_ReturnsVisibleCountPerPerson(
        int[] heights, int[] expected) =>
        Assert.Equal(expected, NumberOfVisiblePeopleInAQueueSolution.CountVisibleByMonotonicStackSweep(heights));
}
