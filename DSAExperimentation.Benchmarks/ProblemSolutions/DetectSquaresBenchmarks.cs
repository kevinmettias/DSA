using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DetectSquares;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DetectSquaresSolution's, the same factories
// DetectSquaresTests proves correct. [GlobalSetup] builds a dense lattice of points -
// the case where "which points share the query's x-coordinate" actually matters - and
// each arm adds every point once, then queries every point once, so the comparison is
// between rescanning the whole point list per candidate corner and looking the corner
// up in the grouped-by-x map.
[MemoryDiagnoser]
public class DetectSquaresBenchmarks
{
    private (int X, int Y)[] _points = [];

    [Params(5, 10)]
    public int GridDimension { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _points = Enumerable.Range(0, GridDimension)
            .SelectMany(x => Enumerable.Range(0, GridDimension), (x, y) => (x, y))
            .ToArray();

    [Benchmark(Baseline = true)]
    public long ListBased() => Replay(DetectSquaresSolution.CreateByPointListScan());

    [Benchmark]
    public long HashMapGroupedByX() => Replay(DetectSquaresSolution.CreateByHashMapGroupedByX());

    private long Replay(DetectSquaresSolution.IDetectSquares detector)
    {
        foreach (var point in _points)
        {
            detector.Add(point.X, point.Y);
        }

        long total = 0;

        foreach (var query in _points)
        {
            total += detector.Count(query.X, query.Y);
        }

        return total;
    }
}
