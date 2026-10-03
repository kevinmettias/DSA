using DSAExperimentation.LeetCode.TransposeMatrix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TransposeMatrixSolution's. The square workload is built
// once in [GlobalSetup] from a fixed seed, so only the flip itself is measured. The
// tiled arm is meant to pull ahead once the source rows no longer fit in cache
// alongside the destination columns, but Size stops at 316, the largest square inside
// LC 867's m * n <= 10^5 cells, whether or not that point is reached by then.
public class TransposeMatrixBenchmarks
{
    private const int MatrixValueUpperBoundExclusive = 1_000;
    private const int Seed = 1;

    private int[][] _matrix = [];

    [Params(100, 316)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _matrix = new int[Size][];

        for (var r = 0; r < Size; r++)
        {
            _matrix[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                _matrix[r][c] = random.Next(1, MatrixValueUpperBoundExclusive);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int[][] DirectIndexSwap() => TransposeMatrixSolution.TransposeByIndexSwap(_matrix);

    [Benchmark]
    public int[][] CacheBlockedTranspose() => TransposeMatrixSolution.TransposeByCacheBlocking(_matrix);
}
