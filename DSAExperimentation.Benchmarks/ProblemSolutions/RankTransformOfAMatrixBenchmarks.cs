using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RankTransformOfAMatrix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RankTransformOfAMatrixSolution's, the same methods
// RankTransformOfAMatrixTests proves correct. Values are random in
// [-10^5, 10^5), matching LeetCode's own constraint range.
[MemoryDiagnoser]
public class RankTransformOfAMatrixBenchmarks
{
    private const int RandomSeed = 1632; // LC 1632: Rank Transform of a Matrix

    private const int ValueRange = 100_000;

    [Params(8, 20)]
    public int Size;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _matrix = new int[Size][];

        for (var r = 0; r < Size; r++)
        {
            _matrix[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                _matrix[r][c] = random.Next(-ValueRange, ValueRange);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int[][] IterativeRelaxation() =>
        RankTransformOfAMatrixSolution.MatrixRankTransformByIterativeRelaxation(_matrix);

    [Benchmark]
    public int[][] DisjointSetRanking() =>
        RankTransformOfAMatrixSolution.MatrixRankTransformByDisjointSetRanking(_matrix);
}
