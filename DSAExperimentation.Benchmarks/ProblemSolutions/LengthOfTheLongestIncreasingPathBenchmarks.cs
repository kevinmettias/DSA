using DSAExperimentation.LeetCode.Solutions.LengthOfTheLongestIncreasingPath;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LengthOfTheLongestIncreasingPathSolution's, the same
// methods CatalogCaseTests proves correct. BruteForce is O(n^2),
// so PointCount stays modest enough for it to still finish - SegmentTreeSweep is the
// O(n log n) arm this problem's own 10^5 bound actually needs.
public class LengthOfTheLongestIncreasingPathBenchmarks
{
    private const int RandomSeed = 3288; // LeetCode problem number
    private const int CoordinateBound = 1_000_000;

    private int[][] _coordinates = [];

    private int _requiredIndex;
    [Params(200, 1_000)]
    public int PointCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        // Kept in the order drawn, the set only checking membership: a HashSet's order follows the
        // points' hashes, which .NET seeds afresh in every process, so taking its order would give each
        // benchmark process a different workload from the same seed.
        var seen = new HashSet<(int X, int Y)>();
        var drawn = new List<int[]>(PointCount);

        while (drawn.Count < PointCount)
        {
            var point = (X: random.Next(CoordinateBound), Y: random.Next(CoordinateBound));
            var isNewPoint = seen.Add(point);

            if (isNewPoint)
            {
                drawn.Add([point.X, point.Y]);
            }
        }

        _coordinates = [.. drawn];
        _requiredIndex = PointCount / 2;
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => LengthOfTheLongestIncreasingPathSolution.MaxPathLengthByBruteForce(
        _coordinates, _requiredIndex);

    [Benchmark]
    public int SegmentTreeSweep() => LengthOfTheLongestIncreasingPathSolution.MaxPathLengthBySegmentTree(
        _coordinates, _requiredIndex);
}
