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

        AssertNextSpan(spanner, 100, 1);
        AssertNextSpan(spanner, 80, 1);
        AssertNextSpan(spanner, 60, 1);
        AssertNextSpan(spanner, 70, 2);
        AssertNextSpan(spanner, 60, 1);
        AssertNextSpan(spanner, 75, 4);
        AssertNextSpan(spanner, 85, 6);
    }

    [Fact]
    public void Next_StrictlyIncreasingPrices_SpanGrowsEveryCall()
    {
        var spanner = new StockSpannerOperations();

        AssertNextSpan(spanner, 10, 1);
        AssertNextSpan(spanner, 20, 2);
        AssertNextSpan(spanner, 30, 3);
    }

    private static void AssertNextSpan(StockSpannerOperations spanner, int price, int expectedSpan)
    {
        var actual = spanner.Next(price);
        Assert.Equal(expectedSpan, actual);
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
