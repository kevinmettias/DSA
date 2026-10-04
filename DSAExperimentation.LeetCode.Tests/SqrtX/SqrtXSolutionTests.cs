using DSAExperimentation.LeetCode.SqrtX;

namespace DSAExperimentation.LeetCode.Tests.SqrtX;

// Harness only: both strategies live in SqrtXSolution and are asserted against the
// same examples. The first two rows are LeetCode's published examples. The rest are
// read by hand, each root r pinned by r^2 <= x < (r + 1)^2:
// - 0 and 1 are their own roots; 2 and 3 have root 1 (1 <= x < 4).
// - 2147395599 has root 46339: 46339^2 = 2147302921 <= x < 46340^2 = 2147395600.
// - int.MaxValue (2147483647) has root 46340: 46340^2 = 2147395600 <= x < 46341^2 = 2147488281.
public sealed partial class SqrtXSolutionTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 4, 2 },
            { 8, 2 },
            { 0, 0 },
            { 1, 1 },
            { 2, 1 },
            { 3, 1 },
            { 2147395599, 46339 },
            { int.MaxValue, 46340 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RootByNewtonIteration_LeetCodeExamples_ReturnsFloorRoot(int value, int expected) =>
        Assert.Equal(expected, SqrtXSolution.RootByNewtonIteration(value));

    [Theory]
    [MemberData(nameof(Examples))]
    public void RootByBinarySearch_LeetCodeExamples_ReturnsFloorRoot(int value, int expected) =>
        Assert.Equal(expected, SqrtXSolution.RootByBinarySearch(value));
}
