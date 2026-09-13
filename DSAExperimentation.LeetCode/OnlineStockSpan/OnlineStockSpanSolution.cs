using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<(int Price, int Span)>;

namespace DSAExperimentation.LeetCode.OnlineStockSpan;

// LeetCode 901. Online Stock Span: a StockSpanner is fed one daily price at a time
// and answers, for each, how many consecutive days ending today had a price less
// than or equal to today's. The design-problem framing is a stream of next(price)
// calls; the answer that stream produces is the sequence of spans, so both
// strategies here take the whole price stream and return its spans in order - the
// shape that can actually be asserted and measured against itself.
//
// SpansByBackwardScan is the textbook per-call rescan: walk backwards from today
// until a strictly greater price stops you (O(n) per day, O(n^2) over the stream).
// SpansByMonotonicStack keeps a non-increasing stack of still-standing
// (price, span) pairs in this repo's own Stack<T> - the same "pop everything the
// new value dominates" move DailyTemperatures/NextGreaterElementI make with a
// plain Stack<int>, carrying each popped run's already-accumulated span forward
// instead of an index gap, so every earlier day is visited at most once across the
// whole stream.
internal static class OnlineStockSpanSolution
{
    // Deliberately written without this repo's primitives - the baseline the
    // amortized stack sweep below has to justify itself against.
    public static int[] SpansByBackwardScan(int[] prices)
    {
        var spans = new int[prices.Length];

        for (var day = 0; day < prices.Length; day++)
        {
            spans[day] = ScanBackFrom(prices, day);
        }

        return spans;
    }

    // Counts today plus every immediately preceding day whose price never exceeds
    // today's, stopping at the first strictly greater price.
    private static int ScanBackFrom(int[] prices, int day)
    {
        var span = 1;
        var previous = day - 1;

        while (previous >= 0 && prices[previous] <= prices[day])
        {
            span++;
            previous--;
        }

        return span;
    }

    // One amortized O(n) sweep: each day is pushed once and popped at most once,
    // and a popped day's span folds into the span of the day that dominated it.
    public static int[] SpansByMonotonicStack(int[] prices)
    {
        var spans = new int[prices.Length];
        var standing = new RepoStack();

        for (var day = 0; day < prices.Length; day++)
        {
            spans[day] = AbsorbDominated(standing, prices[day]);
        }

        return spans;
    }

    // Pops every standing day today's price is at least as high as, accumulating
    // their spans, then pushes today with the span it just earned.
    private static int AbsorbDominated(RepoStack standing, int price)
    {
        var span = 1;

        while (standing.TryPeek(out var top) && top.Price <= price)
        {
            standing.TryPop(out _);
            span += top.Span;
        }

        standing.Push((price, span));
        return span;
    }
}
