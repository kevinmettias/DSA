using DSAExperimentation.LeetCode.CombinationSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CombinationSumSolution's, now returning the same
// combinations the test proves correct instead of merely counting them.
public class CombinationSumBenchmarks
{
    private static readonly int[] CandidateValues = [2, 3, 5, 7];

    private int[] _candidates = [];

    // LC 39's target is at most 40, and its tests promise fewer than 150 combinations: these
    // candidates make 45 at 30 and 90 at 40.
    [Params(30, 40)]
    public int Target { get; set; }

    [GlobalSetup]
    public void Setup() => _candidates = CandidateValues;

    [Benchmark(Baseline = true)]
    public List<List<int>> SpecializedRecursive() =>
        CombinationSumSolution.FindCombinationsBySpecializedRecursion(_candidates, Target);

    [Benchmark]
    public List<List<int>> Backtracking() =>
        CombinationSumSolution.FindCombinationsByBacktracking(_candidates, Target);
}
