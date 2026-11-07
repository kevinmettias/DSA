using DSAExperimentation.Domain.Mathematics.Geometry;
using DSAExperimentation.LeetCode.Solutions.LargestPerimeterTriangle;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LargestPerimeterTriangle;

// Harness only. Both strategies are LargestPerimeterTriangleSolution's - this file
// pins them to one unsorted input whose three largest sides form the best triangle.
// The no-valid-triangle case and the case where the sorted scan has to keep walking
// down past the largest sides are catalog cases. LeetCode's published examples run
// in CatalogCaseTests.
public sealed partial class LargestPerimeterTriangleSolutionTests
{
    // Every triple of sides from 1 to this, flat, closing and unable to meet among them.
    private const int LongestSmallSide = 8;

    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [1, 2, 1, 10, 6, 5], 21 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LargestPerimeterByBruteForceTriples_Cases_ReturnsSumOfBestValidTriple(
        int[] nums, int expected) =>
        Assert.Equal(expected, LargestPerimeterTriangleSolution.LargestPerimeterByBruteForceTriples(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void LargestPerimeterBySortedScan_Cases_ReturnsSumOfBestValidTriple(
        int[] nums, int expected) =>
        Assert.Equal(expected, LargestPerimeterTriangleSolution.LargestPerimeterBySortedScan(nums));

    // The brute-force baseline keeps its own three triangle inequalities (its solution says why),
    // so this holds them to EuclideanPolygons: a lone triple scores its perimeter exactly when its
    // longest side is shorter than the other two together, and 0 otherwise.
    [Fact]
    public void LargestPerimeterByBruteForceTriples_SmallTriples_ScoresWhatCanClose() =>
        Assert.All(SmallTriples(), sides => Assert.Equal(
            ClosingPerimeter(sides),
            LargestPerimeterTriangleSolution.LargestPerimeterByBruteForceTriples(sides)));

    private static IEnumerable<int[]> SmallTriples()
    {
        var sides = Enumerable.Range(1, LongestSmallSide).ToArray();

        return sides.SelectMany(first => sides.SelectMany(second =>
            sides.Select(third => new[] { first, second, third })));
    }

    private static int ClosingPerimeter(int[] sides)
    {
        var (longest, perimeter) = (sides.Max(), sides.Sum());

        return EuclideanPolygons.CanClose(longest, perimeter - longest) ? perimeter : 0;
    }
}
