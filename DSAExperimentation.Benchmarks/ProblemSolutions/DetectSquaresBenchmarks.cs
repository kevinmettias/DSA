using DSAExperimentation.LeetCode.DetectSquares;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DetectSquaresSolution's, the same factories
// DetectSquaresSolutionTests proves correct. [GlobalSetup] builds a dense lattice of points -
// the case where "which points share the query's x-coordinate" actually matters - and
// each arm adds every point once, then queries every point once, so the comparison is
// between rescanning the whole point list per candidate corner and looking the corner
// up in the grouped-by-x map. Each arm returns every query's count, in order.
public class DetectSquaresBenchmarks
{
    private (int X, int Y)[] _points = [];

    // Every count the queries report; sized in setup so the replay allocates nothing.
    private int[] _counts = [];

    [Params(5, 10)]
    public int GridDimension { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _points = Enumerable.Range(0, GridDimension)
            .SelectMany(x => Enumerable.Range(0, GridDimension), (x, y) => (x, y))
            .ToArray();
        _counts = new int[_points.Length];
    }

    [Benchmark(Baseline = true)]
    public int[] ListBased() => Replay(DetectSquaresSolution.CreateByPointListScan());

    [Benchmark]
    public int[] HashMapGroupedByX() => Replay(DetectSquaresSolution.CreateByHashMapGroupedByX());

    private int[] Replay(DetectSquaresSolution.IDetectSquares detector)
    {
        foreach (var point in _points)
        {
            detector.Add(point.X, point.Y);
        }

        for (var i = 0; i < _points.Length; i++)
        {
            _counts[i] = detector.Count(_points[i].X, _points[i].Y);
        }

        return _counts;
    }
}
