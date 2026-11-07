using System.Runtime.InteropServices;
using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<(int Price, int Span)>;

namespace DSAExperimentation.LeetCode.Solutions.OnlineStockSpan;

// LeetCode 901. Online Stock Span: a StockSpanner is fed one daily price at a time
// and answers, for each, how many consecutive days ending today had a price less
// than or equal to today's. LeetCode's shape is that object and its next(price)
// calls, so each strategy is a factory, CreateBy<Strategy>, that makes one: the
// object the catalog harness replays LeetCode's call scripts on (ARCHITECTURE
// 17.13). The strategy's algorithm lives in a nested class named for it: the one
// step that answers a day, which the object takes on each next(price) call, and
// Spans, which takes the whole price stream and returns its spans in order by
// taking that same step over the stream as given - the method the hand-written
// benchmark times. Each strategy is written once whichever shape a caller wants,
// and the whole-stream shape pays for no object it does not need.
//
// BackwardScan is the textbook per-call rescan: walk backwards from today until a
// strictly greater price stops you (O(n) per day, O(n^2) over the stream).
// MonotonicStack keeps a non-increasing stack of still-standing (price, span)
// pairs in this repo's own Stack<T> - the same "pop everything the new value
// dominates" move DailyTemperatures/NextGreaterElementI make with a plain
// Stack<int>, carrying each popped run's already-accumulated span forward instead
// of an index gap, so every earlier day is visited at most once across the whole
// stream.
[LeetCodeQuestion("online-stock-span", LeetCodeAnswerOrder.AsWritten)]
internal static class OnlineStockSpanSolution
{
    // LeetCode's shape: a StockSpanner that keeps every price and rescans backwards
    // on each call. Deliberately written without this repo's primitives - the
    // baseline the amortized stack sweep below has to justify itself against.
    public static IStockSpanner CreateByBackwardScan() => new BackwardScanSpanner();

    // LeetCode's shape: a StockSpanner whose calls make one amortized O(n) sweep
    // over the stream - each day is pushed once and popped at most once, and a
    // popped day's span folds into the span of the day that dominated it.
    public static IStockSpanner CreateByMonotonicStack() => new MonotonicStackSpanner();

    // The backward-scan strategy: one day's rescan, and the whole stream's spans.
    internal static class BackwardScan
    {
        // The whole stream's spans, each day rescanned over the input array itself.
        public static int[] Spans(int[] prices)
        {
            var spans = new int[prices.Length];

            for (var day = 0; day < prices.Length; day++)
            {
                spans[day] = ScanBackFrom(prices, day);
            }

            return spans;
        }

        // Counts today plus every immediately preceding day whose price never
        // exceeds today's, stopping at the first strictly greater price.
        public static int ScanBackFrom(ReadOnlySpan<int> prices, int day)
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
    }

    // The monotonic-stack strategy: one day's absorption, and the whole stream's spans.
    internal static class MonotonicStack
    {
        // The whole stream's spans, every day absorbed into one stack of standing days.
        public static int[] Spans(int[] prices)
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
        public static int AbsorbDominated(RepoStack standing, int price)
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

    // Every price so far, in a plain BCL List<int>, rescanned backwards from today
    // on each call.
    private sealed class BackwardScanSpanner : IStockSpanner
    {
        private readonly List<int> _prices = [];

        public int Next(int price)
        {
            _prices.Add(price);

            var today = _prices.Count - 1;
            return BackwardScan.ScanBackFrom(CollectionsMarshal.AsSpan(_prices), today);
        }
    }

    // The days still standing, each with the span it earned, highest price at the
    // bottom.
    private sealed class MonotonicStackSpanner : IStockSpanner
    {
        private readonly RepoStack _standing = new();

        public int Next(int price) => MonotonicStack.AbsorbDominated(_standing, price);
    }
}
