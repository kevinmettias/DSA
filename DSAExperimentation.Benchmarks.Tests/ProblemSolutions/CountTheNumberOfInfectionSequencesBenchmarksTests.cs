using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountTheNumberOfInfectionSequencesBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - branching over every live candidate against the
// closed gap-combinatorics form - so a harness whose arms disagree is timing two different problems,
// not two ways of answering one. Setup derives the initially-sick set from QueueLength alone, so the
// same QueueLength must rebuild the same workload; otherwise two published numbers were never
// comparable in the first place.
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
    public void Setup_SameQueueLength_RebuildsTheSameSickSet()
    {
        Assert.InRange(
            BuildHarness().GapCombinatorics(),
            FewestInfectionSequences,
            OrderingsOfUninfected(SmallestQueueLength));
        Assert.Equal(BuildHarness().GapCombinatorics(), BuildHarness().GapCombinatorics());
    }

    [Fact]
    public void BruteForceSimulation_SickEndpointsOnly_AgreesWithGapCombinatorics()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GapCombinatorics(), harness.BruteForceSimulation());
    }

    [Fact]
    public void GapCombinatorics_SickEndpointsOnly_AgreesWithBruteForceSimulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceSimulation(), harness.GapCombinatorics());
    }

    private static CountTheNumberOfInfectionSequencesBenchmarks BuildHarness()
    {
        var harness = new CountTheNumberOfInfectionSequencesBenchmarks { QueueLength = SmallestQueueLength };
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
