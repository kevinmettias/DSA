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

    private int[][] _lcp = [];

    [Params(50, 300)]
    public int Length { get; set; }

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
                var hasDiagonalNeighbor = i + 1 < n && j + 1 < n;
                var lettersMatch = word[i] == word[j];
                var diagonal = hasDiagonalNeighbor ? DiagonalLcp(lcp, i, j) : 0;
                lcp[i][j] = lettersMatch ? ExtendedLcp(diagonal) : 0;
            }
        }

        return lcp;
    }

    private static int DiagonalLcp(int[][] lcp, int rowIndex, int columnIndex) => lcp[rowIndex + 1][columnIndex + 1];

    private static int ExtendedLcp(int diagonal) => diagonal + 1;

    [Benchmark(Baseline = true)]
    public string DirectSweep() => FindTheStringWithLCPSolution.ConstructByDirectSweep(_lcp);

    [Benchmark]
    public string DisjointSet() => FindTheStringWithLCPSolution.ConstructByDisjointSet(_lcp);
}
