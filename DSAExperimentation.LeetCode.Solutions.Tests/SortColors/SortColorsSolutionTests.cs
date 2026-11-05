using DSAExperimentation.LeetCode.Solutions.SortColors;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SortColors;

// Harness only. Both strategies are SortColorsSolution's - this file just pins
// them to the shortest inputs: a single element, and two elements out of order.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class SortColorsSolutionTests
{
    public static TheoryData<int[], int[]> Cases =>
        new()
        {
            { [1], [1] },
            { [1, 0], [0, 1] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SortByArraySort_Cases_SortsInPlace(int[] nums, int[] expected)
    {
        var copy = (int[])nums.Clone();

        SortColorsSolution.SortByArraySort(copy);

        Assert.Equal(expected, copy);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SortByDutchFlagPartition_Cases_SortsInPlace(int[] nums, int[] expected)
    {
        var copy = (int[])nums.Clone();

        SortColorsSolution.SortByDutchFlagPartition(copy);

        Assert.Equal(expected, copy);
    }
}
