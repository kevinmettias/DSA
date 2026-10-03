using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignAnATMMachineBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot pin:
// that every withdrawal in the script succeeds, which follows from Setup's construction rather than from either
// arm. Setup draws every amount from one fixed seed as a multiple of 100 and opens the machine with a billion notes
// of each denomination, so no withdrawal can fail, and each arm returns the notes every Withdraw handed back.
public sealed partial class DesignAnATMMachineBenchmarksTests
{
    private const int SmallestCalls = 1_000;

    // How much of the [1, 999] multiplier draw a complete replay is allowed to fall short of or
    // exceed; the uniform draw's own spread over SmallestCalls amounts is a fraction of a percent.
    private const long MiddleHalfLowMultiplier = 400;
    private const long MiddleHalfHighMultiplier = 600;

    private const int AmountGranularity = 100;

    // A complete replay dispenses whole amounts, so nothing is left over once they are counted in
    // units of the granularity every requested amount is a multiple of.
    private const long WholeAmountRemainder = 0;

    // LC 2241's banknote values, in the order Withdraw reports how many of each it dispensed.
    private static readonly long[] Denominations = [20, 50, 100, 200, 500];

    [Fact]
    public void FiveSlotArrayDispatch_ThousandWithdrawals_DispensesEveryRequestedAmount() =>
        AssertDispensesEveryRequestedAmount(BuildHarness().FiveSlotArrayDispatch());

    [Fact]
    public void HashMapDispatch_ThousandWithdrawals_DispensesEveryRequestedAmount() =>
        AssertDispensesEveryRequestedAmount(BuildHarness().HashMapDispatch());

    // A machine carrying a billion notes of each denomination can always make a whole number of
    // hundreds, so no withdrawal is LC 2241's [-1] refusal, and the money dispensed lands near the
    // middle of the draw rather than at the zero a greedy walk rejecting amounts it can cover would
    // leave behind.
    private static void AssertDispensesEveryRequestedAmount(long[][] withdrawn)
    {
        Assert.All(withdrawn, notes => Assert.NotEqual(LeetCodeAnswer.None, notes[0]));

        var total = withdrawn.Sum(notes => notes.Zip(Denominations, (count, value) => count * value).Sum());

        Assert.Equal(WholeAmountRemainder, total % AmountGranularity);
        Assert.InRange(
            total,
            SmallestCalls * MiddleHalfLowMultiplier * AmountGranularity,
            SmallestCalls * MiddleHalfHighMultiplier * AmountGranularity);
    }

    private static DesignAnATMMachineBenchmarks BuildHarness()
    {
        var harness = new DesignAnATMMachineBenchmarks { Calls = SmallestCalls };
        harness.Setup();

        return harness;
    }
}
