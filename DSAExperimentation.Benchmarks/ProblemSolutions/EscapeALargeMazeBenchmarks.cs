using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.LeetCode.EscapeALargeMaze;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are EscapeALargeMazeSolution's - a full unbounded flood
// fill of the whole board (O(BoardSize^2), the only option once the board is
// actually materialized as a bool[,] visited grid) against the capped
// DepthFirstSearch.Traverse over an implicit Set<(int, int)>-backed successor
// closure, which is independent of BoardSize entirely.
//
// BoardSize stands in for LC 1036's real 10^6 bound (a full flood fill at the real
// scale would never finish); growing it here while BlockedCount stays fixed is what
// makes the capped strategy's board-size independence visible against the baseline's
// O(BoardSize^2) growth.
[MemoryDiagnoser]
public class EscapeALargeMazeBenchmarks
{
    private const int BlockedCount = 40;
    private const int RandomSeed = 1036; // LC problem number
    private const int BoardMargin = 2;

    private (int Row, int Col)[] _blockedCells = [];

    private Set<(int Row, int Col)> _blocked = new();
    private (int Row, int Col) _source;
    private (int Row, int Col) _target;
    [Params(500, 2_000)]
    public int BoardSize { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var blocked = new Set<(int Row, int Col)>();
        var blockedList = new List<(int Row, int Col)>();

        // Scatter blocked cells away from both corners so neither source nor target
        // is ever sealed in - both arms below are expected to agree: true.
        while (blockedList.Count < BlockedCount)
        {
            var cell = (Row: random.Next(BoardMargin, BoardSize - BoardMargin), Col: random.Next(BoardMargin, BoardSize - BoardMargin));

            if (blocked.TryAdd(cell))
            {
                blockedList.Add(cell);
            }
        }

        _blocked = blocked;
        _blockedCells = [.. blockedList];
        _source = (Row: 0, Col: 0);
        _target = (Row: BoardSize - 1, Col: BoardSize - 1);
    }

    [Benchmark(Baseline = true)]
    public bool FullBoardFloodFill() =>
        EscapeALargeMazeSolution.CanEscapeByFullBoardFloodFill(_blockedCells, _source, _target, BoardSize);

    [Benchmark]
    public bool CappedTraversalWithSetPrimitive() =>
        EscapeALargeMazeSolution.CanEscapeByCappedTraversal(_blocked, _source, _target, BoardSize);
}
