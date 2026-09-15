using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SurfaceAreaOf3DShapes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SurfaceAreaOf3DShapesSolution's, the same methods
// SurfaceAreaOf3DShapesTests proves correct. The square grid is built once in
// [GlobalSetup] from a fixed seed so only the face-summing pass is measured; the
// padded arm's own border allocation stays inside the measured method because paying
// for it once is precisely the trade it makes against the baseline's per-cell
// bounds checks.
[MemoryDiagnoser]
public class SurfaceAreaOf3DShapesBenchmarks
{
    // Exclusive upper bound passed to Random.Next(0, _): cell heights land in [0, 49].
    private const int MaxHeight = 50;

    private const int Seed = 1;

    private int[][] _grid = [];

    [Params(50, 400)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _grid = new int[Size][];

        for (var r = 0; r < Size; r++)
        {
            _grid[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                _grid[r][c] = random.Next(0, MaxHeight);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int BoundsCheckedPerNeighbor() =>
        SurfaceAreaOf3DShapesSolution.SurfaceAreaByBoundsCheckedNeighbors(_grid);

    [Benchmark]
    public int PaddedGridNoBoundsChecks() =>
        SurfaceAreaOf3DShapesSolution.SurfaceAreaByPaddedBorder(_grid);
}
