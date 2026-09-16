using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidArrangementOfPairsBenchmarks (ARCHITECTURE 17.9): its two arms are
// ValidArrangementOfPairsSolution's competing strategies for the same question - the same Hierholzer
// walk over a BCL Dictionary<int, List<int>> against this repo's HashMap<int, Stack<int>> - so a
// harness whose arms disagree is walking two different graphs.
//
// Both strategies consume each node's destination bucket from the back (List.RemoveAt(Count - 1)
// and Stack.TryPop are the same LIFO removal) and pick their start by scanning the input pairs in
// order, so the same pairs must produce the identical sequence; the comparison is deliberately
// order-sensitive because LC 2097's answer IS a path, where a reordered sequence would be a
// different - and possibly invalid - arrangement. Setup's pairs are random over a small node set and
// need not admit any valid arrangement, which the benchmark's own comment acknowledges, so this
// harness witnesses that both strategies traverse the given graph identically rather than that
// either returns a valid arrangement.
public sealed partial class ValidArrangementOfPairsBenchmarksTests
{
    // The smaller of Setup's [Params(200, 2_000)] pair counts.
    private const int SmallestPairCount = 200;

    [Fact]
    public void Setup_SamePairCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().DictionaryWithList()),
            AnswerText.Of(BuildHarness().DictionaryWithList()));

    [Fact]
    public void DictionaryWithList_SmallestPairCount_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.RepoHashMapWithStack()),
            AnswerText.Of(harness.DictionaryWithList()));
    }

    [Fact]
    public void RepoHashMapWithStack_SmallestPairCount_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.DictionaryWithList()),
            AnswerText.Of(harness.RepoHashMapWithStack()));
    }

    private static ValidArrangementOfPairsBenchmarks BuildHarness()
    {
        var harness = new ValidArrangementOfPairsBenchmarks { PairCount = SmallestPairCount };
        harness.Setup();

        return harness;
    }
}
