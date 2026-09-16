using DSAExperimentation.LeetCode.DesignAnATMMachine;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignAnATMMachine;

// Harness only: both strategies live in DesignAnATMMachineSolution. LeetCode's own
// shape here is a stateful object across a sequence of calls, so Examples encodes a
// call script instead of a single argument tuple - the same shape
// DesignAuthenticationManagerTests uses for its own instance-API problem. The
// five-slot array strategy used to exist only as a benchmark arm with nothing
// asserting it, and it answered a weaker question there (did the withdrawal
// succeed?) than the test's own arm did (which banknotes came out); both are pinned
// to LeetCode's real answer here.
public sealed partial class DesignAnATMMachineTests
{
    public static TheoryData<AtmOp[], long[]?[]> Examples =>
        new()
        {
            {
                // LeetCode's published example. The second withdrawal fails because
                // the greedy walk takes the $500 first and is then left with $100 it
                // has no note for; the third succeeds on the same contents.
                [
                    AtmOp.Deposit([0, 0, 1, 2, 1]),
                    AtmOp.Withdraw(600),
                    AtmOp.Deposit([0, 1, 0, 1, 1]),
                    AtmOp.Withdraw(600),
                    AtmOp.Withdraw(550),
                ],
                [null, [0, 0, 1, 0, 1], null, [-1], [0, 1, 0, 0, 1]]
            },
            {
                // 60 could be reached as three $20s, but the greedy walk never tries
                // that: it takes one $50 first, leaving 10, and no smaller
                // denomination covers 10. The failed withdrawal must leave the $20s
                // untouched, which the following one proves.
                [
                    AtmOp.Deposit([10, 10, 0, 0, 0]),
                    AtmOp.Withdraw(60),
                    AtmOp.Withdraw(20),
                ],
                [null, [-1], [1, 0, 0, 0, 0]]
            },
            {
                // An empty machine refuses everything, and a machine emptied by a
                // successful withdrawal refuses the repeat.
                [
                    AtmOp.Withdraw(20),
                    AtmOp.Deposit([1, 0, 0, 0, 0]),
                    AtmOp.Withdraw(20),
                    AtmOp.Withdraw(20),
                ],
                [[-1], null, [1, 0, 0, 0, 0], [-1]]
            },
            {
                // An amount no combination of the notes on hand can make: a single
                // $500 covers neither $30 nor $520, but covers $500 exactly.
                [
                    AtmOp.Deposit([0, 0, 0, 0, 1]),
                    AtmOp.Withdraw(30),
                    AtmOp.Withdraw(520),
                    AtmOp.Withdraw(500),
                ],
                [null, [-1], [-1], [0, 0, 0, 0, 1]]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AtmByFiveSlotArray_LeetCodeExamples_MatchesExpectedSequence(
        AtmOp[] operations, long[]?[] expected) =>
        Assert.Equal(expected, RunScript(new DesignAnATMMachineSolution.AtmByFiveSlotArray(), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void AtmByHashMap_LeetCodeExamples_MatchesExpectedSequence(
        AtmOp[] operations, long[]?[] expected) =>
        Assert.Equal(expected, RunScript(new DesignAnATMMachineSolution.AtmByHashMap(), operations));

    private static long[]?[] RunScript(
        DesignAnATMMachineSolution.IAtm atm, AtmOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(atm))];
}
