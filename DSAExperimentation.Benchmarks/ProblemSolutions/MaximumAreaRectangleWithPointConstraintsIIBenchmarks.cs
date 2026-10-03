using DSAExperimentation.LeetCode.MaximumAreaRectangleWithPointConstraintsII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MaximumAreaRectangleWithPointConstraintsIISolution's, the same methods
// MaximumAreaRectangleWithPointConstraintsIISolutionTests proves correct. The sweep arm
// is handed its prepared, x-then-y sorted Point[] from [GlobalSetup] so sorting
// is never part of the measured sweep - the same reason
// MaximumAreaRectangleWithPointConstraintsIBenchmarks hands its corner-lookup
// arm a pre-built Set. Points are a dense grid, same shape Part I's benchmark
// uses, so both arms have real rectangles to find rather than scanning a mostly
// empty space.
//
// Sizes are per arm. The quadruple scan is O(n^5) in the point count, so it stops at
// an 8 x 8 grid; the O(n log n) sweep runs on to a 400 x 400 grid, whose 160,000 points
// stay inside LC 3382's bound of 2 * 10^5. The two are compared at the sizes both run.
public class MaximumAreaRectangleWithPointConstraintsIIBenchmarks
{
    private Dictionary<int, (int[] XCoord, int[] YCoord)> _coordinatesBySide = [];

    private Dictionary<int, MaximumAreaRectangleWithPointConstraintsIISolution.Point[]> _sortedPointsBySide = [];

    public static IEnumerable<int> QuadrupleScanSizes => [4, 8];

    public static IEnumerable<int> SweepSizes => [.. QuadrupleScanSizes, 64, 400];

    // Every grid side any arm runs is built here, outside the timed region, in both shapes
    // the arms take; an arm looks its own up.
    [GlobalSetup]
    public void Setup()
    {
        _coordinatesBySide = SweepSizes.ToDictionary(side => side, BuildGridCoordinates);
        _sortedPointsBySide = _coordinatesBySide.ToDictionary(entry => entry.Key, entry => SortPoints(entry.Value));
    }

    private static (int[] XCoord, int[] YCoord) BuildGridCoordinates(int gridSide)
    {
        var n = gridSide * gridSide;
        var xCoord = new int[n];
        var yCoord = new int[n];
        var index = 0;

        for (var x = 0; x < gridSide; x++)
        {
            for (var y = 0; y < gridSide; y++)
            {
                xCoord[index] = x;
                yCoord[index] = y;
                index++;
            }
        }

        return (xCoord, yCoord);
    }

    private static MaximumAreaRectangleWithPointConstraintsIISolution.Point[] SortPoints((int[] XCoord, int[] YCoord) grid) =>
        Enumerable.Range(0, grid.XCoord.Length)
            .Select(i => new MaximumAreaRectangleWithPointConstraintsIISolution.Point(grid.XCoord[i], grid.YCoord[i]))
            .OrderBy(p => p.X)
            .ThenBy(p => p.Y)
            .ToArray();

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(QuadrupleScanSizes))]
    public long QuadrupleScan(int gridSide)
    {
        var (xCoord, yCoord) = _coordinatesBySide[gridSide];

        return MaximumAreaRectangleWithPointConstraintsIISolution.MaxAreaByQuadrupleScan(xCoord, yCoord);
    }

    [Benchmark]
    [ArgumentsSource(nameof(SweepSizes))]
    public long SweepSegmentTree(int gridSide) =>
        MaximumAreaRectangleWithPointConstraintsIISolution.MaxAreaBySweepSegmentTree(_sortedPointsBySide[gridSide]);
}
