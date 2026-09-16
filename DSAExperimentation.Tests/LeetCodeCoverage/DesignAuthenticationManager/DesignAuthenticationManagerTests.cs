using DSAExperimentation.LeetCode.DesignAuthenticationManager;

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
        RunScript(
            new DesignAuthenticationManagerSolution.AuthenticationManagerByLinearScanList(timeToLive),
            operations,
            expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void AuthenticationManagerByHashMap_LeetCodeExamples_MatchesExpectedSequence(
        int timeToLive, AuthenticationManagerOp[] operations, int?[] expected) =>
        RunScript(
            new DesignAuthenticationManagerSolution.AuthenticationManagerByHashMap(timeToLive),
            operations,
            expected);

    private static void RunScript(
        DesignAuthenticationManagerSolution.IAuthenticationManager manager,
        AuthenticationManagerOp[] operations,
        int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(manager));
        }
    }
}
