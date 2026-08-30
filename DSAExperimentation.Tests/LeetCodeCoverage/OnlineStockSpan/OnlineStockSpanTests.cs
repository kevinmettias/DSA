using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<(int Price, int Span)>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OnlineStockSpan;

// LeetCode 901. Online Stock Span: a monotonic non-increasing Stack<(Price, Span)> of
// this repo's own Stack<T> - the same "pop everything the new value dominates" move
// DailyTemperatures/NextGreaterElementI already make with a plain Stack<int>, just
// carrying each popped run's already-accumulated span forward instead of an index
// gap, so every earlier day is visited at most once across the whole stream.
public sealed partial class OnlineStockSpanTests
{
    [Fact]
    public void Next_LeetCodeExample_ReturnsExpectedSpans()
    {
        var spanner = new StockSpannerOperations();

        Assert.Equal(1, spanner.Next(100));
        Assert.Equal(1, spanner.Next(80));
        Assert.Equal(1, spanner.Next(60));
        Assert.Equal(2, spanner.Next(70));
        Assert.Equal(1, spanner.Next(60));
        Assert.Equal(4, spanner.Next(75));
        Assert.Equal(6, spanner.Next(85));
    }

    [Fact]
    public void Next_StrictlyIncreasingPrices_SpanGrowsEveryCall()
    {
        var spanner = new StockSpannerOperations();

        Assert.Equal(1, spanner.Next(10));
        Assert.Equal(2, spanner.Next(20));
        Assert.Equal(3, spanner.Next(30));
    }

    private sealed class StockSpannerOperations
    {
        private readonly RepoStack _prices = new();

        public int Next(int price)
        {
            var span = 1;

            while (_prices.TryPeek(out var top) && top.Price <= price)
            {
                _prices.TryPop(out _);
                span += top.Span;
            }

            _prices.Push((price, span));
            return span;
        }
    }
}
