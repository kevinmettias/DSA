using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ScoreOfParentheses;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ScoreOfParenthesesSolution's, the same methods
// ScoreOfParenthesesTests proves correct. The O(n^2) depth rescan is the baseline
// the O(n) sentinel-seeded Stack<int> fold is measured against; string generation
// is charged to [GlobalSetup], and the generated string is already LeetCode's own
// input shape so no hoisted overload is needed.
[MemoryDiagnoser]
public class ScoreOfParenthesesBenchmarks
{
    // Nesting depth cap - see ScoreOfParenthesesWorkloads for why it exists.
    private const int MaxDepth = 10;

    // LC problem number, used as the RNG seed.
    private const int RandomSeed = 856;

    [Params(100, 2_000)]
    public int PairCount;

    private string _expression = null!;

    [GlobalSetup]
    public void Setup() =>
        _expression = ScoreOfParenthesesWorkloads.BuildBalanced(PairCount, MaxDepth, seed: RandomSeed);

    [Benchmark(Baseline = true)]
    public int NestedDepthScan() =>
        ScoreOfParenthesesSolution.ScoreByNestedDepthScan(_expression);

    [Benchmark]
    public int MonotonicStackFold() =>
        ScoreOfParenthesesSolution.ScoreByMonotonicStackFold(_expression);
}
