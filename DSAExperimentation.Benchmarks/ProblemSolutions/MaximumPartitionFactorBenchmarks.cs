using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumPartitionFactor;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumPartitionFactorSolution's, the same
// methods MaximumPartitionFactorTests proves correct. _points is a PointCount x
// PointCount integer grid flattened to n points, deterministic and dense enough
// that most pairwise Manhattan distances repeat - the "too close" graph at a
// mid-range threshold has plenty of edges, not the trivial all-isolated case a
// sparse random cloud would produce.
[MemoryDiagnoser]
public class MaximumPartitionFactorBenchmarks
{
    private const int GridWidth = 32;

    private int[][] _points = [];

    [Params(50, 300)]
    public int PointCount { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _points = Enumerable.Range(0, PointCount)
            .Select(i => new[] { i % GridWidth, i / GridWidth })
            .ToArray();

    [Benchmark(Baseline = true)]
    public int LinearScan() => MaximumPartitionFactorSolution.MaxPartitionFactorByLinearScan(_points);

    [Benchmark]
    public int BinarySearchBipartiteCheck() =>
        MaximumPartitionFactorSolution.MaxPartitionFactorByBinarySearchBipartiteCheck(_points);
}
