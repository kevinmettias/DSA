using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.InverseCoinChange;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are InverseCoinChangeSolution's, the same methods
// InverseCoinChangeTests proves correct. _numWays is built by simulating the
// real forward DP over a random denomination set (seeded, so both arms see
// the same reconstructible array every run) rather than random ints, since a
// value neither strategy can ever match would return [] on the very first
// amount and measure almost nothing.
[MemoryDiagnoser]
public class InverseCoinChangeBenchmarks
{
    private const int Seed = 3592;

    [Params(30, 100)]
    public int Length;

    private int[] _numWays = null!;

    [GlobalSetup]
    public void Setup()
    {
        var denominations = BuildDenominations(Length, seed: Seed);
        _numWays = BuildNumWays(Length, denominations);
    }

    [Benchmark(Baseline = true)]
    public int[] ArrayTabulation() => InverseCoinChangeSolution.FindDenominationsByArrayTabulation(_numWays);

    [Benchmark]
    public int[] MemoizedRecurrence() => InverseCoinChangeSolution.FindDenominationsByMemoizedRecurrence(_numWays);

    private static List<int> BuildDenominations(int length, int seed)
    {
        var random = new Random(seed);
        var denominations = new List<int>();

        for (var value = 1; value <= length; value++)
        {
            if (random.Next(3) == 0)
            {
                denominations.Add(value);
            }
        }

        if (denominations.Count == 0)
        {
            denominations.Add(1);
        }

        return denominations;
    }

    private static int[] BuildNumWays(int length, List<int> denominations)
    {
        var ways = new int[length + 1];
        ways[0] = 1;

        foreach (var coin in denominations)
        {
            for (var i = coin; i <= length; i++)
            {
                ways[i] += ways[i - coin];
            }
        }

        return ways[1..];
    }
}
