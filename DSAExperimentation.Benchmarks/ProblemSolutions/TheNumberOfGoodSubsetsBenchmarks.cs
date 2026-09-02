using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// The Number of Good Subsets (LC 1994): the same (candidateIndex, usedPrimeMask) bitmask
// recursion as TheNumberOfGoodSubsetsTests.cs, run two ways. BruteForceRecursion
// re-explores every (index, mask) pair from scratch on every branch - many different
// take/skip orderings across the ~18 squarefree candidates in [2, 30] reach the identical
// state, so the unmemoized tree revisits it repeatedly. MemoizedRecursion routes the
// identical recursion through this repo's own Memoizer, caching each state once - the
// same brute-force-vs-Memoizer pairing NumberOfWaysToWearDifferentHatsToEachOtherBenchmarks
// already uses for a different bitmask DP. The candidate list itself (~18 squarefree
// values in [2, 30], masked over the 10 primes <= 30) is fixed by the problem's own
// constraints regardless of input size - Length only varies how nums populates each
// candidate's occurrence weight, not the recursion's shape.
[MemoryDiagnoser]
public class TheNumberOfGoodSubsetsBenchmarks
{
    private const int Modulo = 1_000_000_007;
    private const int RandomSeed = 1994;
    private const int MinCandidateValue = 2;
    private const int MaxCandidateValue = 30;
    private const int ValueUpperBoundExclusive = MaxCandidateValue + 1;
    private static readonly int[] Primes = [2, 3, 5, 7, 11, 13, 17, 19, 23, 29];

    [Params(200, 5_000)]
    public int Length;

    private List<(int Mask, int Weight)> _candidates = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var counts = new int[ValueUpperBoundExclusive];

        for (var i = 0; i < Length; i++)
        {
            counts[random.Next(1, ValueUpperBoundExclusive)]++;
        }

        _candidates = BuildSquarefreeCandidates(counts);
    }

    [Benchmark(Baseline = true)]
    public long BruteForceRecursion() => CountWays(0, 0);

    private long CountWays(int index, int usedMask)
    {
        if (index == _candidates.Count)
        {
            return usedMask != 0 ? 1L : 0L;
        }

        var (mask, weight) = _candidates[index];
        var skip = CountWays(index + 1, usedMask);

        if ((usedMask & mask) != 0 || weight == 0)
        {
            return skip;
        }

        var take = (weight * CountWays(index + 1, usedMask | mask)) % Modulo;
        return (skip + take) % Modulo;
    }

    [Benchmark]
    public long MemoizedRecursion() => Memoizer.Memoize<(int Index, int UsedMask), long>((0, 0), (state, waysFor) =>
    {
        var (index, usedMask) = state;

        if (index == _candidates.Count)
        {
            return usedMask != 0 ? 1L : 0L;
        }

        var (mask, weight) = _candidates[index];
        var skip = waysFor((index + 1, usedMask));

        if ((usedMask & mask) != 0 || weight == 0)
        {
            return skip;
        }

        var take = (weight * waysFor((index + 1, usedMask | mask))) % Modulo;
        return (skip + take) % Modulo;
    });

    private static List<(int Mask, int Weight)> BuildSquarefreeCandidates(int[] counts)
    {
        var candidates = new List<(int Mask, int Weight)>();

        for (var value = MinCandidateValue; value <= MaxCandidateValue; value++)
        {
            if (TryComputeSquarefreePrimeMask(value, out var mask))
            {
                candidates.Add((mask, counts[value]));
            }
        }

        return candidates;
    }

    private static bool TryComputeSquarefreePrimeMask(int value, out int mask)
    {
        mask = 0;

        for (var i = 0; i < Primes.Length; i++)
        {
            var prime = Primes[i];
            if (value % prime != 0)
            {
                continue;
            }

            value /= prime;
            if (value % prime == 0)
            {
                mask = 0;
                return false;
            }

            mask |= 1 << i;
        }

        return true;
    }
}
