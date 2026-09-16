using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumNumberOfOperationsToMakeStringSortedBenchmarks (ARCHITECTURE
// 17.9): both arms are MinimumNumberOfOperationsToMakeStringSortedSolution's, the same methods
// MinimumNumberOfOperationsToMakeStringSortedTests proves correct, and both accumulate the same
// modular permutation-rank sum. They differ only in how the "how many remaining letters are
// smaller" query is answered - a linear frequency scan against this repo's FenwickTree - so
// arms that disagree are timing two different problems.
public sealed partial class MinimumNumberOfOperationsToMakeStringSortedBenchmarksTests
{
    // The smallest declared [Params] value: the frequency scan is the arm being justified, and
    // its per-position cost is what the longer string is there to exaggerate.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            BuildHarness().LinearFrequencyScan(),
            BuildHarness().LinearFrequencyScan());

    [Fact]
    public void LinearFrequencyScan_AgreesWithFenwickTreeSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FenwickTreeSweep(), harness.LinearFrequencyScan());
    }

    [Fact]
    public void FenwickTreeSweep_AgreesWithLinearFrequencyScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearFrequencyScan(), harness.FenwickTreeSweep());
    }

    private static MinimumNumberOfOperationsToMakeStringSortedBenchmarks BuildHarness()
    {
        var harness = new MinimumNumberOfOperationsToMakeStringSortedBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
