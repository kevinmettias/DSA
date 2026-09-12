using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FallingSquares;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FallingSquaresSolution's - the classic O(n^2) "check
// every earlier square's footprint for overlap" brute force vs. the O(n log n)
// coordinate-compression + LazySegmentTree approach. Footprints are randomly
// overlapping so both strategies pay their full worst-case cost rather than
// degenerating to disjoint, non-interacting squares.
[MemoryDiagnoser]
public class FallingSquaresBenchmarks
{
    private const int PositionRangeMultiplier = 2;
    private const int MaxSquareSize = 50;

    [Params(100, 1_000)]
    public int SquareCount;

    private int[][] _positions = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _positions = new int[SquareCount][];

        for (var i = 0; i < SquareCount; i++)
        {
            var left = random.Next(0, SquareCount * PositionRangeMultiplier);
            var size = random.Next(1, MaxSquareSize);
            _positions[i] = [left, size];
        }
    }

    [Benchmark(Baseline = true)]
    public List<int> BruteForceOverlapScan() => FallingSquaresSolution.HeightsByBruteForceOverlapScan(_positions);

    [Benchmark]
    public List<int> LazySegmentTreeRangeMax() => FallingSquaresSolution.HeightsByLazySegmentTree(_positions);
}
