using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AssignCookiesBenchmarks (ARCHITECTURE 17.9): its two arms are AssignCookiesSolution's
// competing strategies for the same question - a per-child rescan against a sort and two-pointer sweep - so a
// harness whose arms disagree has contented two different sets of children. The matched count is also bounded
// against the workload itself: no strategy can match more children than the array holds, which is asserted
// alongside the arms' agreement rather than resting on it. Setup draws both arrays from one seed over the same
// range, so the same Length must rebuild the same greed and sizes.
public sealed partial class AssignCookiesBenchmarksTests
{
    // The smaller of Setup's [Params(200, 3_000)] lengths; both arrays hold exactly this many
    // entries, so it is also the largest number of children any strategy can content.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceScan(), BuildHarness().BruteForceScan());

    [Fact]
    public void BruteForceScan_TwoHundredChildArray_AgreesWithSortThenTwoPointer()
    {
        var harness = BuildHarness();

        Assert.InRange(harness.BruteForceScan(), 0, SmallestLength);
        Assert.Equal(harness.SortThenTwoPointer(), harness.BruteForceScan());
    }

    [Fact]
    public void SortThenTwoPointer_TwoHundredChildArray_AgreesWithBruteForceScan()
    {
        var harness = BuildHarness();

        Assert.InRange(harness.SortThenTwoPointer(), 0, SmallestLength);
        Assert.Equal(harness.BruteForceScan(), harness.SortThenTwoPointer());
    }

    private static AssignCookiesBenchmarks BuildHarness()
    {
        var harness = new AssignCookiesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
