using BenchmarkDotNet.Attributes;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Cost Tree From Leaf Values (LC 1130): plain un-memoized interval
// recursion over (left, right) leaf-index bounds, recomputing
// max(arr[left..split])/max(arr[split+1..right]) freshly for every split
// candidate - exponential, since each (left, right) sub-range recurs through
// every possible split many times over (the same MinimumScoreTriangulation-
// OfPolygonBenchmarks/BurstBalloonsBenchmarks interval-DP shape) - vs. the
// optimal O(n) monotonic-decreasing sweep over this repo's own Stack<int>
// (SumOfSubarrayMinimumsBenchmarks precedent): whichever leaf is smaller than
// its still-open neighbors can only ever be combined with the smaller of
// them first, so a single "pop while top <= current" pass finds every merge
// in the optimal tree without exploring alternatives. Length is kept modest
// for the same reason the polygon benchmark's VertexCount is: the
// un-memoized baseline's blowup is real.
[MemoryDiagnoser]
public class MinimumCostTreeFromLeafValuesBenchmarks
{
    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 1130;

    private const int MaxLeafValueExclusive = 100;

    private const int RemainingStackFloor = 2;

    [Params(10, 14)]
    public int Length;

    private int[] _arr = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _arr = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxLeafValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long UnmemoizedRecursion() => MinCost(0, _arr.Length - 1);

    private long MinCost(int left, int right)
    {
        if (left == right)
        {
            return 0;
        }

        var best = long.MaxValue;

        for (var split = left; split < right; split++)
        {
            var cost = MinCost(left, split) + MinCost(split + 1, right)
                + ((long)MaxIn(left, split) * MaxIn(split + 1, right));
            best = Math.Min(best, cost);
        }

        return best;
    }

    private int MaxIn(int left, int right)
    {
        var max = _arr[left];
        for (var i = left + 1; i <= right; i++)
        {
            max = Math.Max(max, _arr[i]);
        }

        return max;
    }

    [Benchmark]
    public long MonotonicStack()
    {
        var stack = new RepoIntStack();
        stack.Push(int.MaxValue);
        long total = 0;

        total += MergeSmallerNeighbors(stack, _arr);
        total += DrainRemainingStack(stack);

        return total;
    }

    private static long MergeSmallerNeighbors(RepoIntStack stack, int[] values)
    {
        long total = 0;

        foreach (var value in values)
        {
            while (stack.TryPeek(out var top) && top <= value)
            {
                stack.TryPop(out var mid);
                stack.TryPeek(out var next);
                total += (long)mid * Math.Min(next, value);
            }

            stack.Push(value);
        }

        return total;
    }

    private static long DrainRemainingStack(RepoIntStack stack)
    {
        long total = 0;

        while (stack.Count > RemainingStackFloor)
        {
            stack.TryPop(out var mid);
            stack.TryPeek(out var next);
            total += (long)mid * next;
        }

        return total;
    }
}
