using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountValidSequencesBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing on a rebuilt workload, so this asserts only an expected value derived
// from the problem's closed form rather than from either arm.
//
// There is no seed here: the whole workload is the TargetSum parameter together with the two things
// Setup derives from it - length = TargetSum / 2 and a factorial table that reaches TargetSum. The
// only report either arm makes is the answer, so the documented shape is asserted through that
// answer's own documented value: LC 4002 counts sequences of length k = n / 2 of positive integers
// summing to n with an even product as C(n - 1, k - 1) - C((n + k) / 2 - 1, k - 1) mod 1e9+7, which
// for n = 1000 is C(999, 499) - C(749, 499). A length other than n / 2, or a table that stops short
// of n, changes that residue.
public sealed partial class CountValidSequencesBenchmarksTests
{
    private const int SmallestTargetSum = 1_000;

    private const int ExpectedEvenProductSequenceCount = 498_280_751;

    [Fact]
    public void DirectBinomial_HalfLengthSequences_MatchesTheDocumentedResidue() =>
        Assert.Equal(ExpectedEvenProductSequenceCount, BuildHarness().DirectBinomial());

    private static CountValidSequencesBenchmarks BuildHarness()
    {
        var harness = new CountValidSequencesBenchmarks { TargetSum = SmallestTargetSum };
        harness.Setup();

        return harness;
    }
}
