using static DSAExperimentation.LeetCode.DesignBrowserHistory.DesignBrowserHistorySolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignBrowserHistory;

// Harness only: both strategies live in DesignBrowserHistorySolution. LeetCode's
// own shape here is a stateful object across a sequence of calls, so Examples
// encodes a call script instead of a single argument tuple - the same shape
// DesignCircularQueueTests already uses for its own instance-API problem. A Visit
// returns null in LeetCode's judge output, so BrowserHistoryOp.Apply returns null
// for it too and the expected sequence reads exactly like the published one.
public sealed class DesignBrowserHistoryTests
{
    public static TheoryData<string, BrowserHistoryOp[], string?[]> Examples =>
        new()
        {
            {
                "leetcode.com",
                [
                    BrowserHistoryOp.Visit("google.com"),
                    BrowserHistoryOp.Visit("facebook.com"),
                    BrowserHistoryOp.Visit("youtube.com"),
                    BrowserHistoryOp.Back(1),
                    BrowserHistoryOp.Back(1),
                    BrowserHistoryOp.Forward(1),
                    BrowserHistoryOp.Visit("linkedin.com"),
                    BrowserHistoryOp.Forward(2),
                    BrowserHistoryOp.Back(2),
                    BrowserHistoryOp.Back(7),
                ],
                [
                    null,
                    null,
                    null,
                    "facebook.com",
                    "google.com",
                    "facebook.com",
                    null,
                    "linkedin.com",
                    "google.com",
                    "leetcode.com",
                ]
            },
            {
                // A Visit after moving back discards the forward history, so
                // Forward has nowhere left to go past the newly-visited page.
                "home.com",
                [
                    BrowserHistoryOp.Visit("a.com"),
                    BrowserHistoryOp.Visit("b.com"),
                    BrowserHistoryOp.Back(2),
                    BrowserHistoryOp.Visit("c.com"),
                    BrowserHistoryOp.Forward(1),
                    BrowserHistoryOp.Back(2),
                ],
                [null, null, "home.com", null, "c.com", "home.com"]
            },
            {
                // Nothing has been visited, so both directions clamp to the
                // homepage however many steps are asked for.
                "only.com",
                [
                    BrowserHistoryOp.Back(3),
                    BrowserHistoryOp.Forward(3),
                ],
                ["only.com", "only.com"]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void BrowserHistoryByListBacked_LeetCodeExamples_MatchesExpectedSequence(
        string homepage, BrowserHistoryOp[] operations, string?[] expected) =>
        RunScript(new BrowserHistoryByListBacked(homepage), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void BrowserHistoryByDynamicArrayBacked_LeetCodeExamples_MatchesExpectedSequence(
        string homepage, BrowserHistoryOp[] operations, string?[] expected) =>
        RunScript(new BrowserHistoryByDynamicArrayBacked(homepage), operations, expected);

    private static void RunScript(
        IBrowserHistory history, BrowserHistoryOp[] operations, string?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(history));
        }
    }
}

// One call in a BrowserHistory script: which operation to invoke, and with what
// url or step count. Pure dispatch, built via the named factories below so a
// script reads like the LeetCode call sequence it replays.
public readonly record struct BrowserHistoryOp
{
    private readonly Kind _kind;
    private readonly string _url;
    private readonly int _steps;

    private BrowserHistoryOp(Kind kind, string url, int steps)
    {
        _kind = kind;
        _url = url;
        _steps = steps;
    }

    public static BrowserHistoryOp Visit(string url) => new(Kind.Visit, url, 0);

    public static BrowserHistoryOp Back(int steps) => new(Kind.Back, string.Empty, steps);

    public static BrowserHistoryOp Forward(int steps) => new(Kind.Forward, string.Empty, steps);

    // null for Visit, matching LeetCode's own judge output for a void operation;
    // the landed url for the two navigations - so a script runner can assert
    // against one expected value per operation uniformly.
    internal string? Apply(IBrowserHistory history)
    {
        switch (_kind)
        {
            case Kind.Visit:
                history.Visit(_url);
                return null;
            case Kind.Back:
                return history.Back(_steps);
            default:
                return history.Forward(_steps);
        }
    }

    private enum Kind
    {
        Visit,
        Back,
        Forward,
    }
}
