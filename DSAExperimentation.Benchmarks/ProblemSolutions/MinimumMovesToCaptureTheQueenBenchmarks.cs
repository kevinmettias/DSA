using DSAExperimentation.Domain.Games.Chess;
using DSAExperimentation.LeetCode.Solutions.MinimumMovesToCaptureTheQueen;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumMovesToCaptureTheQueenSolution's, the same
// methods MinimumMovesToCaptureTheQueenSolutionTests proves correct (TwoSumBenchmarks
// precedent). Every single query is O(1) on the fixed 8x8 board, so there is no
// LeetCode-shaped input to grow - instead [Params] sizes a BATCH of independent
// random queries per iteration (a workload-sizing decision, same role
// LockWorkloads.BuildDeadends(count, seed) plays for OpenTheLockBenchmarks), which
// is what actually gives BenchmarkDotNet something to scale.
public class MinimumMovesToCaptureTheQueenBenchmarks
{
    private const int Seed = 3001;

    // The queen, the rook and the bishop: every query fixes one square per piece.
    private const int PieceCount = 3;

    private QueenQuery[] _queries = [];

    // Every query's move count, in batch order - what each arm returns.
    private int[] _moves = [];

    [Params(1_000, 100_000)]
    public int BatchSize { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _queries = new QueenQuery[BatchSize];
        _moves = new int[BatchSize];

        for (var i = 0; i < BatchSize; i++)
        {
            _queries[i] = RandomDistinctSquares(random);
        }
    }

    // The squares are handed out in the order they were drawn. A HashSet's order follows the
    // tuples' hashes, which .NET seeds afresh in every process, so each benchmark process gave the
    // pieces different squares and the same code read up to 23% apart from run to run.
    private static QueenQuery RandomDistinctSquares(Random random)
    {
        var squares = new List<(int Row, int Col)>(PieceCount);

        while (squares.Count < PieceCount)
        {
            var square = (random.Next(1, ChessRules.BoardSize + 1), random.Next(1, ChessRules.BoardSize + 1));
            var isNewSquare = !squares.Contains(square);

            if (isNewSquare)
            {
                squares.Add(square);
            }
        }

        return new QueenQuery(
            squares[0].Row, squares[0].Col, squares[1].Row, squares[1].Col, squares[2].Row, squares[2].Col);
    }

    [Benchmark(Baseline = true)]
    public int[] DestinationEnumeration()
    {
        for (var i = 0; i < _queries.Length; i++)
        {
            var (a, b, c, d, e, f) = _queries[i];
            _moves[i] = MinimumMovesToCaptureTheQueenSolution.MinMovesByDestinationEnumeration(
                new ChessSquare(a, b), new ChessSquare(c, d), new ChessSquare(e, f));
        }

        return _moves;
    }

    [Benchmark]
    public int[] LineOfSight()
    {
        for (var i = 0; i < _queries.Length; i++)
        {
            var (a, b, c, d, e, f) = _queries[i];
            _moves[i] = MinimumMovesToCaptureTheQueenSolution.MinMovesByLineOfSight(
                new ChessSquare(a, b), new ChessSquare(c, d), new ChessSquare(e, f));
        }

        return _moves;
    }
}
