using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.DesignAnATMMachine;

// LeetCode 2241. Design an ATM Machine: five fixed banknote denominations - $20,
// $50, $100, $200 and $500 - deposited in bulk and withdrawn by a strict greedy
// walk from the largest denomination down. Because the walk is greedy rather than a
// coin-change search, a withdrawal can fail even when some other combination of the
// banknotes on hand would have covered the amount, and a failed withdrawal must
// leave the machine's contents exactly as it found them.
//
// This is a design problem - LeetCode's own shape is a stateful object with a
// constructor and two operations, not a single return value - so "every strategy
// for the problem" (ARCHITECTURE.md section 17.3) takes the form of two full
// classes implementing the shared IAtm surface below, the same shape
// DesignParkingSystemSolution uses for its own instance-API problem (LC 1603).
//
// Both are O(1) per call regardless of how large the denomination key space is; the
// question the pair answers is whether keying the counters by banknote value in
// this repo's own HashMap<int, long> costs anything measurable over plain index
// arithmetic into a five-slot array when the key space is, in this problem, always
// exactly five.
internal static class DesignAnATMMachineSolution
{
    // Ascending, which is the order LeetCode both deposits in and reports a
    // withdrawal in; the greedy walk reads it backwards.
    private static readonly int[] DenominationsAscending = [20, 50, 100, 200, 500];

    // How many counts a deposit vector carries and a successful withdrawal reports.
    public static int DenominationCount => DenominationsAscending.Length;

    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface IAtm
    {
        void Deposit(long[] banknotesCount);

        // Ascending-index counts of the banknotes dispensed, or LeetCode's [-1] when
        // the greedy walk cannot make the amount exactly.
        long[] Withdraw(long amount);
    }

    // The textbook baseline this composition has to justify itself against: one
    // five-slot array of counts reached by index, deliberately written without any
    // container from this repo at all.
    internal sealed class AtmByFiveSlotArray : IAtm
    {
        private readonly long[] _counts = new long[DenominationsAscending.Length];

        public void Deposit(long[] banknotesCount)
        {
            for (var i = 0; i < _counts.Length; i++)
            {
                _counts[i] += banknotesCount[i];
            }
        }

        public long[] Withdraw(long amount)
        {
            var used = new long[_counts.Length];
            var remaining = amount;

            for (var i = _counts.Length - 1; i >= 0; i--)
            {
                var notes = Math.Min(_counts[i], remaining / DenominationsAscending[i]);
                used[i] = notes;
                remaining -= notes * DenominationsAscending[i];
            }

            if (remaining != 0)
            {
                return [LeetCodeAnswer.None];
            }

            for (var i = 0; i < _counts.Length; i++)
            {
                _counts[i] -= used[i];
            }

            return used;
        }
    }

    // The composed answer: the same five counters held in this repo's own
    // HashMap<int, long> keyed by banknote value, so the machine's contents are
    // addressed by what a note is worth rather than by where it happens to sit.
    internal sealed class AtmByHashMap : IAtm
    {
        private readonly HashMap<int, long> _countByDenomination = new();

        public AtmByHashMap()
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

        // Feasibility is decided by a read-only greedy pass first, so a withdrawal
        // that cannot be made never touches _countByDenomination.
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
                return [LeetCodeAnswer.None];
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
