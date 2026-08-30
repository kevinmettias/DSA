using RepoDynamicArray = DSAExperimentation.DataStructures.DynamicArray.DynamicArray<string>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignBrowserHistory;

// LeetCode 1472. Design Browser History: a cursor into a growable sequence that
// truncates everything past the cursor on a new Visit - exactly this repo's own
// DynamicArray<string> (RemoveAt/Add/Get), the same "sequence + access constraint"
// composition Stack<T> already makes over DynamicArray<T> (ARCHITECTURE.md §4.1),
// just with the truncate-on-write behavior layered on top instead of a LIFO one.
public sealed partial class DesignBrowserHistoryTests
{
    [Fact]
    public void VisitBackForward_LeetCodeExample_MatchesExpectedSequence()
    {
        var history = new BrowserHistory("leetcode.com");

        history.Visit("google.com");
        history.Visit("facebook.com");
        history.Visit("youtube.com");

        Assert.Equal("facebook.com", history.Back(1));
        Assert.Equal("google.com", history.Back(1));
        Assert.Equal("facebook.com", history.Forward(1));

        history.Visit("linkedin.com");

        Assert.Equal("linkedin.com", history.Forward(2));
        Assert.Equal("google.com", history.Back(2));
        Assert.Equal("leetcode.com", history.Back(7));
    }

    [Fact]
    public void Visit_AfterMovingBack_TruncatesTheDiscardedForwardHistory()
    {
        var history = new BrowserHistory("home.com");

        history.Visit("a.com");
        history.Visit("b.com");
        history.Back(2);
        history.Visit("c.com");

        // "b.com" was discarded by the intervening Visit, so Forward has nowhere
        // left to go past the newly-visited page.
        Assert.Equal("c.com", history.Forward(1));
        Assert.Equal("home.com", history.Back(2));
    }

    private sealed class BrowserHistory
    {
        private readonly RepoDynamicArray _history = new();
        private int _current;

        public BrowserHistory(string homepage) => _history.Add(homepage);

        public void Visit(string url)
        {
            while (_history.Count > _current + 1)
            {
                _history.RemoveAt(_history.Count - 1);
            }

            _history.Add(url);
            _current++;
        }

        public string Back(int steps)
        {
            _current = Math.Max(0, _current - steps);
            return _history.Get(_current);
        }

        public string Forward(int steps)
        {
            _current = Math.Min(_history.Count - 1, _current + steps);
            return _history.Get(_current);
        }
    }
}
