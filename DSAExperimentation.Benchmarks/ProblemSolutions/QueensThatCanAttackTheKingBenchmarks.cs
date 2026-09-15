using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.QueensThatCanAttackTheKing;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are QueensThatCanAttackTheKingSolution's, the same methods
// QueensThatCanAttackTheKingTests proves correct. They ray-walk the same 8 queen-move
// directions and differ only in how "is this square occupied" is answered per step:
// a rescan of the raw queens array, or one O(1) lookup in this repo's own
// Set<(int Row, int Col)>. Each is handed the prepared KingBoard its hoisted overload
// takes, so placing the king and sizing the board is charged to [GlobalSetup] rather
// than to the measured search; building the set stays inside the set arm, since that
// one-off cost is exactly what it has to earn back.
//
// Both arms now return LeetCode's actual answer - the attacking queens' coordinates -
// and the harness takes .Count, where previously both counted attackers without
// building the list (§17.8's deliberate-measurement-change note).
[MemoryDiagnoser]
public class QueensThatCanAttackTheKingBenchmarks
{
    private const int BoardSize = 1_000;
    private const int BoardCenter = BoardSize / 2;
    private const int RandomSeed = 1222;

    private int[][] _queens = [];

    private KingBoard _board;
    [Params(50, 500)]
    public int QueensCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _board = new KingBoard(BoardCenter, BoardCenter, BoardSize);

        var coordinates = new HashSet<(int Row, int Col)>();

        while (coordinates.Count < QueensCount)
        {
            var candidate = (random.Next(BoardSize), random.Next(BoardSize));

            if (candidate != (_board.KingRow, _board.KingCol))
            {
                coordinates.Add(candidate);
            }
        }

        _queens = coordinates.Select(c => new[] { c.Row, c.Col }).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearScanRayWalk() =>
        QueensThatCanAttackTheKingSolution.QueensAttackTheKingByLinearScan(_queens, _board).Count;

    [Benchmark]
    public int SetLookupRayWalk() =>
        QueensThatCanAttackTheKingSolution.QueensAttackTheKingBySetLookup(_queens, _board).Count;
}
