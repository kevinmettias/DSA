using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ImplementTrieBenchmarks (ARCHITECTURE 17.9): the arm has no rival, so what
// needs pinning is the workload rather than a ratio. Setup's words are seeded, so the same word
// count must rebuild the same script; and the generated words are all the same length, so the
// prefix half of every query is a real prefix of the word it was drawn from rather than a slice
// that happened to fall inside it.
public sealed partial class ImplementTrieBenchmarksTests
{
    private const int SmallestWordCount = 1_000;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().InsertThenSearchAndStartsWith(), BuildHarness().InsertThenSearchAndStartsWith());

    // Every inserted word is found by search after the insert pass, which is only guaranteed if the
    // generated words are distinct and the trie round-trips them - a checksum of 2*WordCount is
    // exactly the whole set found by both HasKey and HasPrefix.
    [Fact]
    public void InsertThenSearchAndStartsWith_FindsTheWholeGeneratedSet() =>
        Assert.Equal(2 * SmallestWordCount, BuildHarness().InsertThenSearchAndStartsWith());

    private static ImplementTrieBenchmarks BuildHarness()
    {
        var harness = new ImplementTrieBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
