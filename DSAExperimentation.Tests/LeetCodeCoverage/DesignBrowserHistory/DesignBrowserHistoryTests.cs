using DSAExperimentation.LeetCode.DesignBrowserHistory;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignBrowserHistory;

// Harness only: both strategies live in DesignBrowserHistorySolution. LeetCode's
// own shape here is a stateful object across a sequence of calls, so Examples
// encodes a call script instead of a single argument tuple - the same shape
// DesignCircularQueueTests already uses for its own instance-API problem. A Visit
// returns null in LeetCode's judge output, so BrowserHistoryOp.Apply returns null
// for it too and the expected sequence reads exactly like the published one.
public sealed partial class DesignBrowserHistoryTests
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
        Assert.Equal(expected, RunScript(new DesignBrowserHistorySolution.BrowserHistoryByListBacked(homepage), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void BrowserHistoryByDynamicArrayBacked_LeetCodeExamples_MatchesExpectedSequence(
        string homepage, BrowserHistoryOp[] operations, string?[] expected) =>
        Assert.Equal(expected, RunScript(
            new DesignBrowserHistorySolution.BrowserHistoryByDynamicArrayBacked(homepage),
            operations));

    private static string?[] RunScript(
        DesignBrowserHistorySolution.IBrowserHistory history,
        BrowserHistoryOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(history))];
}
