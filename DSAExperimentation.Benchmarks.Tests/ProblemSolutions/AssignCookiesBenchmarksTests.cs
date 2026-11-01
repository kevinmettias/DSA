using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AssignCookiesBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms to each
// other, and this bounds the matched count against the workload itself - no strategy can match more children
// than the array holds, a bound no comparison between arms can establish.
public sealed partial class AssignCookiesBenchmarksTests
{
    // The smaller of Setup's [Params(200, 3_000)] lengths; both arrays hold exactly this many
    // entries, so it is also the largest number of children any strategy can content.
    private const int SmallestLength = 200;

    [Fact]
    public void BruteForceScan_TwoHundredChildArray_ContentsNoMoreChildrenThanTheArrayHolds() =>
        Assert.InRange(BuildHarness().BruteForceScan(), 0, SmallestLength);

    private static AssignCookiesBenchmarks BuildHarness()
    {
        var harness = new AssignCookiesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
