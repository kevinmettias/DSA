using DSAExperimentation.Domain.Mathematics.Geometry;
using DSAExperimentation.LeetCode.Solutions.TypeOfTriangle;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TypeOfTriangle;

// Harness only. Both strategies are TypeOfTriangleSolution's, and LeetCode's published examples
// run in CatalogCaseTests. This file holds the pairwise baseline's own three triangle
// inequalities, which it keeps (its solution says why), to EuclideanPolygons.
public sealed partial class TypeOfTriangleSolutionTests
{
    // Every triple of sides from 1 to this, flat, closing and unable to meet among them.
    private const int LongestSmallSide = 8;

    // LeetCode's answer for sides that form no triangle.
    private const string NoTriangle = "none";

    // "none" exactly where the longest side is no shorter than the other two together.
    [Fact]
    public void ClassifyByDirectComparison_SmallTriples_IsNoneExactlyWhereTheSidesCannotClose() =>
        Assert.All(SmallTriples(), sides => Assert.Equal(
            !CanClose(sides),
            TypeOfTriangleSolution.ClassifyByDirectComparison(sides) == NoTriangle));

    private static IEnumerable<int[]> SmallTriples()
    {
        var sides = Enumerable.Range(1, LongestSmallSide).ToArray();

        return sides.SelectMany(first => sides.SelectMany(second =>
            sides.Select(third => new[] { first, second, third })));
    }

    private static bool CanClose(int[] sides)
    {
        var longest = sides.Max();

        return EuclideanPolygons.CanClose(longest, sides.Sum() - longest);
    }
}
