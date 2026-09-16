using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BasicCalculatorBenchmarks (ARCHITECTURE 17.9): its two arms are BasicCalculatorSolution's
// competing strategies for the same question - recursive descent against a stack scan over the same grammar - so a
// harness whose arms disagree has evaluated two different expressions. Both return the one integer total, so they
// are compared directly rather than through a rendering; the generated expression chains sequential single-level
// groups off one fixed length, so Setup has nothing random in it and the same Length must rebuild the same text.
public sealed partial class BasicCalculatorBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] expression lengths.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().RecursiveDescent(), BuildHarness().RecursiveDescent());

    [Fact]
    public void RecursiveDescent_TwoHundredCharacterExpression_AgreesWithStackScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StackScan(), harness.RecursiveDescent());
    }

    [Fact]
    public void StackScan_TwoHundredCharacterExpression_AgreesWithRecursiveDescent()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RecursiveDescent(), harness.StackScan());
    }

    private static BasicCalculatorBenchmarks BuildHarness()
    {
        var harness = new BasicCalculatorBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
