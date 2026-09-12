using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SwimInRisingWater;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SwimInRisingWaterSolution's, the same methods
// SwimInRisingWaterTests proves correct. Grid values are a random permutation
// of 0..n*n-1, matching the problem's own constraint that every elevation from
// 0 to n^2-1 appears exactly once.
[MemoryDiagnoser]
public class SwimInRisingWaterBenchmarks
{
    private const int RandomSeed = 778; // LC 778: Swim in Rising Water

    [Params(15, 40)]
    public int Size;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var values = Enumerable.Range(0, Size * Size).OrderBy(_ => random.Next()).ToArray();
        _grid = new int[Size][];

        for (var r = 0; r < Size; r++)
        {
            _grid[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                _grid[r][c] = values[(r * Size) + c];
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int BinarySearchFloodFill() => SwimInRisingWaterSolution.MinTimeByBinarySearchFloodFill(_grid);

    [Benchmark]
    public int HeapDijkstra() => SwimInRisingWaterSolution.MinTimeByHeapDijkstra(_grid);
}
