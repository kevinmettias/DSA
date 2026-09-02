using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find Positive Integer Solution for a Given Equation (LC 1237): three genuinely
// distinct strategies over the same "f increasing in both x and y" black box
// (ShortestPathAlgorithmBenchmarks precedent for benchmarking every real tier
// instead of just winner-vs-brute-force) - the O(n^2) check-every-pair brute force,
// the classic O(n) two-pointer walk that exploits monotonicity on both axes, and an
// O(n log n) per-row BinarySearch.Find over this repo's own IRandomAccessSequence
// (KokoEatingBananasBenchmarks/CapacityToShipPackagesWithinDDaysBenchmarks precedent
// for "binary search over a computed sequence"). BinarySearchPerRow is expected to
// lose to TwoPointer here - the point of including it is showing the reusable
// primitive is *available* and correct, not that it's the asymptotically best tool
// for this particular monotone-on-both-axes shape.
[MemoryDiagnoser]
public class FindPositiveIntegerSolutionForAGivenEquationBenchmarks
{
    // Doubling factor for the largest reachable sum (Bound + Bound).
    private const int MaxSumFactor = 2;

    [Params(300, 1_000)]
    public int Bound;

    private int _z;

    [GlobalSetup]
    public void Setup()
    {
        // The largest reachable sum (Bound + Bound) is never a solution here, so
        // every strategy is forced through its full worst-case walk instead of an
        // early-exit on the first/last pair making brute force look artificially
        // competitive.
        _z = (MaxSumFactor * Bound) - 1;
    }

    private static int Function(int x, int y) => x + y;

    [Benchmark(Baseline = true)]
    public int BruteForceEveryPair()
    {
        var count = 0;

        for (var x = 1; x <= Bound; x++)
        {
            for (var y = 1; y <= Bound; y++)
            {
                if (Function(x, y) == _z)
                {
                    count++;
                }
            }
        }

        return count;
    }

    [Benchmark]
    public int TwoPointer()
    {
        var (x, y) = InitializeTwoPointerBounds();
        return CountSolutionsTwoPointer(x, y);
    }

    private (int X, int Y) InitializeTwoPointerBounds() => (1, Bound);

    private int CountSolutionsTwoPointer(int x, int y)
    {
        var count = 0;

        while (x <= Bound && y >= 1)
        {
            (x, y, count) = StepTwoPointer(x, y, count);
        }

        return count;
    }

    private (int X, int Y, int Count) StepTwoPointer(int x, int y, int count)
    {
        var value = Function(x, y);

        if (value == _z)
        {
            count++;
            x++;
            y--;
        }
        else if (value < _z)
        {
            x++;
        }
        else
        {
            y--;
        }

        return (x, y, count);
    }

    [Benchmark]
    public int BinarySearchPerRow()
    {
        var count = 0;

        for (var x = 1; x <= Bound; x++)
        {
            var row = new FunctionRowSequence(x, Bound);

            if (BinarySearch.Find(row, _z) is not null)
            {
                count++;
            }
        }

        return count;
    }

    private readonly struct FunctionRowSequence(int x, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;

        public int Get(int index) => Function(x, index + 1);
    }
}
