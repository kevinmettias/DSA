using DSAExperimentation.LeetCode.DesignAnATMMachine;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignAnATMMachine;

// One call in an ATM script: a bulk deposit or a withdrawal of a given amount.
// Pure dispatch, built via the named factories below so a script (like Examples
// in DesignAnATMMachineTests) reads like the LeetCode call sequence it replays.
public readonly record struct AtmOp(long[]? banknotesCount, long amount)
{
    public static AtmOp Deposit(long[] banknotesCount) => new(banknotesCount, 0);

    public static AtmOp Withdraw(long amount) => new(null, amount);

    // Deposit returns nothing in LeetCode's judge output, so it reports null here
    // and the expected sequence reads exactly like the published one.
    internal long[]? Apply(DesignAnATMMachineSolution.IAtm atm)
    {
        if (banknotesCount is not null)
        {
            atm.Deposit(banknotesCount);

            return null;
        }

        return atm.Withdraw(amount);
    }
}
