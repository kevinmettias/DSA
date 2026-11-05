using DSAExperimentation.LeetCode.Solutions.SqrtX;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SqrtX;

// Harness only: both strategies live in SqrtXSolution and are asserted against the
// same cases. The one case is read by hand, its root r pinned by
// r^2 <= x < (r + 1)^2: 2147395599 has root 46339, since
// 46339^2 = 2147302921 <= x < 46340^2 = 2147395600, which makes it the largest x
// whose root is 46339. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class SqrtXSolutionTests
{
    public static TheoryData<int, int> Cases =>
        new()
        {
            { 2147395599, 46339 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void RootByNewtonIteration_Cases_ReturnsFloorRoot(int value, int expected) =>
        Assert.Equal(expected, SqrtXSolution.RootByNewtonIteration(value));

    [Theory]
    [MemberData(nameof(Cases))]
    public void RootByBinarySearch_Cases_ReturnsFloorRoot(int value, int expected) =>
        Assert.Equal(expected, SqrtXSolution.RootByBinarySearch(value));
}
