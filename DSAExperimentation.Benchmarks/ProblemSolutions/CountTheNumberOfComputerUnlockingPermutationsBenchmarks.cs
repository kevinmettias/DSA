using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountTheNumberOfComputerUnlockingPermutations;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// CountTheNumberOfComputerUnlockingPermutationsSolution's, the same methods
// CountTheNumberOfComputerUnlockingPermutationsTests proves correct. The workload is
// always solvable (complexity[0] is the global minimum), so Backtracking explores its
// full, uncollapsed (n-1)! search tree - the case FactorialFormula's O(n) closed form
// exists to replace.
[MemoryDiagnoser]
public class CountTheNumberOfComputerUnlockingPermutationsBenchmarks
{
    private const int ComplexitySeed = 3577;

    private int[] _complexity = [];

    [Params(7, 9)]
    public int ComputerCount { get; set; }

    [GlobalSetup]
    public void Setup() => _complexity = ComputerUnlockingWorkloads.BuildSolvable(ComputerCount, ComplexitySeed);

    [Benchmark(Baseline = true)]
    public long Backtracking() =>
        CountTheNumberOfComputerUnlockingPermutationsSolution.CountUnlockingPermutationsByBacktracking(_complexity);

    [Benchmark]
    public long FactorialFormula() =>
        CountTheNumberOfComputerUnlockingPermutationsSolution.CountUnlockingPermutationsByFactorialFormula(_complexity);
}
