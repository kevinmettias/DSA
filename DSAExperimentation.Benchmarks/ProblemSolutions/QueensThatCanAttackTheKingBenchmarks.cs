using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Queens That Can Attack the King (LC 1222): both strategies ray-walk the same 8
// queen-move directions outward from the king until the edge or a queen is reached -
// they differ only in how "is this cell occupied" is answered per ray step.
// LinearScanRayWalk rescans the raw queens array at every step (O(boardSize*queens)
// worst case). SetLookupRayWalk builds this repo's own Set<(int Row,int Col)> once up
// front (the same primitive MinimumAreaRectangleBenchmarks uses for its own corner
// checks) and answers each step in O(1), the same "swap a linear rescan for a hash
// lookup" move that benchmark and TwoSumBenchmarks both make.
[MemoryDiagnoser]
public class QueensThatCanAttackTheKingBenchmarks
{
    private static readonly (int DRow, int DCol)[] Directions =
    [
        (-1, -1), (-1, 0), (-1, 1),
        (0, -1), (0, 1),
        (1, -1), (1, 0), (1, 1),
    ];

    private const int BoardSize = 1_000;

    [Params(50, 500)]
    public int QueensCount;

    private int[][] _queens = null!;
    private (int Row, int Col) _king;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1222);
        _king = (BoardSize / 2, BoardSize / 2);

        var coordinates = new HashSet<(int Row, int Col)>();

        while (coordinates.Count < QueensCount)
        {
            var candidate = (random.Next(BoardSize), random.Next(BoardSize));

            if (candidate != _king)
            {
                coordinates.Add(candidate);
            }
        }

        _queens = coordinates.Select(c => new[] { c.Row, c.Col }).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearScanRayWalk()
    {
        var attackers = 0;

        foreach (var (dRow, dCol) in Directions)
        {
            var row = _king.Row + dRow;
            var col = _king.Col + dCol;

            while (IsInBounds(row, col) && !IsOccupiedByLinearScan(row, col))
            {
                row += dRow;
                col += dCol;
            }

            if (IsInBounds(row, col))
            {
                attackers++;
            }
        }

        return attackers;
    }

    [Benchmark]
    public int SetLookupRayWalk()
    {
        var occupied = new Set<(int Row, int Col)>();

        foreach (var queen in _queens)
        {
            occupied.TryAdd((queen[0], queen[1]));
        }

        var attackers = 0;

        foreach (var (dRow, dCol) in Directions)
        {
            var row = _king.Row + dRow;
            var col = _king.Col + dCol;

            while (IsInBounds(row, col) && !occupied.Has((row, col)))
            {
                row += dRow;
                col += dCol;
            }

            if (IsInBounds(row, col))
            {
                attackers++;
            }
        }

        return attackers;
    }

    private bool IsOccupiedByLinearScan(int row, int col)
    {
        foreach (var queen in _queens)
        {
            if (queen[0] == row && queen[1] == col)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsInBounds(int row, int col) => row >= 0 && row < BoardSize && col >= 0 && col < BoardSize;
}
