using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignAStackWithIncrementOperationBenchmarks (ARCHITECTURE 17.9): the generic arm
// check compares the arms only with each other, so what stays here is a bound derived without either arm.
// Both arms report the top of the stack after the replay, which is the last push: the increments are all
// applied once every value is already on the stack, so they only ever reach its bottom window and never
// the top. That makes the reading a pushed value from Setup's own domain.
public sealed partial class DesignAStackWithIncrementOperationBenchmarksTests
{
    private const int SmallestPushCount = 200;

    // [GlobalSetup] draws every pushed value from this half-open range.
    private const int MinPushedValue = 1;
    private const int MaxPushedValueExclusive = 1_000;

    // The top is never inside the increment window, so it keeps the value it was pushed with.
    private const int ExpectedTopValueLowerBound = MinPushedValue;
    private const int ExpectedTopValueUpperBound = MaxPushedValueExclusive - 1;

    [Fact]
    public void IndexedListIncrement_TwoHundredPushes_TopKeepsAPushedValue() =>
        Assert.InRange(
            BuildHarness().IndexedListIncrement(),
            ExpectedTopValueLowerBound,
            ExpectedTopValueUpperBound);

    private static DesignAStackWithIncrementOperationBenchmarks BuildHarness()
    {
        var harness = new DesignAStackWithIncrementOperationBenchmarks { PushCount = SmallestPushCount };
        harness.Setup();

        return harness;
    }
}
