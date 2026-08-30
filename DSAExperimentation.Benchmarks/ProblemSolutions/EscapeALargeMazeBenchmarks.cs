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
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    [Params(500, 2_000)]
    public int BoardSize;

    private (int Row, int Col)[] _blockedCells = null!;
    private (int Row, int Col) _source;
    private (int Row, int Col) _target;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1036);
        var blockedSet = new Set<(int Row, int Col)>();
        var blockedList = new List<(int Row, int Col)>();

        // Scatter blocked cells away from both corners so neither source nor target
        // is ever sealed in - both algorithms below are expected to agree: true.
        while (blockedList.Count < BlockedCount)
        {
            var cell = (Row: random.Next(2, BoardSize - 2), Col: random.Next(2, BoardSize - 2));

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

        while (stack.Count > 0)
        {
            var (row, col) = stack.Pop();

            foreach (var (dRow, dCol) in Directions)
            {
                var nextRow = row + dRow;
                var nextCol = col + dCol;

                if (nextRow < 0 || nextRow >= BoardSize || nextCol < 0 || nextCol >= BoardSize)
                {
                    continue;
                }

                if (visited[nextRow, nextCol] || blocked[nextRow, nextCol])
                {
                    continue;
                }

                visited[nextRow, nextCol] = true;
                stack.Push((nextRow, nextCol));
            }
        }

        return visited[_target.Row, _target.Col];
    }

    [Benchmark]
    public bool CappedTraversalWithSetPrimitive()
    {
        var blocked = new Set<(int Row, int Col)>();

        foreach (var cell in _blockedCells)
        {
            blocked.TryAdd(cell);
        }

        var threshold = BlockedCount * (BlockedCount - 1) / 2;

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

            foreach (var (dRow, dCol) in Directions)
            {
                var next = (Row: cell.Row + dRow, Col: cell.Col + dCol);

                if (next.Row < 0 || next.Row >= BoardSize || next.Col < 0 || next.Col >= BoardSize)
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
    }
}
