using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfStepsToReduceANumberInBinaryRepresentationToOneBenchmarks
// (ARCHITECTURE 17.9): both arms return the step count for the same binary string - the "+1 as a full
// binary addition" simulation against the single-pass carry-propagation scan - so a harness whose arms
// disagree is timing two different questions. The step count is the problem's whole answer rather than
// a proxy. Setup repeats the alternating "10" unit, so the same Length must rebuild the same string.
public sealed partial class NumberOfStepsToReduceANumberInBinaryRepresentationToOneBenchmarksTests
{
    private const int SmallestLength = 100;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().StackAddSimulation(), BuildHarness().StackAddSimulation());

    [Fact]
    public void StackAddSimulation_AgreesWithCarryPropagationScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CarryPropagationScan(), harness.StackAddSimulation());
    }

    [Fact]
    public void CarryPropagationScan_AgreesWithStackAddSimulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StackAddSimulation(), harness.CarryPropagationScan());
    }

    private static NumberOfStepsToReduceANumberInBinaryRepresentationToOneBenchmarks BuildHarness()
    {
        var harness = new NumberOfStepsToReduceANumberInBinaryRepresentationToOneBenchmarks
        {
            Length = SmallestLength,
        };
        harness.Setup();

        return harness;
    }
}
