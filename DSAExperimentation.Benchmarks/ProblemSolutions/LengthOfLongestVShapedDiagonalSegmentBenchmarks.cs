using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.LengthOfLongestVShapedDiagonalSegment;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LengthOfLongestVShapedDiagonalSegmentSolution's, the
// same methods LengthOfLongestVShapedDiagonalSegmentTests proves correct. Neither
// strategy has a separable construction step to hoist - the grid itself is the
// whole input - so [GlobalSetup] only builds the workload grid, same as
// DigitGridWorkloads' consumers.
[MemoryDiagnoser]
public class LengthOfLongestVShapedDiagonalSegmentBenchmarks
{
    // LC problem number, reused as the deterministic grid seed.
    private const int GridSeed = 3459;

    private int[][] _grid = [];

    [Params(10, 40)]
    public int GridSize { get; set; }

    [GlobalSetup]
    public void Setup() => _grid = VShapedDiagonalGridWorkloads.BuildGrid(GridSize, seed: GridSeed);

    [Benchmark(Baseline = true)]
    public int BruteForceWalk() => LengthOfLongestVShapedDiagonalSegmentSolution.LongestLengthByBruteForceWalk(_grid);

    [Benchmark]
    public int DirectionalDp() => LengthOfLongestVShapedDiagonalSegmentSolution.LongestLengthByDirectionalDp(_grid);
}
