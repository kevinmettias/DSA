using DSAExperimentation.LeetCode.MajorityElement;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MajorityElement;

// Harness only. The one strategy here is MajorityElementSolution's - this file
// just pins it to LeetCode's published examples plus a couple of edge cases the
// original two-example test left uncovered.
public sealed class MajorityElementTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [3, 2, 3], 3 }, // LC's example 1
            { [2, 2, 1, 1, 1, 2, 2], 2 }, // LC's example 2
            { [1], 1 }, // single element
            { [6, 5, 5], 5 }, // majority found on the very last element
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MajorityByHashMap_LeetCodeExamples_ReturnsMajorityElement(int[] nums, int expected) =>
        Assert.Equal(expected, MajorityElementSolution.MajorityByHashMap(nums));
}
