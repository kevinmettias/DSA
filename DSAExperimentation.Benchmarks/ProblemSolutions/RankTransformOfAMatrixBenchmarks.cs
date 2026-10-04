using DSAExperimentation.LeetCode.RankTransformOfAMatrix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RankTransformOfAMatrixSolution's, the same methods
// RankTransformOfAMatrixSolutionTests proves correct. Values are random in
// [-10^5, 10^5), inside LeetCode's [-10^9, 10^9] and narrow enough that equal values
// meet in a shared row or column at the larger sides, which is the case the disjoint set
// is there for.
//
// Sizes are per arm. DisjointSetRanking runs on to LC 1632's 500 x 500; the relaxation
// rescans every pair in every row and column until a pass changes nothing, and stops
// at 50 x 50.
public class RankTransformOfAMatrixBenchmarks
{
    private const int RandomSeed = 1632; // LC 1632: Rank Transform of a Matrix

    private const int ValueRange = 100_000;

    private Dictionary<int, int[][]> _matrixBySide = [];

    public static IEnumerable<int> RelaxationSides => [8, 20, 50];

    public static IEnumerable<int> DisjointSetSides => [.. RelaxationSides, 150, 500];

    // Every side any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _matrixBySide = RelaxationSides.Union(DisjointSetSides).ToDictionary(side => side, BuildMatrix);

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(RelaxationSides))]
    public int[][] IterativeRelaxation(int side) =>
        RankTransformOfAMatrixSolution.MatrixRankTransformByIterativeRelaxation(_matrixBySide[side]);

    [Benchmark]
    [ArgumentsSource(nameof(DisjointSetSides))]
    public int[][] DisjointSetRanking(int side) =>
        RankTransformOfAMatrixSolution.MatrixRankTransformByDisjointSetRanking(_matrixBySide[side]);

    private static int[][] BuildMatrix(int side)
    {
        var random = new Random(RandomSeed);
        var matrix = new int[side][];

        for (var r = 0; r < side; r++)
        {
            matrix[r] = new int[side];

            for (var c = 0; c < side; c++)
            {
                matrix[r][c] = random.Next(-ValueRange, ValueRange);
            }
        }

        return matrix;
    }
}
