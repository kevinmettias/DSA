using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignAnATMMachine;

// LeetCode 2241. Design an ATM Machine: five fixed denomination counters keyed by
// banknote value, held in this repo's own HashMap<int,long> (DesignParkingSystemTests
// precedent) rather than five separate fields. Withdraw is a strict greedy walk from
// the largest denomination down - not a general coin-change search - so it can fail
// (and must leave state untouched) even when some other combination of banknotes
// would have worked.
public sealed partial class DesignAnATMMachineTests
{
    private static readonly int[] DenominationsAscending = [20, 50, 100, 200, 500];

    [Fact]
    public void WithdrawSequence_LeetCodeExample_MatchesExpectedResults()
    {
        var atm = new Atm();

        atm.Deposit([0, 0, 1, 2, 1]);
        Assert.Equal(new long[] { 0, 0, 1, 0, 1 }, atm.Withdraw(600));

        atm.Deposit([0, 1, 0, 1, 1]);
        Assert.Equal(new long[] { -1 }, atm.Withdraw(600));
        Assert.Equal(new long[] { 0, 1, 0, 0, 1 }, atm.Withdraw(550));
    }

    [Fact]
    public void Withdraw_AmountNotRepresentableInGreedyOrder_FailsWithoutChangingState()
    {
        var atm = new Atm();
        atm.Deposit([10, 10, 0, 0, 0]);

        // 60 could be reached as three $20s, but the greedy walk never tries that:
        // it takes one $50 first (leaving 10), and no smaller denomination covers 10.
        Assert.Equal(new long[] { -1 }, atm.Withdraw(60));

        // State must be untouched by the failed withdrawal above - the $20s are
        // still all there for this one to succeed.
        Assert.Equal(new long[] { 1, 0, 0, 0, 0 }, atm.Withdraw(20));
    }

    private sealed class Atm
    {
        private readonly HashMap<int, long> _countByDenomination = new();

        public Atm()
        {
            foreach (var denomination in DenominationsAscending)
            {
                _countByDenomination.Set(denomination, 0);
            }
        }

        public void Deposit(long[] banknotesCount)
        {
            for (var i = 0; i < DenominationsAscending.Length; i++)
            {
                _countByDenomination.TryGetValue(DenominationsAscending[i], out var count);
                _countByDenomination.Set(DenominationsAscending[i], count + banknotesCount[i]);
            }
        }

        // Ascending-index result (matching DenominationsAscending), [-1] on failure.
        // Feasibility is decided by a read-only greedy pass first so a failing
        // withdrawal never mutates _countByDenomination.
        public long[] Withdraw(long amount)
        {
            var used = new long[DenominationsAscending.Length];
            var remaining = amount;

            for (var i = DenominationsAscending.Length - 1; i >= 0; i--)
            {
                var (notes, updatedRemaining) = TakeNotes(DenominationsAscending[i], remaining);
                used[i] = notes;
                remaining = updatedRemaining;
            }

            if (remaining != 0)
            {
                return [-1];
            }

            for (var i = 0; i < DenominationsAscending.Length; i++)
            {
                _countByDenomination.TryGetValue(DenominationsAscending[i], out var available);
                _countByDenomination.Set(DenominationsAscending[i], available - used[i]);
            }

            return used;
        }

        private (long Notes, long Remaining) TakeNotes(int denomination, long remaining)
        {
            _countByDenomination.TryGetValue(denomination, out var available);
            var notes = Math.Min(available, remaining / denomination);
            return (notes, remaining - (notes * denomination));
        }
    }
}
