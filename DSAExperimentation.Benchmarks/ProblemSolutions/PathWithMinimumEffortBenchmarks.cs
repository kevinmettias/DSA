using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PathWithMinimumEffort;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PathWithMinimumEffortSolution's, the same methods
// PathWithMinimumEffortTests proves correct. Heights are random in [0, 10^6),
// matching LeetCode's own constraint range, so equal-height ties are effectively
// absent and the binary search arm pays for a full re-scan per candidate.
[MemoryDiagnoser]
public class PathWithMinimumEffortBenchmarks
{
    private const int RandomSeed = 1631; // LC 1631: Path With Minimum Effort

    private const int HeightUpperBoundExclusive = 1_000_000;

    [Params(15, 40)]
    public int Size;

    private int[][] _heights = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _heights = new int[Size][];

        for (var r = 0; r < Size; r++)
        {
            _heights[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                _heights[r][c] = random.Next(0, HeightUpperBoundExclusive);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int BinarySearchFloodFill() =>
        PathWithMinimumEffortSolution.MinimumEffortPathByBinarySearchFloodFill(_heights);

    [Benchmark]
    public int HeapDijkstra() =>
        PathWithMinimumEffortSolution.MinimumEffortPathByHeapDijkstra(_heights);
}
