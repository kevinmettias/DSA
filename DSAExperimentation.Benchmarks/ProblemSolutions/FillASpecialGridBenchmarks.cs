using DSAExperimentation.LeetCode.FillASpecialGrid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FillASpecialGridSolution's, the same methods
// FillASpecialGridSolutionTests proves correct. Both visit exactly size^2 cells - the gap
// is recursion/allocation overhead against a closed-form per-cell computation,
// not algorithm class.
public class FillASpecialGridBenchmarks
{
    [Params(5, 9)]
    public int LevelCount { get; set; }

    [Benchmark(Baseline = true)]
    public int[][] RecursiveQuadrants() => FillASpecialGridSolution.SpecialGridByRecursiveQuadrants(LevelCount);

    [Benchmark]
    public int[][] BitQuadrantDigits() => FillASpecialGridSolution.SpecialGridByBitQuadrantDigits(LevelCount);
}
