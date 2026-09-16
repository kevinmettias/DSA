using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ExtraCharactersInAStringBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a full-length substring sweep against a trie walk
// that stops at the first dead prefix - so a harness whose arms disagree is minimising two
// different texts. Setup builds the text from 'k'..'o' while every dictionary word is built from
// 'a'..'j', so no word can cover a single character; that is what forces both arms through their
// full per-start scan instead of an early exact match, and it makes the answer decisive: every
// character of the text is an extra character. The same Length must rebuild the same text.
public sealed partial class ExtraCharactersInAStringBenchmarksTests
{
    private const int SmallestLength = 300;

    // Setup's characters never appear in the dictionary, so no word can cover any of them and the
    // minimum extra characters is the whole length.
    private const int ExpectedExtraCharacterCount = SmallestLength;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameText()
    {
        Assert.Equal(ExpectedExtraCharacterCount, BuildHarness().HashSetFullScan());

        Assert.Equal(BuildHarness().HashSetFullScan(), BuildHarness().HashSetFullScan());
    }

    [Fact]
    public void HashSetFullScan_UndictionaryableText_AgreesWithTriePrunedScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TriePrunedScan(), harness.HashSetFullScan());
    }

    [Fact]
    public void TriePrunedScan_UndictionaryableText_AgreesWithHashSetFullScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashSetFullScan(), harness.TriePrunedScan());
    }

    private static ExtraCharactersInAStringBenchmarks BuildHarness()
    {
        var harness = new ExtraCharactersInAStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
