using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SumOfTwoIntegersBenchmarks (ARCHITECTURE 17.9): both arms answer with a bare
// int, and the pair is the class's own fixed second addend against the smaller of its two
// FirstAddend values, so the sum is a constant rather than merely agreed. The baseline is pinned to
// it, since two arms that were both wrong in the same way would still pass the generic arm check's
// agreement.
public sealed partial class SumOfTwoIntegersBenchmarksTests
{
    // Mirrors SumOfTwoIntegersBenchmarks' own private SecondAddend.
    private const int SecondAddend = -1_000;
    private const int SmallestFirstAddend = 100;
    private const int ExpectedSum = SmallestFirstAddend + SecondAddend;

    [Fact]
    public void RippleCarryAdder_SmallestAddendPair_ReturnsTheirSum() =>
        Assert.Equal(ExpectedSum, BuildHarness().RippleCarryAdder());

    private static SumOfTwoIntegersBenchmarks BuildHarness() =>
        new() { FirstAddend = SmallestFirstAddend };
}
