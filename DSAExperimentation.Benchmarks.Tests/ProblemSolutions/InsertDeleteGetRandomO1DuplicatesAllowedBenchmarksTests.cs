using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for InsertDeleteGetRandomO1DuplicatesAllowedBenchmarks (ARCHITECTURE 17.9), for what
// BenchmarkArmsTests cannot pin: that the script empties the collection, which follows from Setup's construction
// rather than from either arm. Each arm replays the same fixed script against an instance it builds inside the call
// and returns every Insert verdict, then every Remove verdict. The removal order is a shuffled copy of the insertion
// order, so it carries exactly the same multiplicities and every removal finds an occurrence left to consume - with
// the fixture's narrow value range that means every duplicate occurrence was found and consumed. LC 381's Insert
// answers true only for a value not already present, so at most one insert per distinct value answers true.
public sealed partial class InsertDeleteGetRandomO1DuplicatesAllowedBenchmarksTests
{
    private const int SmallestCount = 200;

    // The benchmark draws every inserted value from this many distinct values.
    private const int DistinctValues = 10;

    [Fact]
    public void ListScan_InsertThenRemoveEveryOccurrence_EmptiesTheCollection() =>
        AssertEmptiesTheCollection(BuildHarness().ListScan());

    [Fact]
    public void LinkedOccurrences_InsertThenRemoveEveryOccurrence_EmptiesTheCollection() =>
        AssertEmptiesTheCollection(BuildHarness().LinkedOccurrences());

    private static void AssertEmptiesTheCollection(bool[] verdicts)
    {
        var inserts = verdicts[..SmallestCount];
        var removals = verdicts[SmallestCount..];

        Assert.InRange(inserts.Count(isNew => isNew), 1, DistinctValues);
        Assert.Equal(SmallestCount, removals.Length);
        Assert.DoesNotContain(false, removals);
    }

    private static InsertDeleteGetRandomO1DuplicatesAllowedBenchmarks BuildHarness()
    {
        var harness = new InsertDeleteGetRandomO1DuplicatesAllowedBenchmarks { Count = SmallestCount };
        harness.Setup();

        return harness;
    }
}
