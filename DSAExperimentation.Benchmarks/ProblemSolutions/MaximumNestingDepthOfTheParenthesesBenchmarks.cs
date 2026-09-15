using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaximumNestingDepthOfTheParentheses;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumNestingDepthOfTheParenthesesSolution's, the
// same methods MaximumNestingDepthOfTheParenthesesTests proves correct - a plain
// running-depth counter (baseline) against this repo's own Stack<char> holding each
// unmatched opener, the same pair RemoveOutermostParenthesesBenchmarks compares for
// a sibling parentheses-depth problem. Neither needs anything hoisted beyond the
// string LeetCode itself hands in, so [GlobalSetup] only sizes and seeds the
// balanced expression, reusing the generator LC 856 and LC 1021 already measure with.
[MemoryDiagnoser]
public class MaximumNestingDepthOfTheParenthesesBenchmarks
{
    private const int MaxDepthCap = 20;

    private const int RandomSeed = 1614; private string _expression = "";

    // LeetCode problem number

    [Params(1_000, 20_000)]
    public int PairCount { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _expression = ScoreOfParenthesesWorkloads.BuildBalanced(PairCount, MaxDepthCap, seed: RandomSeed);

    [Benchmark(Baseline = true)]
    public int RunningDepthCounter() =>
        MaximumNestingDepthOfTheParenthesesSolution.MaxDepthByRunningCounter(_expression);

    [Benchmark]
    public int StackOfOpeners() =>
        MaximumNestingDepthOfTheParenthesesSolution.MaxDepthByOpenerStack(_expression);
}
