using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountTheRepetitionsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the full O(N1 * |S1|) walk against the pigeonhole cycle
// shortcut - so a harness whose arms disagree is timing two different problems, not two ways of
// answering one. Setup rebuilds the same s1 on every call, so two published numbers over the same
// parameters were comparable in the first place.
//
// s1 is private, but the reading depends on its size in a way the answer itself reports: s1 is the
// 2-character block "ab" repeated 25 times, so [s1, N1] holds SmallestN1 * 50 characters and can
// contain at most that many / 2 non-overlapping copies of the 2-character s2. Note that Setup reads
// only constants - N1 sizes the [s1, N1] repetition at the call site, not the block it repeats - so
// the same s1 is rebuilt for every N1.
public sealed partial class CountTheRepetitionsBenchmarksTests
{
    private const int SmallestN1 = 5_000;

    private const int S1BuildingBlockLength = 2;

    private const int S1BuildingBlockRepeatCount = 25;

    private const int S2Length = 2;

    private const long MaxRepetitions =
        (long)SmallestN1 * S1BuildingBlockLength * S1BuildingBlockRepeatCount / S2Length;

    // s1 alternates, so it holds "ba" at every other position - the repetitions being counted
    // are there to be found, not a degenerate zero.
    private const int FewestRepetitions = 1;

    [Fact]
    public void Setup_SameN1_RebuildsTheSameS1()
    {
        Assert.InRange(BuildHarness().NaiveFullSimulation(), FewestRepetitions, MaxRepetitions);
        Assert.Equal(BuildHarness().NaiveFullSimulation(), BuildHarness().NaiveFullSimulation());
    }

    [Fact]
    public void NaiveFullSimulation_AlternatingBlockAndNoN2Repetition_AgreesWithHashMapCycleDetection()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapCycleDetection(), harness.NaiveFullSimulation());
    }

    [Fact]
    public void HashMapCycleDetection_AlternatingBlockAndNoN2Repetition_AgreesWithNaiveFullSimulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveFullSimulation(), harness.HashMapCycleDetection());
    }

    private static CountTheRepetitionsBenchmarks BuildHarness()
    {
        var harness = new CountTheRepetitionsBenchmarks { N1 = SmallestN1 };
        harness.Setup();

        return harness;
    }
}
