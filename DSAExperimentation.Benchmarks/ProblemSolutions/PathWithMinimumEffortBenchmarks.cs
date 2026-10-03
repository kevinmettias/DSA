using DSAExperimentation.LeetCode.PathWithMinimumEffort;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PathWithMinimumEffortSolution's, the same methods
// PathWithMinimumEffortSolutionTests proves correct. Heights are random in [1, 10^6),
// inside LeetCode's own [1, 10^6] constraint range, so equal-height ties are effectively
// absent and the binary search arm pays for a full re-scan per candidate.
public class PathWithMinimumEffortBenchmarks
{
    private const int RandomSeed = 1631; // LC 1631: Path With Minimum Effort

    private const int MinHeight = 1;
    private const int HeightUpperBoundExclusive = 1_000_000;

    private int[][] _heights = [];

    [Params(15, 40)]
    public int Size { get; set; }

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
                _heights[r][c] = random.Next(MinHeight, HeightUpperBoundExclusive);
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
