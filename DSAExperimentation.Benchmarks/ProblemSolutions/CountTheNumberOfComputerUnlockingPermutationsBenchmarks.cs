using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountTheNumberOfComputerUnlockingPermutations;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// CountTheNumberOfComputerUnlockingPermutationsSolution's, the same methods
// CountTheNumberOfComputerUnlockingPermutationsSolutionTests proves correct. The workload is
// always solvable (complexity[0] is the global minimum), so Backtracking explores its
// full, uncollapsed (n-1)! search tree - the case FactorialFormula's O(n) closed form
// exists to replace.
//
// Sizes are per arm. That search tree is factorial, so Backtracking stops at 9
// computers; FactorialFormula runs on to LC 3577's own bound of 10^5, and the two are
// compared at the counts both run.
public class CountTheNumberOfComputerUnlockingPermutationsBenchmarks
{
    private const int ComplexitySeed = 3577;

    private Dictionary<int, int[]> _complexityByCount = [];

    public static IEnumerable<int> BacktrackingSizes => [7, 9];

    public static IEnumerable<int> FactorialFormulaSizes => [.. BacktrackingSizes, 1_000, 100_000];

    // Every computer count any arm runs is built here, outside the timed region; an arm
    // looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _complexityByCount = FactorialFormulaSizes.ToDictionary(
            count => count,
            count => ComputerUnlockingWorkloads.BuildSolvable(count, ComplexitySeed));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BacktrackingSizes))]
    public long Backtracking(int computerCount) =>
        CountTheNumberOfComputerUnlockingPermutationsSolution.CountUnlockingPermutationsByBacktracking(
            _complexityByCount[computerCount]);

    [Benchmark]
    [ArgumentsSource(nameof(FactorialFormulaSizes))]
    public long FactorialFormula(int computerCount) =>
        CountTheNumberOfComputerUnlockingPermutationsSolution.CountUnlockingPermutationsByFactorialFormula(
            _complexityByCount[computerCount]);
}
