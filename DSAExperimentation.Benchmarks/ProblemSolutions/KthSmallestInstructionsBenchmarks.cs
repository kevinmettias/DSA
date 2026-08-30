using System.Text;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Kth Smallest Instructions (LC 1643): brute-force enumerate every valid H/V
// sequence and sort them vs. this repo's own Memoizer computing the same
// Pascal's-triangle binomial counts UniquePathsTests already uses, to greedily
// pick each character in O((rows+cols)^2) instead of
// O(C(rows+cols, rows) * (rows+cols)).
[MemoryDiagnoser]
public class KthSmallestInstructionsBenchmarks
{
    [Params(5, 8)]
    public int Size;

    private int[] _destination = null!;
    private long _k;

    [GlobalSetup]
    public void Setup()
    {
        _destination = [Size, Size];
        var totalSequences = Binomial(2 * Size, Size);
        _k = totalSequences / 2;
    }

    [Benchmark(Baseline = true)]
    public string EnumerateAndSort()
    {
        var results = new List<string>();
        Generate("", _destination[0], _destination[1], results);
        results.Sort(StringComparer.Ordinal);
        return results[(int)_k - 1];
    }

    [Benchmark]
    public string MemoizedGreedy()
    {
        var remainingV = _destination[0];
        var remainingH = _destination[1];
        var k = _k;
        var path = new StringBuilder();

        while (remainingV > 0 || remainingH > 0)
        {
            if (remainingH == 0)
            {
                path.Append('V');
                remainingV--;
                continue;
            }

            if (remainingV == 0)
            {
                path.Append('H');
                remainingH--;
                continue;
            }

            var waysIfH = Memoizer.Memoize<(int V, int H), long>((remainingV, remainingH - 1), Ways);

            if (k <= waysIfH)
            {
                path.Append('H');
                remainingH--;
            }
            else
            {
                k -= waysIfH;
                path.Append('V');
                remainingV--;
            }
        }

        return path.ToString();
    }

    private static void Generate(string prefix, int remainingV, int remainingH, List<string> results)
    {
        if (remainingV == 0 && remainingH == 0)
        {
            results.Add(prefix);
            return;
        }

        if (remainingH > 0)
        {
            Generate(prefix + "H", remainingV, remainingH - 1, results);
        }

        if (remainingV > 0)
        {
            Generate(prefix + "V", remainingV - 1, remainingH, results);
        }
    }

    private static long Ways((int V, int H) state, Func<(int V, int H), long> ways)
    {
        var (v, h) = state;
        return v == 0 || h == 0 ? 1 : ways((v - 1, h)) + ways((v, h - 1));
    }

    private static long Binomial(int n, int r)
    {
        long result = 1;

        for (var i = 0; i < r; i++)
        {
            result = result * (n - i) / (i + 1);
        }

        return result;
    }
}
