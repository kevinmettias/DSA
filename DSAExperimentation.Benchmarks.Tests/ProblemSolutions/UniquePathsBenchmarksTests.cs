using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for UniquePathsBenchmarks (ARCHITECTURE 17.9): its two arms are
// UniquePathsSolution's competing strategies for the same question - the closed-form binomial
// coefficient against the memoized grid recurrence - so a harness whose arms disagree is counting
// two different grids. Both arms answer with LC 62's own quantity, so agreement is agreement on the
// whole answer.
//
// On the smaller parameter the count is decisive: a 10 x 10 grid needs C(18, 9) = 48620 paths, and
// that literal is asserted alongside the agreement rather than left to a shared wrong number.
//
// The larger parameter, 18, is outside LC 62's guarantee that the answer fits an int - C(34, 17) is
// 2333606220 - and both arms return int, so they wrap to the same 32-bit value and still agree. That
// is a property of the parameters the class publishes, not of this harness, so the [Params] values
// are left exactly as they are and the observation is reported instead of acted on.
public sealed partial class UniquePathsBenchmarksTests
{
    // The smaller of the class's [Params(10, 18)] grid sizes.
    private const int SmallestSize = 10;

    // C(18, 9) = 48620: the paths across a 10 x 10 grid, 9 downs and 9 rights in any order.
    private const int ExpectedPathCount = 48_620;

    [Fact]
    public void Combinatorics_SmallestSize_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedPathCount, harness.Combinatorics());
        Assert.Equal(harness.MemoizedRecurrence(), harness.Combinatorics());
    }

    [Fact]
    public void MemoizedRecurrence_SmallestSize_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedPathCount, harness.MemoizedRecurrence());
        Assert.Equal(harness.Combinatorics(), harness.MemoizedRecurrence());
    }

    private static UniquePathsBenchmarks BuildHarness() => new() { Size = SmallestSize };
}
