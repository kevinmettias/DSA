using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// The kth Factor of n (LC 1492): the textbook full-range trial division from 1..n vs.
// this repo's own BinarySearch.LowerBound anchoring at floor(sqrt(n)) - the same
// technique FourDivisorsBenchmarks/ClosestDivisorsBenchmarks use - then a short
// divisor walk in sorted order that stops the moment the kth factor is found. K is
// fixed above the largest possible divisor count for any n in range (840 has the
// most divisors below 1000, at 32) so both strategies are forced through their full
// worst-case scan instead of an early return on the first factor making brute force
// look artificially competitive - the same "unreachable target" idea
// TwoSumBenchmarks uses.
[MemoryDiagnoser]
public class TheKthFactorOfNBenchmarks
{
    private const int UnreachableK = 40;
    private const int RandomSeed = 1492; // LC problem number
    private const int ValueUpperBoundExclusive = 1_000;

    [Params(1_000, 10_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, ValueUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int FullRangeScan()
    {
        var total = 0;

        foreach (var n in _values)
        {
            var factor = FullRangeKthFactor(n, UnreachableK);
            total += factor == -1 ? 0 : factor;
        }

        return total;
    }

    [Benchmark]
    public int BinarySearchAnchored()
    {
        var total = 0;

        foreach (var n in _values)
        {
            var factor = AnchoredKthFactor(n, UnreachableK);
            total += factor == -1 ? 0 : factor;
        }

        return total;
    }

    private static int FullRangeKthFactor(int n, int k)
    {
        var remaining = k;

        for (var divisor = 1; divisor <= n; divisor++)
        {
            if (n % divisor == 0 && --remaining == 0)
            {
                return divisor;
            }
        }

        return -1;
    }

    private static int AnchoredKthFactor(int n, int k)
    {
        var sequence = new SquareExceedsSequence(n, n + 1);
        var anchor = BinarySearch.LowerBound<int, SquareExceedsSequence>(sequence, 1) - 1;

        var (found, remaining) = ScanSmallDivisors(n, anchor, k);

        return found != -1 ? found : ScanPairedDivisors(n, anchor, remaining);
    }

    private static (int Found, int Remaining) ScanSmallDivisors(int n, int anchor, int remaining)
    {
        for (var divisor = 1; divisor <= anchor; divisor++)
        {
            if (n % divisor == 0 && --remaining == 0)
            {
                return (divisor, remaining);
            }
        }

        return (-1, remaining);
    }

    private static int ScanPairedDivisors(int n, int anchor, int remaining)
    {
        for (var divisor = anchor; divisor >= 1; divisor--)
        {
            if (divisor * divisor == n || n % divisor != 0)
            {
                continue;
            }

            if (--remaining == 0)
            {
                return n / divisor;
            }
        }

        return -1;
    }

    private readonly struct SquareExceedsSequence(long x, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;
        public int Get(int value) => (long)value * value > x ? 1 : 0;
    }
}
