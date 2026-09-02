using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindConsecutiveIntegersFromADataStream;

// LeetCode 2526. Find Consecutive Integers from a Data Stream: this repo's own
// Deque<int> holds a fixed-size window of the last k values seen (PushBack on
// arrival, TryPopFront once the window grows past k) - the same windowing shape
// SlidingWindowMaximumTests already uses - with a running count of how many slots
// currently equal `value` maintained incrementally as items enter/leave the window,
// instead of rescanning the window on every call.
public sealed class FindConsecutiveIntegersFromADataStreamTests
{
    [Fact]
    public void Consec_LeetCodeExample_MatchesExpectedSequence()
    {
        var stream = new DataStream(4, 3);

        Assert.False(stream.Consec(4));
        Assert.False(stream.Consec(4));
        Assert.True(stream.Consec(4));
        Assert.False(stream.Consec(3));
    }

    [Fact]
    public void Consec_FewerThanKAdded_ReturnsFalse()
    {
        var stream = new DataStream(1, 5);

        Assert.False(stream.Consec(1));
        Assert.False(stream.Consec(1));
    }

    [Fact]
    public void Consec_MismatchThenWindowRefillsWithMatches_ReturnsTrueAgain()
    {
        var stream = new DataStream(5, 3);

        Assert.False(stream.Consec(5));
        Assert.False(stream.Consec(5));
        Assert.True(stream.Consec(5));
        Assert.False(stream.Consec(1));
        Assert.False(stream.Consec(5));
        Assert.False(stream.Consec(5));
        Assert.True(stream.Consec(5));
    }

    private sealed class DataStream(int value, int k)
    {
        private readonly RepoDeque _window = new();
        private int _matchCount;

        public bool Consec(int num)
        {
            _window.PushBack(num);

            if (num == value)
            {
                _matchCount++;
            }

            if (_window.Count > k && _window.TryPopFront(out var evicted) && evicted == value)
            {
                _matchCount--;
            }

            return _window.Count == k && _matchCount == k;
        }
    }
}
