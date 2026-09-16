using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindMaximumNonDecreasingArrayLengthBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - the O(n^2) dp over every earlier cut point
// against the same recurrence driven by a Pareto stack plus one UpperBound lookup - so a harness
// whose arms disagree has counted two different partitions of the same array. Both answers are one
// int, so they are compared directly.
//
// Setup's values are random rather than sorted, which keeps the candidate stack non-trivial, and a
// partition of an array into contiguous blocks always exists here: every element is positive, so at
// worst each element is its own block and the answer is a length between one and Length.
public sealed partial class FindMaximumNonDecreasingArrayLengthBenchmarksTests
{
    // The smaller of Setup's [Params(200, 2_000)] lengths.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameMaximumLength() =>
        Assert.Equal(BuildHarness().BruteForceDp(), BuildHarness().BruteForceDp());

    [Fact]
    public void BruteForceDp_RandomPositiveValues_AgreesWithMonotonicStackBinarySearch()
    {
        var harness = BuildHarness();

        Assert.InRange(harness.BruteForceDp(), 1, SmallestLength);
        Assert.Equal(harness.MonotonicStackBinarySearch(), harness.BruteForceDp());
    }

    [Fact]
    public void MonotonicStackBinarySearch_RandomPositiveValues_AgreesWithBruteForceDp()
    {
        var harness = BuildHarness();

        Assert.InRange(harness.MonotonicStackBinarySearch(), 1, SmallestLength);
        Assert.Equal(harness.BruteForceDp(), harness.MonotonicStackBinarySearch());
    }

    private static FindMaximumNonDecreasingArrayLengthBenchmarks BuildHarness()
    {
        var harness = new FindMaximumNonDecreasingArrayLengthBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
