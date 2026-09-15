using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.RemoveOutermostParentheses;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveOutermostParenthesesSolution's, the same methods
// RemoveOutermostParenthesesTests proves correct - a plain running-depth counter
// (baseline) against this repo's own Stack<char> holding each unmatched opener.
// Neither needs anything hoisted beyond the string LeetCode itself hands in, so
// [GlobalSetup] only sizes and seeds the balanced expression.
[MemoryDiagnoser]
public class RemoveOutermostParenthesesBenchmarks
{
    private const int MaxDepth = 10;

    private const int RandomSeed = 1021; private string _expression = "";

    // LeetCode problem number

    [Params(1_000, 20_000)]
    public int PairCount { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _expression = ScoreOfParenthesesWorkloads.BuildBalanced(PairCount, MaxDepth, seed: RandomSeed);

    [Benchmark(Baseline = true)]
    public string RunningDepthCounter() =>
        RemoveOutermostParenthesesSolution.RemoveOuterParenthesesByDepthCounter(_expression);

    [Benchmark]
    public string StackOfOpeners() =>
        RemoveOutermostParenthesesSolution.RemoveOuterParenthesesByOpenerStack(_expression);
}
