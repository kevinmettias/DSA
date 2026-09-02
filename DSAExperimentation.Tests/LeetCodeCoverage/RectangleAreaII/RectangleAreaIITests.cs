using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RectangleAreaII;

// LeetCode 850. Rectangle Area II: a sweep line over the distinct x-coordinates -
// at each vertical slab, every rectangle spanning the full slab contributes a
// y-range, and this repo's own IntervalSet<TKey> merges those ranges into
// disjoint intervals so the covered height sums to exactly the union length
// (no double-counting where rectangles overlap) - the same closed-interval merge
// MergeIntervalsTests already reuses for LC 56.
public sealed partial class RectangleAreaIITests
{
    private const int Modulus = 1_000_000_007;

    [Fact]
    public void TotalArea_ClassicExample_ReturnsUnionArea()
    {
        int[][] rectangles = [[0, 0, 2, 2], [1, 0, 2, 3], [1, 0, 3, 1]];

        var area = TotalArea(rectangles);

        Assert.Equal(6, area);
    }

    [Fact]
    public void TotalArea_SingleHugeRectangle_ReturnsAreaModuloOneBillionSeven()
    {
        int[][] rectangles = [[0, 0, 1_000_000_000, 1_000_000_000]];

        var area = TotalArea(rectangles);

        // A single 10^9 x 10^9 rectangle: 10^18 mod (10^9 + 7), the case that
        // forces the overflow-safe modular accumulation above rather than a
        // plain long sum.
        Assert.Equal(49, area);
    }

    [Fact]
    public void TotalArea_DisjointRectangles_SumsBothAreasWithNoOverlap()
    {
        int[][] rectangles = [[0, 0, 2, 2], [10, 10, 12, 12]];

        var area = TotalArea(rectangles);

        Assert.Equal(8, area);
    }

    private static int TotalArea(int[][] rectangles)
    {
        var xs = rectangles.SelectMany(r => new[] { r[0], r[2] }).Distinct().OrderBy(x => x).ToArray();
        long area = 0;

        for (var i = 0; i < xs.Length - 1; i++)
        {
            var slabArea = SlabArea(rectangles, xs[i], xs[i + 1]);
            area = (area + slabArea) % Modulus;
        }

        return (int)area;
    }

    private static long SlabArea(int[][] rectangles, int x1, int x2)
    {
        var yIntervals = new IntervalSet<int>();

        foreach (var rectangle in rectangles)
        {
            if (rectangle[0] <= x1 && rectangle[2] >= x2)
            {
                yIntervals.Add(rectangle[1], rectangle[3]);
            }
        }

        long height = 0;
        for (var j = 0; j < yIntervals.Count; j++)
        {
            var (start, end) = yIntervals.Get(j);
            height += end - start;
        }

        return (x2 - x1) * (height % Modulus);
    }
}
