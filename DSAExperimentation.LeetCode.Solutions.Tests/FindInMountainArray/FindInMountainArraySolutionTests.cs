using DSAExperimentation.LeetCode.Solutions.FindInMountainArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindInMountainArray;

// Harness only. Both strategies are FindInMountainArraySolution's; this file pins
// them to the cases that separate the two
// slopes - a target on the peak itself, and one present on both slopes whose
// smaller index is the required answer.
public sealed partial class FindInMountainArraySolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            // Present only on the descending slope, so the ascending search must
            // miss before the reversed comparer finds it.
            { [1, 5, 10, 20, 15, 8, 2], 8, 5 },

            // The peak itself, which belongs to the ascending slope.
            { [1, 2, 3, 4, 5, 3, 1], 5, 4 },

            // Present on both slopes: the smallest index wins.
            { [1, 2, 3, 4, 5, 3, 1], 1, 0 },

            // Peak at index 1, so the ascending slope is two elements long.
            { [0, 5, 3, 1], 3, 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindIndexByLinearScan_Cases_ReturnsSmallestMatchingIndex(
        int[] mountain, int target, int expected)
    {
        var actual = FindInMountainArraySolution.FindIndexByLinearScan(mountain, target);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindIndexByPeakBisection_Cases_ReturnsSmallestMatchingIndex(
        int[] mountain, int target, int expected)
    {
        var actual = FindInMountainArraySolution.FindIndexByPeakBisection(mountain, target);

        Assert.Equal(expected, actual);
    }
}
