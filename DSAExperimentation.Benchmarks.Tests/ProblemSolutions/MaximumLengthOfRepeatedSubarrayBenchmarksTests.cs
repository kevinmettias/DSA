using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumLengthOfRepeatedSubarrayBenchmarks (ARCHITECTURE 17.9): Setup leaves
// both arrays at their default element value, which is the identical all-one-value shape the
// benchmark's own comment describes. Two identical length-n arrays share their entire run, so the
// longest common subarray is the whole of each and the answer is exactly the Length Setup drew - an
// oracle read off the fixture's layout, not off either arm's return, which the generic agreement
// check cannot supply. The baseline answers with a single length, compared directly.
public sealed partial class MaximumLengthOfRepeatedSubarrayBenchmarksTests
{
    private const int SmallestLength = 60;

    [Fact]
    public void BruteForce_IdenticalArrays_SpansTheWholeArray() =>
        Assert.Equal(SmallestLength, BuildHarness().BruteForce());

    private static MaximumLengthOfRepeatedSubarrayBenchmarks BuildHarness()
    {
        var harness = new MaximumLengthOfRepeatedSubarrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
