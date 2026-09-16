using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceBenchmarks (ARCHITECTURE
// 17.9): its two arms are competing strategies for the same question - a direct character
// comparison per word against an insert-then-HasPrefix round trip through the repo's own Trie - so
// a harness whose arms disagree is timing two different problems. Both arms answer with a single
// int, the 1-indexed position of the first word with the prefix or the problem's -1, so agreement
// between them says the two walks stopped at the same word.
public sealed partial class CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceBenchmarksTests
{
    private const int SmallestWordCount = 200;

    // The problem's own "no word starts with the search word" answer, and the value Setup's
    // workload must produce: the scenario's search word is longer than any generated word and
    // outside the generated alphabet's runs, so no word in the rebuilt sentence can start with it,
    // which is precisely why the workload forces both strategies through the whole word list.
    private const int NoPrefixWordFound = -1;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSentenceNoWordStartsWith()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        Assert.Equal(NoPrefixWordFound, first.StartsWithScan());
        Assert.Equal(NoPrefixWordFound, second.TriePerWordHasPrefix());
    }

    [Fact]
    public void StartsWithScan_UnmatchedSearchWord_AgreesWithTriePerWordHasPrefix()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TriePerWordHasPrefix(), harness.StartsWithScan());
    }

    [Fact]
    public void TriePerWordHasPrefix_UnmatchedSearchWord_AgreesWithStartsWithScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StartsWithScan(), harness.TriePerWordHasPrefix());
    }

    private static CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceBenchmarks BuildHarness()
    {
        var harness = new CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
