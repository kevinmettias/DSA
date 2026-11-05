using DSAExperimentation.LeetCode.Solutions.MajorityElement;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MajorityElement;

// Harness only. Both strategies here are MajorityElementSolution's - this file
// pins them to two edge cases the original two-example test left uncovered: a
// single element, and a majority settled only by the last element. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class MajorityElementSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [1], 1 }, // single element
            { [6, 5, 5], 5 }, // majority found on the very last element
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MajorityByHashMap_Cases_ReturnsMajorityElement(int[] nums, int expected) =>
        Assert.Equal(expected, MajorityElementSolution.MajorityByHashMap(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MajorityByBoyerMooreVoting_Cases_ReturnsMajorityElement(int[] nums, int expected) =>
        Assert.Equal(expected, MajorityElementSolution.MajorityByBoyerMooreVoting(nums));
}
