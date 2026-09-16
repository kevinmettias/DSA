using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.KthSymbolInGrammar;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are KthSymbolInGrammarSolution's, the same methods
// KthSymbolInGrammarTests proves correct. RowNumber is kept small enough for the
// row-expansion baseline's O(2^n) allocation to stay tractable; _symbolIndex is fixed
// at the last symbol of the row, the deepest possible recursion for the halving walk.
[MemoryDiagnoser]
public class KthSymbolInGrammarBenchmarks
{
    private int _symbolIndex;

    [Params(10, 20)]
    public int RowNumber { get; set; }

    [GlobalSetup]
    public void Setup() => _symbolIndex = 1 << (RowNumber - 1);

    [Benchmark(Baseline = true)]
    public int BuildFullRow() =>
        KthSymbolInGrammarSolution.KthGrammarByRowExpansion(RowNumber, _symbolIndex);

    [Benchmark]
    public int RecursiveHalving() =>
        KthSymbolInGrammarSolution.KthGrammarByRecursiveHalving(RowNumber, _symbolIndex);
}
