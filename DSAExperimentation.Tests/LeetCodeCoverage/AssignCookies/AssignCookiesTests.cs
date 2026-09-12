using DSAExperimentation.LeetCode.AssignCookies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AssignCookies;

// Harness only: both strategies live in AssignCookiesSolution and are asserted
// against the same examples, including the case where no cookie is large enough
// for either child.
public sealed class AssignCookiesTests
{
    public static TheoryData<int[], int[], int> Examples =>
        new()
        {
            { [1, 2, 3], [1, 1], 1 },
            { [1, 2], [1, 2, 3], 2 },
            { [5, 9], [1, 2, 3], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindContentChildrenByBruteForceScan_LeetCodeExamples_ReturnsContentChildCount(int[] greed, int[] sizes, int expected) =>
        Assert.Equal(expected, AssignCookiesSolution.FindContentChildrenByBruteForceScan(greed, sizes));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindContentChildrenBySortThenTwoPointer_LeetCodeExamples_ReturnsContentChildCount(int[] greed, int[] sizes, int expected) =>
        Assert.Equal(expected, AssignCookiesSolution.FindContentChildrenBySortThenTwoPointer(greed, sizes));
}
