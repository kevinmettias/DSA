using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.FenwickTree;
using RepoIntQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Possible Integer After at Most K Adjacent Swaps On Digits (LC
// 1505): the textbook physical simulation - repeatedly scan the still-live
// digit list for the cheapest reachable smallest digit and List.RemoveAt it,
// O(Length) per output slot, O(Length^2) overall, the direct cost of
// literally performing the adjacent swaps - against this repo's own
// FenwickTree<int,SumOperation<int>> tracking "still unplaced" as a 0/1
// array, which answers the identical "how many unplaced digits sit before
// this one" question in O(log Length) instead of an O(Length) scan. K is
// fixed at int.MaxValue/2 (an effectively unlimited swap budget) so both
// strategies are always forced to consider the entire remaining digit list
// at every slot, rather than an early exit on a tiny budget making brute
// force look artificially competitive.
[MemoryDiagnoser]
public class MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsBenchmarks
{
    private const int UnlimitedBudget = int.MaxValue / 2;

    private const int RandomSeed = 1505; // LeetCode problem number

    private const int DigitCount = 10;

    [Params(200, 2_000)]
    public int Length;

    private string _digits = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _digits = new string(Enumerable.Range(0, Length).Select(_ => (char)('0' + random.Next(DigitCount))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public string BruteForceListRemoval()
    {
        var remaining = _digits.ToList();
        var result = new char[remaining.Count];
        var remainingSwaps = UnlimitedBudget;

        for (var i = 0; i < result.Length; i++)
        {
            var bestIndex = 0;

            for (var index = 1; index < remaining.Count && index <= remainingSwaps; index++)
            {
                if (remaining[index] < remaining[bestIndex])
                {
                    bestIndex = index;
                }
            }

            remainingSwaps -= bestIndex;
            result[i] = remaining[bestIndex];
            remaining.RemoveAt(bestIndex);
        }

        return new string(result);
    }

    private readonly record struct GreedyPlacementContext(
        RepoIntQueue[] PositionsByDigit, FenwickTree<int, SumOperation<int>> StillUnplaced, char[] Result);

    [Benchmark]
    public string FenwickTreeGreedy()
    {
        var n = _digits.Length;
        var positionsByDigit = BuildPositionsByDigit(n);
        var stillUnplaced = new FenwickTree<int, SumOperation<int>>(Enumerable.Repeat(1, n).ToArray());
        var result = new char[n];
        var context = new GreedyPlacementContext(positionsByDigit, stillUnplaced, result);

        RunPlacement(n, context);

        return new string(result);
    }

    private RepoIntQueue[] BuildPositionsByDigit(int n)
    {
        var positionsByDigit = new RepoIntQueue[DigitCount];

        for (var digit = 0; digit < DigitCount; digit++)
        {
            positionsByDigit[digit] = new RepoIntQueue();
        }

        for (var i = 0; i < n; i++)
        {
            positionsByDigit[_digits[i] - '0'].Enqueue(i);
        }

        return positionsByDigit;
    }

    private static void RunPlacement(int n, GreedyPlacementContext context)
    {
        var remainingSwaps = UnlimitedBudget;

        for (var i = 0; i < n; i++)
        {
            remainingSwaps = PlaceNextDigit(i, remainingSwaps, context);
        }
    }

    private static int PlaceNextDigit(int i, int remainingSwaps, GreedyPlacementContext context)
    {
        for (var digit = 0; digit < DigitCount; digit++)
        {
            var placed = TryPlaceDigit(digit, i, remainingSwaps, context);

            if (placed is { } updatedSwaps)
            {
                return updatedSwaps;
            }
        }

        return remainingSwaps;
    }

    private static int? TryPlaceDigit(int digit, int i, int remainingSwaps, GreedyPlacementContext context)
    {
        if (!context.PositionsByDigit[digit].TryPeek(out var position))
        {
            return null;
        }

        var cost = position == 0 ? 0 : context.StillUnplaced.PrefixQuery(position - 1);

        if (cost > remainingSwaps)
        {
            return null;
        }

        context.Result[i] = (char)('0' + digit);
        context.PositionsByDigit[digit].TryDequeue(out _);
        context.StillUnplaced.Add(position, -1);

        return remainingSwaps - cost;
    }
}
