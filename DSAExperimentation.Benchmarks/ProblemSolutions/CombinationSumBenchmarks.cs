using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CombinationSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CombinationSumSolution's, now returning the same
// combinations the test proves correct instead of merely counting them.
[MemoryDiagnoser]
public class CombinationSumBenchmarks
{
    private static readonly int[] CandidateValues = [2, 3, 5, 7];

    private int[] _candidates = [];

    [Params(30, 60)]
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
