using DSAExperimentation.LeetCode.DetectSquares;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DetectSquaresSolution's, the same factories
// DetectSquaresSolutionTests proves correct. [GlobalSetup] builds a dense lattice of points -
// the case where "which points share the query's x-coordinate" actually matters - and
// each arm adds every point once, then queries every point once, so the comparison is
// between rescanning the whole point list per candidate corner and looking the corner
// up in the grouped-by-x map. Each arm returns every query's count, in order.
//
// Sizes are per arm. The list scan rescans every stored point for each corner it checks,
// O(d^5) over a d x d lattice's adds and queries, so it stops at 10; the grouped map visits
// only the query's own column, O(d^3), and runs on to 38 - the largest lattice whose adds
// and queries stay within LC 2013's 3000 calls. The two are compared at the sizes both run.
public class DetectSquaresBenchmarks
{
    private Dictionary<int, (int X, int Y)[]> _pointsByDimension = [];

    // Every count the queries report, one buffer per size; sized in setup so the replay allocates nothing.
    private Dictionary<int, int[]> _countsByDimension = [];

    public static IEnumerable<int> ListScanSizes => [5, 10];

    public static IEnumerable<int> GroupedByXSizes => [.. ListScanSizes, 20, 38];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup()
    {
        _pointsByDimension = GroupedByXSizes.ToDictionary(gridDimension => gridDimension, Lattice);
        _countsByDimension = GroupedByXSizes.ToDictionary(
            gridDimension => gridDimension,
            gridDimension => new int[gridDimension * gridDimension]);
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(ListScanSizes))]
    public int[] ListBased(int gridDimension) =>
        Replay(DetectSquaresSolution.CreateByPointListScan(), gridDimension);

    [Benchmark]
    [ArgumentsSource(nameof(GroupedByXSizes))]
    public int[] HashMapGroupedByX(int gridDimension) =>
        Replay(DetectSquaresSolution.CreateByHashMapGroupedByX(), gridDimension);

    private static (int X, int Y)[] Lattice(int gridDimension) =>
        Enumerable.Range(0, gridDimension)
            .SelectMany(x => Enumerable.Range(0, gridDimension), (x, y) => (x, y))
            .ToArray();

    private int[] Replay(DetectSquaresSolution.IDetectSquares detector, int gridDimension)
    {
        var points = _pointsByDimension[gridDimension];
        var counts = _countsByDimension[gridDimension];

        foreach (var point in points)
        {
            detector.Add(point.X, point.Y);
        }

        for (var i = 0; i < points.Length; i++)
        {
            counts[i] = detector.Count(points[i].X, points[i].Y);
        }

        return counts;
    }
}
