using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignAnATMMachineBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a raw five-slot array reached by index against
// this repo's own HashMap keyed by banknote value - so a harness whose arms disagree is timing
// two different problems. Setup draws every amount from one fixed seed and opens the machine with
// a billion notes of each denomination, so the same Calls must rebuild the same amount script,
// and every withdrawal in it must succeed - which is what each arm's returned total sums.
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

    [Fact]
    public void Setup_SameCallCount_RebuildsTheSameAmountScript()
    {
        var total = BuildHarness().FiveSlotArrayDispatch();

        // Both facts the reading depends on are visible in the returned total. Every amount is a
        // multiple of AmountGranularity and a machine carrying a billion notes of each
        // denomination can always make one, so a complete replay reports the requested amount to
        // the note - a whole number of hundreds - and lands near the middle of the draw rather
        // than at the zero a greedy walk rejecting amounts it can cover would leave behind.
        Assert.Equal(WholeAmountRemainder, total % AmountGranularity);
        Assert.InRange(
            total,
            SmallestCalls * MiddleHalfLowMultiplier * AmountGranularity,
            SmallestCalls * MiddleHalfHighMultiplier * AmountGranularity);
        Assert.Equal(total, BuildHarness().FiveSlotArrayDispatch());
    }

    [Fact]
    public void FiveSlotArrayDispatch_ThousandWithdrawals_AgreesWithHashMapDispatch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapDispatch(), harness.FiveSlotArrayDispatch());
    }

    [Fact]
    public void HashMapDispatch_ThousandWithdrawals_AgreesWithFiveSlotArrayDispatch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FiveSlotArrayDispatch(), harness.HashMapDispatch());
    }

    private static DesignAnATMMachineBenchmarks BuildHarness()
    {
        var harness = new DesignAnATMMachineBenchmarks { Calls = SmallestCalls };
        harness.Setup();

        return harness;
    }
}
