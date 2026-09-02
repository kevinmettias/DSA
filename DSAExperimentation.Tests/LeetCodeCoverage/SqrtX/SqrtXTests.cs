using DSAExperimentation.LeetCode.SqrtX;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SqrtX;

// Harness only: both strategies live in SqrtXSolution and are asserted against the
// same examples, including the near-int-max case that stresses the search range.
public sealed class SqrtXTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 0, 0 },
            { 1, 1 },
            { 4, 2 },
            { 8, 2 },
            { 2147395599, 46339 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RootByMathSqrt_LeetCodeExamples_ReturnsFloorRoot(int x, int expected) =>
        Assert.Equal(expected, SqrtXSolution.RootByMathSqrt(x));

    [Theory]
    [MemberData(nameof(Examples))]
    public void RootByBinarySearch_LeetCodeExamples_ReturnsFloorRoot(int x, int expected) =>
        Assert.Equal(expected, SqrtXSolution.RootByBinarySearch(x));
}
