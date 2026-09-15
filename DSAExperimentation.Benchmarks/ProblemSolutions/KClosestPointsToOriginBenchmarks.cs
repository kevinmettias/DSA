using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.KClosestPointsToOrigin;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are KClosestPointsToOriginSolution's, the same methods
// KClosestPointsToOriginTests proves correct. The point cloud is generated once in
// [GlobalSetup]; it is already LeetCode's own input shape, so each arm is handed it
// directly and only the selection is measured - a full O(n log n) sort of every point
// against an O(n log k) size-k max-heap that never orders more than k of them.
[MemoryDiagnoser]
public class KClosestPointsToOriginBenchmarks
{
    private const int K = 10;
    private const int RandomSeed = 973; // LC problem number
    private const int CoordinateBound = 10_000;

    private int[][] _points = [];

    [Params(1_000, 50_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _points = Enumerable.Range(0, Length)
            .Select(_ => new[] { random.Next(-CoordinateBound, CoordinateBound), random.Next(-CoordinateBound, CoordinateBound) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[][] FullSort() =>
        KClosestPointsToOriginSolution.KClosestByFullSort(_points, K);

    [Benchmark]
    public int[][] SizeKMaxHeap() =>
        KClosestPointsToOriginSolution.KClosestBySizeKMaxHeap(_points, K);
}
