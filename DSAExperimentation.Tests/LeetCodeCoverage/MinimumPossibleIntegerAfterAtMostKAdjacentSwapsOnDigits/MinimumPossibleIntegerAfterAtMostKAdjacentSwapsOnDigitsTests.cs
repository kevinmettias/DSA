using DSAExperimentation.DataStructures.FenwickTree;
using RepoIntQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigits;

// LeetCode 1505. Minimum Possible Integer After at Most K Adjacent Swaps On
// Digits: greedily fill each output slot with the smallest digit whose
// frontmost still-unplaced occurrence is affordable within the remaining
// swap budget. "Affordable" means: how many still-unplaced digits sit
// before it in the original string, since adjacent swaps only ever have to
// hop it past those - exactly a Fenwick-tree prefix count over a 0/1
// "still unplaced" array, this repo's own FenwickTree<int,SumOperation<int>>
// (Add marks a position placed by subtracting its 1, PrefixQuery counts how
// many unplaced positions remain before a given index). Per-digit occurrence
// order is tracked with this repo's own Queue<int> (FIFO: earliest, and
// therefore cheapest, occurrence first - PrefixQuery is monotonic
// non-decreasing in position, so a digit's frontmost occurrence is always
// its cheapest one).
public sealed partial class MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsTests
{
    [Fact]
    public void MinInteger_LeetCodeExampleOne_ReturnsSmallestReachableArrangement()
    {
        var actual = MinInteger("4321", k: 4);

        Assert.Equal("1342", actual);
    }

    [Fact]
    public void MinInteger_LeetCodeExampleTwo_ReturnsSmallestReachableArrangement()
    {
        var actual = MinInteger("100", k: 1);

        Assert.Equal("010", actual);
    }

    [Fact]
    public void MinInteger_AlreadyAscending_UnaffectedByAnySwapBudget()
    {
        var actual = MinInteger("36789", k: 1000);

        Assert.Equal("36789", actual);
    }

    [Fact]
    public void MinInteger_ZeroSwapBudget_ReturnsInputUnchanged()
    {
        var actual = MinInteger("4321", k: 0);

        Assert.Equal("4321", actual);
    }

    private static string MinInteger(string num, int k)
    {
        var positionsByDigit = BuildPositionsByDigit(num);
        var result = PlaceAllDigits(positionsByDigit, num.Length, k);

        return new string(result);
    }

    private static RepoIntQueue[] BuildPositionsByDigit(string num)
    {
        var positionsByDigit = new RepoIntQueue[10];

        for (var digit = 0; digit < 10; digit++)
        {
            positionsByDigit[digit] = new RepoIntQueue();
        }

        for (var i = 0; i < num.Length; i++)
        {
            positionsByDigit[num[i] - '0'].Enqueue(i);
        }

        return positionsByDigit;
    }

    private static char[] PlaceAllDigits(RepoIntQueue[] positionsByDigit, int n, int k)
    {
        var stillUnplaced = new FenwickTree<int, SumOperation<int>>(Enumerable.Repeat(1, n).ToArray());
        var result = new char[n];
        var remainingSwaps = k;
        var context = new PlacementContext(positionsByDigit, stillUnplaced, result);

        for (var i = 0; i < n; i++)
        {
            PlaceBestDigit(context, i, ref remainingSwaps);
        }

        return result;
    }

    private static void PlaceBestDigit(PlacementContext context, int i, ref int remainingSwaps)
    {
        for (var digit = 0; digit < 10; digit++)
        {
            if (!context.PositionsByDigit[digit].TryPeek(out var position))
            {
                continue;
            }

            var cost = position == 0 ? 0 : context.StillUnplaced.PrefixQuery(position - 1);

            if (cost > remainingSwaps)
            {
                continue;
            }

            remainingSwaps -= cost;
            context.Result[i] = (char)('0' + digit);
            context.PositionsByDigit[digit].TryDequeue(out _);
            context.StillUnplaced.Add(position, -1);
            break;
        }
    }

    private readonly record struct PlacementContext(
        RepoIntQueue[] PositionsByDigit,
        FenwickTree<int, SumOperation<int>> StillUnplaced,
        char[] Result);
}
