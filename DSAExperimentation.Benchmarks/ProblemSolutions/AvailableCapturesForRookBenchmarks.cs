using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.AvailableCapturesForRook;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AvailableCapturesForRookSolution's, the same methods
// AvailableCapturesForRookTests proves correct. Each is handed the prepared
// RookSquare its hoisted overload takes, so locating the rook - an O(rows*cols)
// scan that would swamp the ray walk it is meant to expose - is charged to
// [GlobalSetup] rather than to the measured search.
[MemoryDiagnoser]
public class AvailableCapturesForRookBenchmarks
{
    private const int PawnSpawnProbabilityDenominator = 4;

    private char[][] _board = [];

    private RookSquare _rook;
    [Params(50, 500)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _board = Enumerable.Range(0, Size).Select(_ => Enumerable.Repeat('.', Size).ToArray()).ToArray();
        _rook = new RookSquare(Size / AlgorithmConstants.HalvingFactor, Size / AlgorithmConstants.HalvingFactor);
        _board[_rook.Row][_rook.Col] = 'R';

        for (var col = 0; col < Size; col++)
        {
            if (col != _rook.Col && random.Next(PawnSpawnProbabilityDenominator) == 0)
            {
                _board[_rook.Row][col] = 'p';
            }
        }

        for (var row = 0; row < Size; row++)
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
