using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfRecentCalls;

// LeetCode 933. Number of Recent Calls: Ping(t) enqueues t and drops every request
// older than t - 3000 from the front before reporting the remaining count - exactly
// this repo's own Queue<int>'s FIFO shape (Enqueue at the back, TryDequeue from the
// front), the same "compose, don't invent a new representation" move
// DesignCircularQueueTests already makes over Deque<int>.
public sealed partial class NumberOfRecentCallsTests
{
    [Fact]
    public void Ping_LeetCodeExample_ReturnsCountsWithinTheThreeThousandMillisecondWindow()
    {
        var counter = new RecentCounter();

        Assert.Equal(1, counter.Ping(1));
        Assert.Equal(2, counter.Ping(100));
        Assert.Equal(3, counter.Ping(3001));
        Assert.Equal(3, counter.Ping(3002));
    }

    [Fact]
    public void Ping_RequestsFarApart_EachDropsAllEarlierRequests()
    {
        var counter = new RecentCounter();

        Assert.Equal(1, counter.Ping(1));
        Assert.Equal(1, counter.Ping(10_000));
        Assert.Equal(1, counter.Ping(20_000));
    }

    private sealed class RecentCounter
    {
        private readonly RepoQueue _pings = new();

        public int Ping(int t)
        {
            _pings.Enqueue(t);

            while (_pings.TryPeek(out var oldest) && oldest < t - 3000)
            {
                _pings.TryDequeue(out _);
            }

            return _pings.Count;
        }
    }
}
