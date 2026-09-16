using DSAExperimentation.LeetCode.DesignBrowserHistory;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignBrowserHistory;

// One call in a BrowserHistory script: which operation to invoke, and with what
// url or step count. Pure dispatch, built via the named factories below so a
// script reads like the LeetCode call sequence it replays.
public readonly record struct BrowserHistoryOp(BrowserHistoryOp.OpKind kind, string url, int steps)
{
    public static BrowserHistoryOp Visit(string url) => new(OpKind.Visit, url, 0);

    public static BrowserHistoryOp Back(int steps) => new(OpKind.Back, string.Empty, steps);

    public static BrowserHistoryOp Forward(int steps) => new(OpKind.Forward, string.Empty, steps);

    // null for Visit, matching LeetCode's own judge output for a void operation;
    // the landed url for the two navigations - so a script runner can assert
    // against one expected value per operation uniformly.
    internal string? Apply(DesignBrowserHistorySolution.IBrowserHistory history)
    {
        switch (kind)
        {
            case OpKind.Visit:
                history.Visit(url);
                return null;
            case OpKind.Back:
                return history.Back(steps);
            default:
                return history.Forward(steps);
        }
    }

    public enum OpKind
    {
        Visit,
        Back,
        Forward,
    }
}
