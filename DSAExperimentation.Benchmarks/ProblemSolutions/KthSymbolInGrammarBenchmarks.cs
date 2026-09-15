using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.KthSymbolInGrammar;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are KthSymbolInGrammarSolution's, the same methods
// KthSymbolInGrammarTests proves correct. N is kept small enough for the row-expansion
// baseline's O(2^n) allocation to stay tractable; K is fixed at the last symbol of the
// row, the deepest possible recursion for the halving walk.
[MemoryDiagnoser]
public class KthSymbolInGrammarBenchmarks
{
    private int _k;

    [Params(10, 20)]
    public int N { get; set; }

    [GlobalSetup]
    public void Setup() => _k = 1 << (N - 1);

    [Benchmark(Baseline = true)]
    public int BuildFullRow() => KthSymbolInGrammarSolution.KthGrammarByRowExpansion(N, _k);

    [Benchmark]
    public int RecursiveHalving() => KthSymbolInGrammarSolution.KthGrammarByRecursiveHalving(N, _k);
}
