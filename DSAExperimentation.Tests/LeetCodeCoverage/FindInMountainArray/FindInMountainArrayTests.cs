using DSAExperimentation.LeetCode.FindInMountainArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindInMountainArray;

// Harness only. Both strategies are FindInMountainArraySolution's; this file pins
// them to LeetCode's published examples plus the cases that separate the two
// slopes - a target on the peak itself, and one present on both slopes whose
// smaller index is the required answer.
public sealed class FindInMountainArrayTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            // LeetCode example 1: target sits on the ascending slope.
            { [1, 2, 3, 4, 5, 3, 1], 3, 2 },

            // LeetCode example 2: target is absent from both slopes.
            { [0, 1, 2, 4, 2, 1], 3, -1 },

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
    [MemberData(nameof(Examples))]
    public void FindIndexByLinearScan_LeetCodeExamples_ReturnsSmallestMatchingIndex(
        int[] mountain, int target, int expected)
    {
        var actual = FindInMountainArraySolution.FindIndexByLinearScan(mountain, target);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindIndexByPeakBisection_LeetCodeExamples_ReturnsSmallestMatchingIndex(
        int[] mountain, int target, int expected)
    {
        var actual = FindInMountainArraySolution.FindIndexByPeakBisection(mountain, target);

        Assert.Equal(expected, actual);
    }
}
