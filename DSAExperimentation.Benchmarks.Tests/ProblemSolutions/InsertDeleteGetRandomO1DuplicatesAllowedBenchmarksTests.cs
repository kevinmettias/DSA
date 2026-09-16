using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for InsertDeleteGetRandomO1DuplicatesAllowedBenchmarks (ARCHITECTURE 17.9).
// Both arms are competing strategies for the same question - a BCL List scanned on every insert
// and shifted on every removal against a per-value occurrence list - so they are asserted to
// agree. Each arm replays the same fixed script against an instance it builds inside the call (a
// Design problem's whole shape is a sequence of mutating calls against one instance), so one
// harness is safe to call twice in either order. Each arm returns the collection's surviving
// Count, a proxy for the collection itself: the removal order is a copy of the insertion order,
// so the script removes exactly the multiset it inserted and the honest claim is not merely that
// the arms agree but that both emptied the collection - with the fixture's narrow value range
// that means every duplicate occurrence was found and consumed, which a count-only agreement
// alone would not catch.
public sealed partial class InsertDeleteGetRandomO1DuplicatesAllowedBenchmarksTests
{
    private const int SmallestCount = 200;

    // The removal order is a shuffled copy of the insertion order, so it carries exactly the same
    // multiplicities and every inserted occurrence is removed again.
    private const int ExpectedSurvivingCount = 0;

    [Fact]
    public void Setup_SameCount_RebuildsTheSameScript()
    {
        Assert.Equal(BuildHarness().ListScan(), BuildHarness().ListScan());
        Assert.Equal(BuildHarness().LinkedOccurrences(), BuildHarness().LinkedOccurrences());
    }

    [Fact]
    public void ListScan_SurvivingCount_AgreesWithLinkedOccurrences()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinkedOccurrences(), harness.ListScan());
        Assert.Equal(ExpectedSurvivingCount, harness.ListScan());
    }

    [Fact]
    public void LinkedOccurrences_SurvivingCount_AgreesWithListScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ListScan(), harness.LinkedOccurrences());
        Assert.Equal(ExpectedSurvivingCount, harness.LinkedOccurrences());
    }

    private static InsertDeleteGetRandomO1DuplicatesAllowedBenchmarks BuildHarness()
    {
        var harness = new InsertDeleteGetRandomO1DuplicatesAllowedBenchmarks { Count = SmallestCount };
        harness.Setup();

        return harness;
    }
}
