using BenchmarkDotNet.Attributes;
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
    private const int CenterDivisor = 2;
    private const int PawnSpawnProbabilityDenominator = 4;

    [Params(50, 500)]
    public int Size;

    private char[][] _board = null!;
    private RookSquare _rook;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _board = Enumerable.Range(0, Size).Select(_ => Enumerable.Repeat('.', Size).ToArray()).ToArray();
        _rook = new RookSquare(Size / CenterDivisor, Size / CenterDivisor);
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
        AvailableCapturesForRookSolution.NumRookCapturesByFullBoardScan(_board, _rook);

    [Benchmark]
    public int DirectRayWalk() =>
        AvailableCapturesForRookSolution.NumRookCapturesByRayWalk(_board, _rook);
}
