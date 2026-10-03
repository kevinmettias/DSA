using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.LongestCommonSubpath;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestCommonSubpathSolution's, the same methods
// LongestCommonSubpathSolutionTests proves correct. [GlobalSetup] builds the paths, which
// are LeetCode's own input shape, so each arm is handed them directly - the
// coordinate compression the hashed arm needs is part of that arm's cost and stays
// inside the measured method, exactly as it was measured before.
//
// The set of cities (LongestCommonSubpathWorkloads) is deliberately tiny relative to the
// path length, so long shared runs really do occur and the binary search has to climb
// rather than bail at length 1 - which is where the O(length) vs O(1) per-window key cost
// separates the two arms.
public class LongestCommonSubpathBenchmarks
{
    private const int PathCount = 3;
    private const int RandomSeed = 1923; // LC 1923 problem number

    private int[][] _paths = [];

    [Params(100, 500)]
    public int PathLength { get; set; }

    [GlobalSetup]
    public void Setup() => _paths = LongestCommonSubpathWorkloads.BuildPaths(PathCount, PathLength, RandomSeed);

    [Benchmark(Baseline = true)]
    public int NaiveKeyedIntersection() =>
        LongestCommonSubpathSolution.LongestCommonSubpathByNaiveWindowKeys(_paths);

    [Benchmark]
    public int RollingHashBinarySearch() =>
        LongestCommonSubpathSolution.LongestCommonSubpathByRollingHash(_paths);
}
