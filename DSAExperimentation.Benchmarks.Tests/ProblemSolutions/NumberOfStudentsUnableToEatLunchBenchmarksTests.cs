using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfStudentsUnableToEatLunchBenchmarks (ARCHITECTURE 17.9): both arms return
// the same number of students left unable to eat - the List<int> simulation whose every round pays
// RemoveAt(0) against the Deque-backed Queue<int> one - so a harness whose arms disagree is timing two
// different questions. The leftover count is the problem's whole answer rather than a proxy. Setup
// draws both the preference line and the sandwich pile from a fixed seed, so the same Length must
// rebuild both.
public sealed partial class NumberOfStudentsUnableToEatLunchBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().ListSimulation(), BuildHarness().ListSimulation());

    [Fact]
    public void ListSimulation_AgreesWithQueueStackSimulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.QueueStackSimulation(), harness.ListSimulation());
    }

    [Fact]
    public void QueueStackSimulation_AgreesWithListSimulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ListSimulation(), harness.QueueStackSimulation());
    }

    private static NumberOfStudentsUnableToEatLunchBenchmarks BuildHarness()
    {
        var harness = new NumberOfStudentsUnableToEatLunchBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
