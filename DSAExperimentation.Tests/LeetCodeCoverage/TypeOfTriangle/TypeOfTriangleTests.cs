using DSAExperimentation.LeetCode.TypeOfTriangle;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TypeOfTriangle;

// Harness only: the algorithms live in TypeOfTriangleSolution. One test method per
// strategy over one shared set of LeetCode's own examples, so a failure names the
// strategy that broke.
public sealed partial class TypeOfTriangleTests
{
    public static TheoryData<int[], string> Examples =>
        new()
        {
            { [3, 3, 3], "equilateral" },
            { [3, 4, 5], "scalene" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ClassifyByDirectComparison_LeetCodeExamples_ReturnsTriangleType(int[] sides, string expected) =>
        Assert.Equal(expected, TypeOfTriangleSolution.ClassifyByDirectComparison(sides));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ClassifyByMergeSort_LeetCodeExamples_ReturnsTriangleType(int[] sides, string expected) =>
        Assert.Equal(expected, TypeOfTriangleSolution.ClassifyByMergeSort(sides));
}
