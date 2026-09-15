using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CheckIfItIsAStraightLine;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CheckIfItIsAStraightLineSolution's, the same methods
// CheckIfItIsAStraightLineTests proves correct. The O(n^3) "check every triple of
// points" brute force vs. the O(n) single-pass cross-product check anchored on the
// first two points. _coordinates is always collinear so BOTH strategies are forced
// through their full worst-case scan instead of an early exit on the first bad
// triple/point making brute force look artificially competitive.
[MemoryDiagnoser]
public class CheckIfItIsAStraightLineBenchmarks
{
    private int[][] _coordinates = [];

    [Params(20, 100)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _coordinates = Enumerable.Range(0, Length).Select(i => new[] { i, i }).ToArray();

    [Benchmark(Baseline = true)]
    public bool BruteForceEveryTriple()
        => CheckIfItIsAStraightLineSolution.CheckStraightLineByBruteForceEveryTriple(_coordinates);

    [Benchmark]
    public bool AnchoredCrossProductScan()
        => CheckIfItIsAStraightLineSolution.CheckStraightLineByAnchoredCrossProductScan(_coordinates);
}
