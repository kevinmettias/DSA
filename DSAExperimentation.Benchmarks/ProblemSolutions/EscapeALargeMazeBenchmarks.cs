using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Escape a Large Maze (LC 1036): a full unbounded flood fill of the whole board
// (O(BoardSize^2), the only option once the board is actually materialized as a
// bool[,] visited grid) vs. this repo's own DepthFirstSearch.Traverse over an
// implicit Set<(int,int)>-backed successor closure, capped at
// BlockedCount*(BlockedCount-1)/2 visited cells - independent of BoardSize
// entirely, since that's the largest pocket the fixed, small blocked set could ever
// wall off. BoardSize stands in for LC1036's real 10^6 bound (a full flood fill at
// the real scale would never finish); growing it here while BlockedCount stays
// fixed is exactly what makes the capped primitive's board-size independence
// visible against the naive baseline's O(BoardSize^2) growth.
[MemoryDiagnoser]
public class EscapeALargeMazeBenchmarks
{
    private const int BlockedCount = 40;
    private const int RandomSeed = 1036; // LC problem number
    private const int BoardMargin = 2;
    private const int PairCountDivisor = 2; // n choose 2: n * (n - 1) / PairCountDivisor
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    [Params(500, 2_000)]
    public int BoardSize;

    private (int Row, int Col)[] _blockedCells = null!;
    private (int Row, int Col) _source;
    private (int Row, int Col) _target;

    private readonly record struct FloodFillState(bool[,] Blocked, bool[,] Visited, Stack<(int Row, int Col)> Stack);

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var blockedSet = new Set<(int Row, int Col)>();
        var blockedList = new List<(int Row, int Col)>();

        // Scatter blocked cells away from both corners so neither source nor target
        // is ever sealed in - both algorithms below are expected to agree: true.
        while (blockedList.Count < BlockedCount)
        {
            var cell = (Row: random.Next(BoardMargin, BoardSize - BoardMargin), Col: random.Next(BoardMargin, BoardSize - BoardMargin));

            if (blockedSet.TryAdd(cell))
            {
                blockedList.Add(cell);
            }
        }

        _blockedCells = blockedList.ToArray();
        _source = (Row: 0, Col: 0);
        _target = (Row: BoardSize - 1, Col: BoardSize - 1);
    }

    [Benchmark(Baseline = true)]
    public bool FullBoardFloodFill()
    {
        var blocked = new bool[BoardSize, BoardSize];

        foreach (var (row, col) in _blockedCells)
        {
            blocked[row, col] = true;
        }

        var visited = new bool[BoardSize, BoardSize];
        var stack = new Stack<(int Row, int Col)>();
        stack.Push(_source);
        visited[_source.Row, _source.Col] = true;

        var state = new FloodFillState(blocked, visited, stack);

        while (stack.Count > 0)
        {
            VisitNeighbors(state, stack.Pop());
        }

        return visited[_target.Row, _target.Col];
    }

    private void VisitNeighbors(FloodFillState state, (int Row, int Col) cell)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            var nextRow = cell.Row + dRow;
            var nextCol = cell.Col + dCol;

            if (!InBounds(nextRow, nextCol))
            {
                continue;
            }

            if (state.Visited[nextRow, nextCol] || state.Blocked[nextRow, nextCol])
            {
                continue;
            }

            state.Visited[nextRow, nextCol] = true;
            state.Stack.Push((nextRow, nextCol));
        }
    }

    [Benchmark]
    public bool CappedTraversalWithSetPrimitive()
    {
        var blocked = new Set<(int Row, int Col)>();

        foreach (var cell in _blockedCells)
        {
            blocked.TryAdd(cell);
        }

        var threshold = BlockedCount * (BlockedCount - 1) / PairCountDivisor;

        return CanEscapeOrReach(_source, _target, blocked, threshold)
            && CanEscapeOrReach(_target, _source, blocked, threshold);
    }

    private bool CanEscapeOrReach(
        (int Row, int Col) start, (int Row, int Col) other, Set<(int Row, int Col)> blocked, int threshold)
    {
        var visitedCount = 0;

        var reached = DepthFirstSearch.Traverse(start, Successors);

        return reached.Count > threshold || reached.Contains(other);

        IEnumerable<(int Row, int Col)> Successors((int Row, int Col) cell)
        {
            visitedCount++;

            if (visitedCount > threshold)
            {
                yield break;
            }

            foreach (var next in UnblockedNeighbors(cell, blocked))
            {
                yield return next;
            }
        }
    }

    private IEnumerable<(int Row, int Col)> UnblockedNeighbors((int Row, int Col) cell, Set<(int Row, int Col)> blocked)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            var next = (Row: cell.Row + dRow, Col: cell.Col + dCol);

            if (!InBounds(next.Row, next.Col))
            {
                continue;
            }

            if (blocked.Has(next))
            {
                continue;
            }

            yield return next;
        }
    }

    private bool InBounds(int row, int col) => row >= 0 && row < BoardSize && col >= 0 && col < BoardSize;
}
