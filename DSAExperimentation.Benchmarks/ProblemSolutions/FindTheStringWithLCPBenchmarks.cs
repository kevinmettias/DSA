using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheStringWithLCP;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheStringWithLCPSolution's, the same methods
// FindTheStringWithLCPTests proves correct (TwoSumBenchmarks precedent). [GlobalSetup]
// builds a random lowercase string over a small alphabet (so plenty of positions share
// a letter, giving DisjointSet's Union real merging to do) and derives its actual LCP
// matrix from it, guaranteeing a satisfiable input that drives both arms through their
// full algorithm instead of an early "" bail-out.
[MemoryDiagnoser]
public class FindTheStringWithLCPBenchmarks
{
    private const int AlphabetSize = 4;
    private const int Seed = 2573;

    [Params(50, 300)]
    public int Length;

    private int[][] _lcp = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var word = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            word[i] = (char)('a' + random.Next(AlphabetSize));
        }

        _lcp = BuildLcp(word);
    }

    [Benchmark(Baseline = true)]
    public string DirectSweep() => FindTheStringWithLCPSolution.ConstructByDirectSweep(_lcp);

    [Benchmark]
    public string DisjointSet() => FindTheStringWithLCPSolution.ConstructByDisjointSet(_lcp);

    private static int[][] BuildLcp(char[] word)
    {
        var n = word.Length;
        var lcp = new int[n][];

        for (var i = 0; i < n; i++)
        {
            lcp[i] = new int[n];
        }

        for (var i = n - 1; i >= 0; i--)
        {
            for (var j = n - 1; j >= 0; j--)
            {
                var diagonal = i + 1 < n && j + 1 < n ? lcp[i + 1][j + 1] : 0;
                lcp[i][j] = word[i] == word[j] ? diagonal + 1 : 0;
            }
        }

        return lcp;
    }
}
