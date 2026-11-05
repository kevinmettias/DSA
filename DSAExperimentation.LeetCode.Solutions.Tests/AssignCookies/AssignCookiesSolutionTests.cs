using DSAExperimentation.LeetCode.Solutions.AssignCookies;

namespace DSAExperimentation.LeetCode.Solutions.Tests.AssignCookies;

// Harness only: both strategies live in AssignCookiesSolution and are asserted
// against the same cases, among them the case where no cookie is large enough
// for either child.
public sealed partial class AssignCookiesSolutionTests
{
    public static TheoryData<int[], int[], int> Cases =>
        new()
        {
            { [5, 9], [1, 2, 3], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindContentChildrenByBruteForceScan_Cases_ReturnsContentChildCount(int[] greed, int[] sizes, int expected)
    {
        var actual = AssignCookiesSolution.FindContentChildrenByBruteForceScan(greed, sizes);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindContentChildrenBySortThenTwoPointer_Cases_ReturnsContentChildCount(int[] greed, int[] sizes, int expected)
    {
        var actual = AssignCookiesSolution.FindContentChildrenBySortThenTwoPointer(greed, sizes);

        Assert.Equal(expected, actual);
    }
}
