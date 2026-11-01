using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindMaximumNonDecreasingArrayLengthBenchmarks (ARCHITECTURE 17.9): the generic
// arms check only compares the arms with each other, so this bounds the baseline's answer by the
// workload itself.
//
// Setup's values are random rather than sorted, which keeps the candidate stack non-trivial, and a
// partition of an array into contiguous blocks always exists here: every element is positive, so at
// worst each element is its own block and the answer is a length between one and Length.
public sealed partial class FindMaximumNonDecreasingArrayLengthBenchmarksTests
{
    // The smaller of Setup's [Params(200, 2_000)] lengths.
    private const int SmallestLength = 200;

    [Fact]
    public void BruteForceDp_RandomPositiveValues_ReturnsALengthWithinTheArray() =>
        Assert.InRange(BuildHarness().BruteForceDp(), 1, SmallestLength);

    private static FindMaximumNonDecreasingArrayLengthBenchmarks BuildHarness()
    {
        var harness = new FindMaximumNonDecreasingArrayLengthBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
