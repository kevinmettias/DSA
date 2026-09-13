using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TransposeMatrix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TransposeMatrixSolution's. The square workload is built
// once in [GlobalSetup] from a fixed seed, so only the flip itself is measured; at
// Size=800 the source rows no longer fit in cache alongside the destination columns,
// which is where the tiled arm is meant to pull ahead.
[MemoryDiagnoser]
public class TransposeMatrixBenchmarks
{
    private const int MatrixValueUpperBoundExclusive = 1_000;
    private const int Seed = 1;

    [Params(100, 800)]
    public int Size;

    private int[][] _matrix = null!;

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
