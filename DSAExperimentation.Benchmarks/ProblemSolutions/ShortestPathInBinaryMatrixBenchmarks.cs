using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ShortestPathInBinaryMatrix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ShortestPathInBinaryMatrixSolution's, the same
// methods ShortestPathInBinaryMatrixTests proves correct - the 8-directional BFS
// over the BCL's Queue<T> against the identical sweep over this repo's own
// Queue<TElement>. Grid cells are blocked with low enough probability that a clear
// corner-to-corner path almost always exists, so both strategies do comparable real
// BFS work instead of failing fast. Grid construction is charged to [GlobalSetup],
// and the prepared grid is already LeetCode's own input shape.
[MemoryDiagnoser]
public class ShortestPathInBinaryMatrixBenchmarks
{
    // 1-in-10 chance a cell is blocked.
    private const int BlockedCellProbability = 10;

    [Params(10, 25)]
    public int Size;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _grid = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, BlockedCellProbability) == 0 ? 1 : 0).ToArray())
            .ToArray();
        _grid[0][0] = 0;
        _grid[Size - 1][Size - 1] = 0;
    }

    [Benchmark(Baseline = true)]
    public int BclQueueBfs() =>
        ShortestPathInBinaryMatrixSolution.ShortestPathBinaryMatrixByBclQueue(_grid);

    [Benchmark]
    public int RepoQueueBfs() =>
        ShortestPathInBinaryMatrixSolution.ShortestPathBinaryMatrixByQueueFrontier(_grid);
}
