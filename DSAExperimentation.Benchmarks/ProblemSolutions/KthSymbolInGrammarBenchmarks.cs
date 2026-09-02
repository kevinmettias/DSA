using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// K-th Symbol in Grammar (LC 779): BuildFullRow materializes the entire O(2^n)
// row and indexes into it (the textbook brute force) vs. RecursiveHalving,
// which never builds more than one symbol per row by walking straight to k's
// ancestor at each level. No repo container or algorithm primitive applies
// here - there is nothing to compose over one running row/index pair, the
// same "lighter repo-primitive fit" case PowXnBenchmarks' exponentiation by
// squaring already is. N is kept small enough for BuildFullRow's O(2^n)
// allocation to stay tractable; K is fixed at the last symbol of the row, the
// deepest possible recursion for RecursiveHalving.
[MemoryDiagnoser]
public class KthSymbolInGrammarBenchmarks
{
    private const int BranchingFactor = 2;

    [Params(10, 20)]
    public int N;

    private int _k;

    [GlobalSetup]
    public void Setup() => _k = 1 << (N - 1);

    [Benchmark(Baseline = true)]
    public int BuildFullRow()
    {
        var row = new List<char> { '0' };

        for (var level = 1; level < N; level++)
        {
            row = BuildNextRow(row);
        }

        return row[_k - 1] - '0';
    }

    private static List<char> BuildNextRow(List<char> row)
    {
        var next = new List<char>(row.Count * BranchingFactor);

        foreach (var symbol in row)
        {
            if (symbol == '0')
            {
                next.Add('0');
                next.Add('1');
            }
            else
            {
                next.Add('1');
                next.Add('0');
            }
        }

        return next;
    }

    [Benchmark]
    public int RecursiveHalving() => KthGrammar(N, _k);

    private static int KthGrammar(int n, int k)
    {
        if (n == 1)
        {
            return 0;
        }

        var parent = KthGrammar(n - 1, (k + 1) / BranchingFactor);
        var isSecondHalfOfParent = k % BranchingFactor == 0;

        return isSecondHalfOfParent ? 1 - parent : parent;
    }
}
