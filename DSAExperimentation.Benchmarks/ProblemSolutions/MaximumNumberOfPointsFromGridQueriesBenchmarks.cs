using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Number of Points From Grid Queries (LC 2503): the problem's own
// per-query re-simulation (a fresh 4-directional flood fill from (0,0), bounded
// by that query's threshold, for every query independently - O(QueriesCount *
// Rows*Cols)) vs. this repo's single shared flood fill that sorts queries
// ascending and drains a min-heap frontier (Heap<Element,TOrder> closed over
// ByPriorityOrder<GridNode,int>, over Graph.Grids' GridNode/GridChildren/
// GridTopology for neighbor arithmetic) exactly once, in O(Rows*Cols*log(Rows*
// Cols) + QueriesCount*log QueriesCount) total. The grid is fixed-size; only
// QueriesCount grows, so the baseline's cost scales linearly with it while the
// heap-based one barely moves - the same "one shared pass amortizes across many
// queries" shape ReconstructItineraryBenchmarks and DeleteGreatestValueInEach-
// RowBenchmarks already demonstrate for their own problems.
[MemoryDiagnoser]
public class MaximumNumberOfPointsFromGridQueriesBenchmarks
{
    private const int RandomSeed = 2503;
    private const int Side = 60;
    private const int ValueBound = 1_000_000;

    [Params(50, 1_000)]
    public int QueriesCount;

    private int[][] _grid = null!;
    private int[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _grid = Enumerable.Range(0, Side)
            .Select(_ => Enumerable.Range(0, Side).Select(_ => random.Next(ValueBound)).ToArray())
            .ToArray();

        _queries = Enumerable.Range(0, QueriesCount).Select(_ => random.Next(ValueBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] FloodFillPerQuery()
    {
        var answers = new int[_queries.Length];

        for (var i = 0; i < _queries.Length; i++)
        {
            answers[i] = FloodFillCount(_queries[i]);
        }

        return answers;
    }

    private int FloodFillCount(int query)
    {
        if (_grid[0][0] >= query)
        {
            return 0;
        }

        var visited = new bool[Side, Side];
        var stack = new Stack<(int Row, int Col)>();
        visited[0, 0] = true;
        stack.Push((0, 0));
        var points = 0;

        while (stack.Count > 0)
        {
            var (row, col) = stack.Pop();
            points++;

            foreach (var (dRow, dCol) in Directions)
            {
                var (nextRow, nextCol) = (row + dRow, col + dCol);

                if (nextRow >= 0 && nextRow < Side && nextCol >= 0 && nextCol < Side
                    && !visited[nextRow, nextCol] && _grid[nextRow][nextCol] < query)
                {
                    visited[nextRow, nextCol] = true;
                    stack.Push((nextRow, nextCol));
                }
            }
        }

        return points;
    }

    private static readonly (int, int)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    [Benchmark]
    public int[] MinHeapFloodFill()
    {
        var grid = new Grid(AllPassable(Side, Side));
        var start = new GridNode(0, 0, grid);
        var visited = new HashSet<GridNode> { start };
        var frontier = new Heap<(GridNode Node, int Priority), ByPriorityOrder<GridNode, int>>();
        frontier.Push((start, _grid[0][0]));

        var order = _queries
            .Select((value, index) => (Value: value, Index: index))
            .OrderBy(query => query.Value);

        var answers = new int[_queries.Length];
        var points = 0;

        foreach (var query in order)
        {
            while (frontier.TryPeek(out var top) && top.Priority < query.Value)
            {
                frontier.TryPop(out var current);
                points++;

                var children = GridTopology.GetChildren(current.Node);

                for (var i = 0; i < children.Count; i++)
                {
                    var neighbor = children.Get(i);

                    if (visited.Add(neighbor))
                    {
                        frontier.Push((neighbor, _grid[neighbor.Row][neighbor.Col]));
                    }
                }
            }

            answers[query.Index] = points;
        }

        return answers;
    }

    private static bool[,] AllPassable(int rows, int cols)
    {
        var passable = new bool[rows, cols];

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                passable[row, col] = true;
            }
        }

        return passable;
    }
}
