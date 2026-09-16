using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumInsertionsToBalanceAParenthesesStringBenchmarks (ARCHITECTURE
// 17.9): both arms are MinimumInsertionsToBalanceAParenthesesStringSolution's, the same methods
// MinimumInsertionsToBalanceAParenthesesStringTests proves correct, and both answer the same
// question - the fewest insertions that balance the bracket string. Arms that disagree are
// timing two different problems.
//
// Setup's workload is seeded, so the same parameters must rebuild the same bracket string;
// otherwise two published numbers were never comparable in the first place.
public sealed partial class MinimumInsertionsToBalanceAParenthesesStringBenchmarksTests
{
    // The smallest declared [Params] value: the scalar counter and the opener stack run the
    // identical scan, so the string's length only changes how long the agreement check takes.
    private const int SmallestLength = 1_000;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            BuildHarness().RunningCounter(),
            BuildHarness().RunningCounter());

    [Fact]
    public void RunningCounter_AgreesWithStackOfOpeners()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StackOfOpeners(), harness.RunningCounter());
    }

    [Fact]
    public void StackOfOpeners_AgreesWithRunningCounter()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RunningCounter(), harness.StackOfOpeners());
    }

    private static MinimumInsertionsToBalanceAParenthesesStringBenchmarks BuildHarness()
    {
        var harness = new MinimumInsertionsToBalanceAParenthesesStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
