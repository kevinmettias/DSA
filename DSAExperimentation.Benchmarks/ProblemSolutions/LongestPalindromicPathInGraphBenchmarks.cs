using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.LongestPalindromicPathInGraph;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestPalindromicPathInGraphSolution's, the same
// methods LongestPalindromicPathInGraphTests proves correct. Both are handed a
// pre-built LabeledGraph so adjacency-mask construction is charged to
// [GlobalSetup] rather than to the search being measured. NodeCount is kept well
// under the problem's own n <= 14 ceiling - the brute-force baseline's simple-path
// enumeration is genuinely exponential, and this is exactly the comparison that's
// meant to show.
[MemoryDiagnoser]
public class LongestPalindromicPathInGraphBenchmarks
{
    private const int GraphSeed = 3615;
    private const int ExtraEdgesPerNode = 1;

    private LabeledGraph _graph = null!;

    [Params(8, 12)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var (edges, label) = LabeledGraphWorkloads.Build(NodeCount, ExtraEdgesPerNode, GraphSeed);
        _graph = LabeledGraph.Build(NodeCount, edges, label);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceDfs() =>
        LongestPalindromicPathInGraphSolution.LongestPalindromeByBruteForceDfs(_graph);

    [Benchmark]
    public int BitmaskMemo() =>
        LongestPalindromicPathInGraphSolution.LongestPalindromeByBitmaskMemo(_graph);
}
