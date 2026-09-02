using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Closest Divisors (LC 1362): the textbook O(candidate) full-range divisor scan vs.
// BinarySearch.LowerBound anchoring the search at floor(sqrt(candidate)) - the same
// technique SqrtXBenchmarks uses for LC 69 - then a short walk down to the first
// exact divisor. Both check num+1 and num+2 and keep whichever pair is closer.
[MemoryDiagnoser]
public class ClosestDivisorsBenchmarks
{
    private const int SecondCandidateOffset = 2; // both benchmarks check num+1 and num+2
    private const int SqrtAnchorCeiling = 46_341; // ceil(sqrt(int.MaxValue)), caps the binary-search anchor

    [Params(1_000, 100_000)]
    public int Num;

    [Benchmark(Baseline = true)]
    public (int First, int Second) BruteForce()
    {
        var lower = BruteForcePairFor(Num + 1);
        var upper = BruteForcePairFor(Num + SecondCandidateOffset);
        return upper.Second - upper.First < lower.Second - lower.First ? upper : lower;
    }

    [Benchmark]
    public (int First, int Second) BinarySearchAnchored()
    {
        var lower = AnchoredPairFor(Num + 1);
        var upper = AnchoredPairFor(Num + SecondCandidateOffset);
        return upper.Second - upper.First < lower.Second - lower.First ? upper : lower;
    }

    private static (int First, int Second) BruteForcePairFor(int candidate)
    {
        var best = (First: 1, Second: candidate);

        for (var i = 1; i <= candidate; i++)
        {
            if (candidate % i != 0 || i > candidate / i)
            {
                continue;
            }

            if (candidate / i - i < best.Second - best.First)
            {
                best = (i, candidate / i);
            }
        }

        return best;
    }

    private static (int First, int Second) AnchoredPairFor(int candidate)
    {
        var sequence = new SquareExceedsSequence(candidate, Math.Min(candidate, SqrtAnchorCeiling) + 1);
        var divisor = BinarySearch.LowerBound<int, SquareExceedsSequence>(sequence, 1) - 1;

        while (candidate % divisor != 0)
        {
            divisor--;
        }

        return (divisor, candidate / divisor);
    }

    private readonly struct SquareExceedsSequence(long x, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;
        public int Get(int value) => (long)value * value > x ? 1 : 0;
    }
}
