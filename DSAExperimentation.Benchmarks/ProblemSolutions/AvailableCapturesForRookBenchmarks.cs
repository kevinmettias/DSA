using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.AvailableCapturesForRook;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AvailableCapturesForRookSolution's, the same methods
// AvailableCapturesForRookSolutionTests proves correct. Each is handed the prepared
// RookSquare its hoisted overload takes, so locating the rook - an O(rows*cols)
// scan that would swamp the ray walk it is meant to expose - is charged to
// [GlobalSetup] rather than to the measured search.
//
// LC 999's board is always 8x8, so there is no size axis: the comparison is the
// constant-factor cost of scanning all 64 squares against walking the rook's four rays.
public class AvailableCapturesForRookBenchmarks
{
    private const int PawnSpawnProbabilityDenominator = 4;
    private const int SquaresPerSide = 8;

    private char[][] _board = [];

    private RookSquare _rook;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _board = Enumerable.Range(0, SquaresPerSide).Select(_ => Enumerable.Repeat('.', SquaresPerSide).ToArray()).ToArray();
        _rook = new RookSquare(SquaresPerSide / AlgorithmConstants.HalvingFactor, SquaresPerSide / AlgorithmConstants.HalvingFactor);
        _board[_rook.Row][_rook.Col] = 'R';

        for (var col = 0; col < SquaresPerSide; col++)
        {
            if (col != _rook.Col && random.Next(PawnSpawnProbabilityDenominator) == 0)
            {
                _board[_rook.Row][col] = 'p';
            }
        }

        for (var row = 0; row < SquaresPerSide; row++)
        {
            if (row != _rook.Row && random.Next(PawnSpawnProbabilityDenominator) == 0)
            {
                _board[row][_rook.Col] = 'p';
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int FullBoardScan() =>
        AvailableCapturesForRookSolution.CountRookCapturesByFullBoardScan(_board, _rook);

    [Benchmark]
    public int DirectRayWalk() =>
        AvailableCapturesForRookSolution.CountRookCapturesByRayWalk(_board, _rook);
}
