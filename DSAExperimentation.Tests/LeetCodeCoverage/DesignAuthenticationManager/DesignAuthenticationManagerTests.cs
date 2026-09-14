using static DSAExperimentation.LeetCode.DesignAuthenticationManager.DesignAuthenticationManagerSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignAuthenticationManager;

// Harness only: both strategies live in DesignAuthenticationManagerSolution.
// LeetCode's own shape here is a stateful object across a sequence of calls, so
// Examples encodes a call script instead of a single argument tuple - the same
// shape DesignBrowserHistoryTests uses for its own instance-API problem. Generate
// and Renew return null in LeetCode's judge output, so AuthenticationManagerOp
// returns null for them too and the expected sequence reads exactly like the
// published one. The linear-scan baseline used to exist only as a benchmark arm
// with nothing asserting it; it is pinned to the same scripts here.
public sealed class DesignAuthenticationManagerTests
{
    public static TheoryData<int, AuthenticationManagerOp[], int?[]> Examples =>
        new()
        {
            {
                // LeetCode's published example.
                5,
                [
                    AuthenticationManagerOp.Renew("aaa", 1), // no such token - ignored
                    AuthenticationManagerOp.Generate("aaa", 2), // expires at 7
                    AuthenticationManagerOp.CountUnexpiredTokens(6),
                    AuthenticationManagerOp.Generate("bbb", 7), // expires at 12
                    AuthenticationManagerOp.Renew("aaa", 8), // expired at 7 - ignored
                    AuthenticationManagerOp.Renew("bbb", 10), // still alive - now 15
                    AuthenticationManagerOp.CountUnexpiredTokens(15),
                ],
                [null, null, 1, null, null, null, 0]
            },
            {
                // Renewal at the exact expiry instant is too late, one second
                // earlier is not.
                5,
                [
                    AuthenticationManagerOp.Generate("1", 1), // expires at 6
                    AuthenticationManagerOp.Generate("2", 2), // expires at 7
                    AuthenticationManagerOp.CountUnexpiredTokens(4),
                    AuthenticationManagerOp.Renew("1", 6), // 6 <= 6 - ignored
                    AuthenticationManagerOp.CountUnexpiredTokens(6),
                    AuthenticationManagerOp.Renew("2", 6), // 7 > 6 - renewed to 11
                    AuthenticationManagerOp.CountUnexpiredTokens(10),
                    AuthenticationManagerOp.CountUnexpiredTokens(11),
                ],
                [null, null, 2, null, 1, null, 1, 0]
            },
            {
                // An unknown token id and an already-expired one are both no-ops.
                3,
                [
                    AuthenticationManagerOp.Generate("1", 1), // expires at 4
                    AuthenticationManagerOp.Renew("missing", 2), // no such token
                    AuthenticationManagerOp.CountUnexpiredTokens(2),
                    AuthenticationManagerOp.Renew("1", 5), // expired at 4 - ignored
                    AuthenticationManagerOp.CountUnexpiredTokens(5),
                ],
                [null, null, 1, null, 0]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AuthenticationManagerByLinearScanList_LeetCodeExamples_MatchesExpectedSequence(
        int timeToLive, AuthenticationManagerOp[] operations, int?[] expected) =>
        RunScript(new AuthenticationManagerByLinearScanList(timeToLive), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void AuthenticationManagerByHashMap_LeetCodeExamples_MatchesExpectedSequence(
        int timeToLive, AuthenticationManagerOp[] operations, int?[] expected) =>
        RunScript(new AuthenticationManagerByHashMap(timeToLive), operations, expected);

    private static void RunScript(
        IAuthenticationManager manager, AuthenticationManagerOp[] operations, int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(manager));
        }
    }
}

// One call in an AuthenticationManager script: which operation to invoke, on which
// token, at what time. Pure dispatch, built via the named factories below so a
// script reads like the LeetCode call sequence it replays.
public readonly record struct AuthenticationManagerOp
{
    private readonly Kind _kind;
    private readonly string _tokenId;
    private readonly int _currentTime;

    private AuthenticationManagerOp(Kind kind, string tokenId, int currentTime)
    {
        _kind = kind;
        _tokenId = tokenId;
        _currentTime = currentTime;
    }

    public static AuthenticationManagerOp Generate(string tokenId, int currentTime) =>
        new(Kind.Generate, tokenId, currentTime);

    public static AuthenticationManagerOp Renew(string tokenId, int currentTime) =>
        new(Kind.Renew, tokenId, currentTime);

    public static AuthenticationManagerOp CountUnexpiredTokens(int currentTime) =>
        new(Kind.Count, string.Empty, currentTime);

    // null for the two void operations, matching LeetCode's own judge output, so a
    // script runner can assert against one expected value per operation uniformly.
    internal int? Apply(IAuthenticationManager manager)
    {
        switch (_kind)
        {
            case Kind.Generate:
                manager.Generate(_tokenId, _currentTime);
                return null;
            case Kind.Renew:
                manager.Renew(_tokenId, _currentTime);
                return null;
            default:
                return manager.CountUnexpiredTokens(_currentTime);
        }
    }

    private enum Kind
    {
        Generate,
        Renew,
        Count,
    }
}
