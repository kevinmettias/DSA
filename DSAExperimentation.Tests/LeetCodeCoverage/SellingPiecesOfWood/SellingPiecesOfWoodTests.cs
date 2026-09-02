using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SellingPiecesOfWood;

// LeetCode 2312. Selling Pieces of Wood: interval DP over (height, width) piece
// dimensions - the same (Left, Right)-keyed Memoizer<TState,TResult> shape
// MinimumScoreTriangulationOfPolygonTests/BurstBalloonsTests already use, generalized
// to two independent cut axes (a piece can be split horizontally OR vertically, not
// just along one interval) the same way NumberOfWaysOfCuttingAPizzaTests' (Row, Col,
// RemainingCuts) state already generalizes a single-axis interval recurrence. This
// repo's own HashMap<TKey,TValue> looks up each (height, width) pair's listed sale
// price in O(1) - the same TwoSum composition, keyed on a ValueTuple instead of a
// plain int. Return type is long, not int: up to 2*10^4 listed prices at up to 10^6
// each can sum past Int32.MaxValue, the same overflow-law-first reasoning
// IntegerReplacementTests' int-vs-long choice already documents.
public sealed partial class SellingPiecesOfWoodTests
{
    [Fact]
    public void SellingWood_LeetCodeExampleOne_ReturnsNineteen()
    {
        var prices = new[]
        {
            (Height: 1, Width: 4, Price: 2),
            (Height: 2, Width: 2, Price: 7),
            (Height: 2, Width: 1, Price: 3),
        };

        var actual = SellingWood(m: 3, n: 5, prices);

        Assert.Equal(19, actual);
    }

    [Fact]
    public void SellingWood_LeetCodeExampleTwo_ReturnsThirtyTwo()
    {
        var prices = new[]
        {
            (Height: 3, Width: 2, Price: 10),
            (Height: 1, Width: 4, Price: 2),
            (Height: 4, Width: 1, Price: 3),
        };

        var actual = SellingWood(m: 4, n: 6, prices);

        Assert.Equal(32, actual);
    }

    [Fact]
    public void SellingWood_NoPricesListed_UncutWholePieceIsWorthless()
    {
        var actual = SellingWood(m: 2, n: 2, []);
        Assert.Equal(0, actual);
    }

    private static long SellingWood(int m, int n, (int Height, int Width, int Price)[] prices)
    {
        var priceByDimensions = new HashMap<(int Height, int Width), int>();
        foreach (var (height, width, price) in prices)
        {
            priceByDimensions.Set((height, width), price);
        }

        return Memoizer.Memoize<(int Height, int Width), long>(
            (m, n),
            (piece, bestValue) => BestValue(piece, priceByDimensions, bestValue));
    }

    private static long BestValue(
        (int Height, int Width) piece,
        HashMap<(int Height, int Width), int> priceByDimensions,
        Func<(int Height, int Width), long> bestValue)
    {
        var (height, width) = piece;
        long best = priceByDimensions.TryGetValue(piece, out var listedPrice) ? listedPrice : 0;

        for (var cut = 1; cut < height; cut++)
        {
            best = Math.Max(best, bestValue((cut, width)) + bestValue((height - cut, width)));
        }

        for (var cut = 1; cut < width; cut++)
        {
            best = Math.Max(best, bestValue((height, cut)) + bestValue((height, width - cut)));
        }

        return best;
    }
}
