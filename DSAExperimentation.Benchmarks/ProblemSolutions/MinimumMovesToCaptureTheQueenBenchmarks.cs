using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumMovesToCaptureTheQueen;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumMovesToCaptureTheQueenSolution's, the same
// methods MinimumMovesToCaptureTheQueenTests proves correct (TwoSumBenchmarks
// precedent). Every single query is O(1) on the fixed 8x8 board, so there is no
// LeetCode-shaped input to grow - instead [Params] sizes a BATCH of independent
// random queries per iteration (a workload-sizing decision, same role
// LockWorkloads.BuildDeadends(count, seed) plays for OpenTheLockBenchmarks), which
// is what actually gives BenchmarkDotNet something to scale.
[MemoryDiagnoser]
public class MinimumMovesToCaptureTheQueenBenchmarks
{
    private const int Seed = 3001;

    // The chessboard the queries are drawn from: 1..8 in both coordinates, so the
    // random range is expressed as the board the problem fixes rather than as the
    // exclusive bound 8 happens to imply.
    private const int BoardSize = 8;

    // The queen, the rook and the bishop: every query fixes one square per piece.
    private const int PieceCount = 3;

    private QueenQuery[] _queries = [];

    [Params(1_000, 100_000)]
    public int BatchSize { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _queries = new QueenQuery[BatchSize];

        for (var i = 0; i < BatchSize; i++)
        {
            _queries[i] = RandomDistinctSquares(random);
        }
    }

    private static QueenQuery RandomDistinctSquares(Random random)
    {
        var squares = new HashSet<(int Row, int Col)>();

        while (squares.Count < PieceCount)
        {
            squares.Add((random.Next(1, BoardSize + 1), random.Next(1, BoardSize + 1)));
        }

        var picked = squares.ToArray();
        return new QueenQuery(
            picked[0].Row, picked[0].Col, picked[1].Row, picked[1].Col, picked[2].Row, picked[2].Col);
    }

    [Benchmark(Baseline = true)]
    public int DestinationEnumeration()
    {
        var total = 0;

        foreach (var (a, b, c, d, e, f) in _queries)
        {
            total += MinimumMovesToCaptureTheQueenSolution.MinMovesByDestinationEnumeration(
                new ChessSquare(a, b), new ChessSquare(c, d), new ChessSquare(e, f));
        }

        return total;
    }

    [Benchmark]
    public int LineOfSight()
    {
        var total = 0;

        foreach (var (a, b, c, d, e, f) in _queries)
        {
            total += MinimumMovesToCaptureTheQueenSolution.MinMovesByLineOfSight(
                new ChessSquare(a, b), new ChessSquare(c, d), new ChessSquare(e, f));
        }

        return total;
    }
}
