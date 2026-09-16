using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ClosestDivisorsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the full-range divisor scan against an anchor at
// floor(sqrt(candidate)) found with BinarySearch.LowerBound - so a harness whose arms disagree is
// timing two different problems. Both arms answer with an (int, int) value tuple, the closest pair of
// divisors, and the tuple is compared as itself rather than through AnswerText: it is an ordered value
// of two scalars whose own structural equality already compares both members, so no rendering is
// needed to keep a pair scored against its own candidate.
public sealed partial class ClosestDivisorsBenchmarksTests
{
    private const int SmallestNumber = 1_000;

    [Fact]
    public void BruteForce_NumberEqualsOneThousand_AgreesWithBinarySearchAnchored()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchAnchored(), harness.BruteForce());
    }

    [Fact]
    public void BinarySearchAnchored_NumberEqualsOneThousand_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.BinarySearchAnchored());
    }

    private static ClosestDivisorsBenchmarks BuildHarness() =>
        new ClosestDivisorsBenchmarks { Number = SmallestNumber };
}
