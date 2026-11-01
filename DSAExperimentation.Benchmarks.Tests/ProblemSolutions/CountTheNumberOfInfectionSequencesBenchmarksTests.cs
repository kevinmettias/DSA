using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountTheNumberOfInfectionSequencesBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this asserts only
// a range that comes from the workload's shape rather than from either arm.
//
// The sick array is private, but its one documented property decides the answer's own range: exactly
// the two endpoints start sick, so only the remaining QueueLength - 2 people are ever infected and
// every infection sequence is an ordering of exactly those people.
public sealed partial class CountTheNumberOfInfectionSequencesBenchmarksTests
{
    private const int SmallestQueueLength = 12;

    // Every healthy person is infected exactly once, so at least one sequence always exists.
    private const long FewestInfectionSequences = 1;

    // Setup's sick array starts with exactly its two endpoints sick, so neither is ever infected.
    private const int InitiallySickEndpoints = 2;

    [Fact]
    public void GapCombinatorics_SickEndpointsOnly_StaysWithinTheOrderingsOfTheUninfected() =>
        Assert.InRange(
            BuildHarness().GapCombinatorics(SmallestQueueLength),
            FewestInfectionSequences,
            OrderingsOfUninfected(SmallestQueueLength));

    private static CountTheNumberOfInfectionSequencesBenchmarks BuildHarness()
    {
        var harness = new CountTheNumberOfInfectionSequencesBenchmarks();
        harness.Setup();

        return harness;
    }

    private static long OrderingsOfUninfected(int queueLength)
    {
        var orderings = 1L;

        for (var remaining = queueLength - InitiallySickEndpoints; remaining > 1; remaining--)
        {
            orderings *= remaining;
        }

        return orderings;
    }
}
