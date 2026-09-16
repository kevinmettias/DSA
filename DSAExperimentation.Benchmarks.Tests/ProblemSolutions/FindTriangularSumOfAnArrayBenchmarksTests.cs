using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTriangularSumOfAnArrayBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question, so a harness whose arms disagree is timing two
// different problems. Setup draws the digit row from a fixed seed, so the same Length must rebuild
// the same workload - and the row is safe to replay in either order because the in-place arm
// clones the caller's array rather than reducing it.
public sealed partial class FindTriangularSumOfAnArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().InPlaceArrayReduction()),
            AnswerText.Of(BuildHarness().InPlaceArrayReduction()));

    [Fact]
    public void InPlaceArrayReduction_AgreesWithDynamicArrayReduction()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DynamicArrayReduction(), harness.InPlaceArrayReduction());
    }

    [Fact]
    public void DynamicArrayReduction_AgreesWithInPlaceArrayReduction()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.InPlaceArrayReduction(), harness.DynamicArrayReduction());
    }

    private static FindTriangularSumOfAnArrayBenchmarks BuildHarness()
    {
        var harness = new FindTriangularSumOfAnArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
