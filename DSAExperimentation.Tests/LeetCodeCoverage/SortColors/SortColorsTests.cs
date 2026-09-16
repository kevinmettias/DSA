using DSAExperimentation.LeetCode.SortColors;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SortColors;

// Harness only. Both strategies are SortColorsSolution's - this file just pins
// them to LeetCode's published examples.
public sealed partial class SortColorsTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [2, 0, 2, 1, 1, 0], [0, 0, 1, 1, 2, 2] },
            { [2, 0, 1], [0, 1, 2] },
            { [1], [1] },
            { [1, 0], [0, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SortByArraySort_LeetCodeExamples_SortsInPlace(int[] nums, int[] expected)
    {
        var copy = (int[])nums.Clone();

        SortColorsSolution.SortByArraySort(copy);

        Assert.Equal(expected, copy);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SortByDutchFlagPartition_LeetCodeExamples_SortsInPlace(int[] nums, int[] expected)
    {
        var copy = (int[])nums.Clone();

        SortColorsSolution.SortByDutchFlagPartition(copy);

        Assert.Equal(expected, copy);
    }
}
