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
    private const int TotalStepsMultiplier = 2; // destination is [Size, Size], so total steps = 2 * Size

    private const int MedianSequenceDivisor = 2;

    private const string EmptyInstructionPrefix = "";

    private const string HorizontalInstruction = "H";

    private const string VerticalInstruction = "V";

    [Params(5, 8)]
    public int Size;

    private int[] _destination = null!;
    private long _k;

    [GlobalSetup]
    public void Setup()
    {
        _destination = [Size, Size];
        var totalSequences = Binomial(TotalStepsMultiplier * Size, Size);
        _k = totalSequences / MedianSequenceDivisor;
    }

    [Benchmark(Baseline = true)]
    public string EnumerateAndSort()
    {
        var results = new List<string>();
        Generate(EmptyInstructionPrefix, _destination[0], _destination[1], results);
        results.Sort(StringComparer.Ordinal);
        return results[(int)_k - 1];
    }

    private readonly record struct GreedyState(int RemainingV, int RemainingH, long K);

    [Benchmark]
    public string MemoizedGreedy()
    {
        var state = new GreedyState(_destination[0], _destination[1], _k);
        var path = new StringBuilder();

        while (state.RemainingV > 0 || state.RemainingH > 0)
        {
            state = AppendNextInstruction(state, path);
        }

        return path.ToString();
    }

    private static GreedyState AppendNextInstruction(GreedyState state, StringBuilder path)
    {
        if (state.RemainingH == 0)
        {
            path.Append('V');
            return state with { RemainingV = state.RemainingV - 1 };
        }

        if (state.RemainingV == 0)
        {
            path.Append('H');
            return state with { RemainingH = state.RemainingH - 1 };
        }

        var waysIfH = Memoizer.Memoize<(int V, int H), long>((state.RemainingV, state.RemainingH - 1), Ways);

        if (state.K <= waysIfH)
        {
            path.Append('H');
            return state with { RemainingH = state.RemainingH - 1 };
        }

        path.Append('V');
        return state with { RemainingV = state.RemainingV - 1, K = state.K - waysIfH };
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
            Generate(prefix + HorizontalInstruction, remainingV, remainingH - 1, results);
        }

        if (remainingV > 0)
        {
            Generate(prefix + VerticalInstruction, remainingV - 1, remainingH, results);
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
