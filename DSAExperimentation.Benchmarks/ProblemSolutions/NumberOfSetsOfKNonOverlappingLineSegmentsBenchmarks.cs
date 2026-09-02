using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Sets of K Non-Overlapping Line Segments (LC 1621): the problem reduces
// to Pascal's identity C(n+k-1, 2k) mod 1e9+7 (see NumberOfSetsOfKNonOverlappingLine
// SegmentsTests' own doc comment for the derivation). Plain bottom-up Pascal's
// triangle tabulation (baseline) vs. this repo's Memoizer-based top-down recursion
// over the same recurrence - UniqueBinarySearchTreesBenchmarks/CountAllValidPickup
// AndDeliveryOptionsBenchmarks' own tabulation-vs-Memoizer precedent for a
// combinatorial counting recurrence.
[MemoryDiagnoser]
public class NumberOfSetsOfKNonOverlappingLineSegmentsBenchmarks
{
    private const long Modulo = 1_000_000_007;
    private const int PointsToKDivisor = 4;
    private const int TargetMultiplier = 2;

    [Params(50, 500)]
    public int Points;

    private int _k;

    [GlobalSetup]
    public void Setup() => _k = Points / PointsToKDivisor;

    [Benchmark(Baseline = true)]
    public long Tabulation()
    {
        var n = Points + _k - 1;
        var target = TargetMultiplier * _k;
        var table = new long[n + 1, target + 1];

        for (var row = 0; row <= n; row++)
        {
            for (var col = 0; col <= Math.Min(row, target); col++)
            {
                table[row, col] = col == 0 || col == row ? 1 : (table[row - 1, col - 1] + table[row - 1, col]) % Modulo;
            }
        }

        return table[n, target];
    }

    [Benchmark]
    public long Memoized() => Choose(Points + _k - 1, TargetMultiplier * _k);

    private static long Choose(int n, int k) => Memoizer.Memoize<(int N, int K), long>((n, k), ChooseRecurrence);

    private static long ChooseRecurrence((int N, int K) state, Func<(int, int), long> choose)
    {
        var (n, k) = state;

        if (k == 0 || k == n)
        {
            return 1;
        }

        if (k > n)
        {
            return 0;
        }

        return (choose((n - 1, k - 1)) + choose((n - 1, k))) % Modulo;
    }
}
