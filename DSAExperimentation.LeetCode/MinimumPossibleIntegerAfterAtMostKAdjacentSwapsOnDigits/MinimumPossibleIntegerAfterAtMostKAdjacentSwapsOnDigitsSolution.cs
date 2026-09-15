using DSAExperimentation.DataStructures.FenwickTree;
using RepoIntQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.LeetCode.MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigits;

// LeetCode 1505. Minimum Possible Integer After at Most K Adjacent Swaps On Digits: the
// smallest arrangement reachable from a digit string using at most k adjacent swaps.
//
// Both strategies are the same greedy: fill each output slot with the smallest digit whose
// frontmost still-unplaced occurrence is affordable within the remaining swap budget, where
// the cost of pulling a digit forward is the number of still-unplaced digits sitting before
// it. They differ in how that count is obtained - an O(length) rescan of the live digit list,
// or an O(log length) Fenwick prefix query over a 0/1 "still unplaced" array.
internal static class MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsSolution
{
    private const int DigitCount = 10;

    // The textbook answer: physically simulate the swaps. Keep the live digits in a BCL
    // List<char>, scan the affordable prefix for the smallest one, emit it and RemoveAt it -
    // O(length) per output slot, O(length^2) overall. Deliberately written without this
    // repo's primitives; it is the arm the Fenwick greedy has to beat.
    public static string MinIntegerByListRemoval(string num, int k)
    {
        var remaining = num.ToList();
        var result = new char[remaining.Count];
        var remainingSwaps = k;

        for (var slot = 0; slot < result.Length; slot++)
        {
            var bestIndex = FindCheapestSmallestIndex(remaining, remainingSwaps);

            remainingSwaps -= bestIndex;
            result[slot] = remaining[bestIndex];
            remaining.RemoveAt(bestIndex);
        }

        return new string(result);
    }

    private static int FindCheapestSmallestIndex(List<char> remaining, int remainingSwaps)
    {
        var bestIndex = 0;

        for (var index = 1; index < remaining.Count && index <= remainingSwaps; index++)
        {
            if (remaining[index] < remaining[bestIndex])
            {
                bestIndex = index;
            }
        }

        return bestIndex;
    }

    // This repo's own FenwickTree<int, SumOperation<int>> over a 0/1 "still unplaced" array
    // answers "how many unplaced digits sit before this position" in O(log length): Add marks
    // a position placed by subtracting its 1, PrefixQuery counts what is left in front of it.
    // Per-digit occurrence order rides on this repo's own Queue<int> - FIFO, because
    // PrefixQuery is monotonic non-decreasing in position, so a digit's frontmost occurrence
    // is always its cheapest one.
    public static string MinIntegerByFenwickTreeGreedy(string num, int k)
    {
        var length = num.Length;
        var unplacedFlags = Enumerable.Repeat(1, length).ToArray();
        var context = new PlacementContext(
            BuildPositionsByDigit(num),
            new FenwickTree<int, SumOperation<int>>(unplacedFlags),
            new char[length]);
        var remainingSwaps = k;

        for (var slot = 0; slot < length; slot++)
        {
            remainingSwaps = PlaceNextDigit(context, slot, remainingSwaps);
        }

        return new string(context.Result);
    }

    private static RepoIntQueue[] BuildPositionsByDigit(string num)
    {
        var positionsByDigit = new RepoIntQueue[DigitCount];

        for (var digit = 0; digit < DigitCount; digit++)
        {
            positionsByDigit[digit] = new RepoIntQueue();
        }

        for (var position = 0; position < num.Length; position++)
        {
            positionsByDigit[num[position] - '0'].Enqueue(position);
        }

        return positionsByDigit;
    }

    private static int PlaceNextDigit(PlacementContext context, int slot, int remainingSwaps)
    {
        for (var digit = 0; digit < DigitCount; digit++)
        {
            var updatedSwaps = TryPlaceDigit(context, digit, slot, remainingSwaps);

            if (updatedSwaps is { } affordable)
            {
                return affordable;
            }
        }

        return remainingSwaps;
    }

    private static int? TryPlaceDigit(PlacementContext context, int digit, int slot, int remainingSwaps)
    {
        if (AffordablePlacement(context, digit, remainingSwaps) is not { } placement)
        {
            return null;
        }

        CommitPlacement(context, digit, slot, placement.Position);
        return remainingSwaps - placement.Cost;
    }

    // This digit's frontmost still-unplaced occurrence, with the swap count that
    // pulling it to the front costs - or nothing, when that exceeds the budget.
    private static (int Position, int Cost)? AffordablePlacement(
        PlacementContext context, int digit, int remainingSwaps)
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

        return (position, cost);
    }

    // Placing the digit consumes its occurrence and marks the position taken, so
    // no later query counts it as still in front of anything.
    private static void CommitPlacement(PlacementContext context, int digit, int slot, int position)
    {
        context.Result[slot] = (char)('0' + digit);
        context.PositionsByDigit[digit].TryDequeue(out _);
        context.StillUnplaced.Add(position, -1);
    }

    private readonly record struct PlacementContext(
        RepoIntQueue[] PositionsByDigit,
        FenwickTree<int, SumOperation<int>> StillUnplaced,
        char[] Result);
}
