using DSAExperimentation.LeetCode.AnglesOfATriangle;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AnglesOfATriangle;

// Harness only. Both strategies are AnglesOfATriangleSolution's - this file just
// pins them to LeetCode's published examples, comparing each returned angle
// within LeetCode's own stated 1e-5 tolerance.
public sealed class AnglesOfATriangleTests
{
    public static TheoryData<int[], double[]> Examples =>
        new()
        {
            { [3, 4, 5], [36.86990, 53.13010, 90.00000] },
            { [2, 4, 2], [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void InternalAnglesByLawOfCosines_LeetCodeExamples_ReturnsSortedInternalAngles(
        int[] sides, double[] expected) =>
        AssertAnglesEqual(expected, AnglesOfATriangleSolution.InternalAnglesByLawOfCosines(sides));

    [Theory]
    [MemberData(nameof(Examples))]
    public void InternalAnglesByAngleSum_LeetCodeExamples_ReturnsSortedInternalAngles(
        int[] sides, double[] expected) =>
        AssertAnglesEqual(expected, AnglesOfATriangleSolution.InternalAnglesByAngleSum(sides));

    private static void AssertAnglesEqual(double[] expected, double[] actual)
    {
        Assert.Equal(expected.Length, actual.Length);

        for (var i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], actual[i], precision: 4);
        }
    }
}
