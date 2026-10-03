using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ImplementTrieBenchmarks (ARCHITECTURE 17.9): the arm has no rival, so what
// needs pinning is the workload rather than a ratio. The generated words are all the same length, so
// the prefix half of every query is a real prefix of the word it was drawn from rather than a slice
// that happened to fall inside it. The arm returns every verdict, each query's search then its
// startsWith.
public sealed partial class ImplementTrieBenchmarksTests
{
    private const int SmallestWordCount = 1_000;

    // Each query asks search and then startsWith.
    private const int VerdictsPerQuery = 2;

    // Every inserted word is found by search after the insert pass, which is only guaranteed if the
    // generated words are distinct and the trie round-trips them - all 2*WordCount verdicts true is
    // exactly the whole set found by both HasKey and HasPrefix.
    [Fact]
    public void InsertThenSearchAndStartsWith_FindsTheWholeGeneratedSet()
    {
        var verdicts = BuildHarness().InsertThenSearchAndStartsWith();

        Assert.Equal(VerdictsPerQuery * SmallestWordCount, verdicts.Length);
        Assert.DoesNotContain(false, verdicts);
    }

    private static ImplementTrieBenchmarks BuildHarness()
    {
        var harness = new ImplementTrieBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
