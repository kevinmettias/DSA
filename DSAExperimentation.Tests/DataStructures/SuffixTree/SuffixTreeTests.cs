using DSAExperimentation.DataStructures.SuffixTree;
using SuffixTreeStructure = DSAExperimentation.DataStructures.SuffixTree.SuffixTree;

namespace DSAExperimentation.Tests.DataStructures.SuffixTree;

public sealed partial class SuffixTreeTests
{
    private const string BananaText = "banana";
    private const string AbcAbdText = "abcabd";
    private const string AaaaText = "aaaa";
    private const string MississippiText = "mississippi";
    private const string AabaabaaabText = "aabaabaaab";
    private const string SingleCharZText = "z";
    private const string KnownSubstringNan = "nan";
    private const string KnownSubstringAna = "ana";
    private const string SingleCharacterPattern = "a";
    private const string AbsentSubstringWithTrailingS = "bananas";
    private const string AbsentPattern = "xyz";
    private const string KnownSubstringNx = "nx";
    private const string AbSubstring = "ab";
    private const string KnownSubstringAbc = "abc";
    private const string KnownSubstringAbd = "abd";
    private const string KnownSubstringCab = "cab";
    private const string MississippiPatternIss = "iss";
    private const string MississippiPatternSsi = "ssi";
    private const string MississippiPatternPpi = "ppi";
    private const string MississippiPatternMis = "mis";
    private const string NonSubstringSuffix = "x";
    private const int SecondAnaOccurrenceIndex = 3;
    private const int SecondAbOccurrenceIndex = 3;

    // Hand-verified worked example from the design pass: "banana" never triggers a split (only the
    // leaf-becomes-internal path), so this alone can't catch a split-ordering bug - see the
    // "abcabd" tests below for that.
    [Fact]
    public void HasSubstring_Banana_FindsKnownSubstrings()
    {
        var tree = new SuffixTreeStructure(BananaText);

        Assert.True(tree.HasSubstring(KnownSubstringNan));
        Assert.True(tree.HasSubstring(KnownSubstringAna));
        Assert.True(tree.HasSubstring(BananaText));
        Assert.True(tree.HasSubstring(SingleCharacterPattern));
        Assert.True(tree.HasSubstring(string.Empty));
    }

    [Fact]
    public void HasSubstring_Banana_RejectsAbsentSubstrings()
    {
        var tree = new SuffixTreeStructure(BananaText);

        Assert.False(tree.HasSubstring(AbsentSubstringWithTrailingS));
        Assert.False(tree.HasSubstring(AbsentPattern));
        Assert.False(tree.HasSubstring(KnownSubstringNx));
    }

    [Fact]
    public void FindOccurrences_Banana_ReturnsAllStartsForRepeatedSubstring()
    {
        var tree = new SuffixTreeStructure(BananaText);

        var occurrences = tree.FindOccurrences(KnownSubstringAna);

        Assert.Equal(new HashSet<int> { 1, SecondAnaOccurrenceIndex }, occurrences.ToHashSet());
    }

    [Fact]
    public void FindOccurrences_AbsentSubstring_ReturnsEmpty()
    {
        var tree = new SuffixTreeStructure(BananaText);

        Assert.Empty(tree.FindOccurrences(AbsentPattern));
    }

    [Fact]
    public void FindOccurrences_EmptyPattern_ReturnsEveryRealSuffixStart()
    {
        var tree = new SuffixTreeStructure(BananaText);

        var occurrences = tree.FindOccurrences(string.Empty);

        Assert.Equal(Enumerable.Range(0, BananaText.Length).ToHashSet(), occurrences.ToHashSet());
    }

    // "abcabd" (repeated substring "ab" at positions 0 and 3) specifically exercises the
    // mid-edge split branch - "banana" alone never triggers it.
    [Fact]
    public void HasSubstring_AbcAbd_FindsSubstringsAcrossASplitEdge()
    {
        var tree = new SuffixTreeStructure(AbcAbdText);

        Assert.True(tree.HasSubstring(AbSubstring));
        Assert.True(tree.HasSubstring(KnownSubstringAbc));
        Assert.True(tree.HasSubstring(KnownSubstringAbd));
        Assert.True(tree.HasSubstring(KnownSubstringCab));
        Assert.True(tree.HasSubstring(AbcAbdText));
    }

    [Fact]
    public void FindOccurrences_AbcAbd_ReturnsBothOccurrencesOfSplitSubstring()
    {
        var tree = new SuffixTreeStructure(AbcAbdText);

        var occurrences = tree.FindOccurrences(AbSubstring);

        Assert.Equal(new HashSet<int> { 0, SecondAbOccurrenceIndex }, occurrences.ToHashSet());
    }

    [Fact]
    public void HasSubstring_EmptyText_OnlyEmptyPatternMatches()
    {
        var tree = new SuffixTreeStructure(string.Empty);

        Assert.True(tree.HasSubstring(string.Empty));
        Assert.False(tree.HasSubstring(SingleCharacterPattern));
    }

    [Fact]
    public void FindOccurrences_EmptyText_EmptyPatternReturnsEmpty()
    {
        var tree = new SuffixTreeStructure(string.Empty);

        Assert.Empty(tree.FindOccurrences(string.Empty));
    }

    [Fact]
    public void Constructor_WithExplicitSuffixArray_ProducesTheSameResultAsTheConvenienceOverload()
    {
        const string text = "mississippi";
        var suffixArray = new DSAExperimentation.DataStructures.SuffixArray.SuffixArray(text);

        var fromExplicitArray = new SuffixTreeStructure(text, suffixArray);
        var fromConvenienceOverload = new SuffixTreeStructure(text);

        foreach (var pattern in new[]
                 {
                     MississippiPatternIss, MississippiPatternSsi, MississippiPatternPpi, MississippiPatternMis,
                     AbsentPattern,
                 })
        {
            Assert.Equal(fromConvenienceOverload.HasSubstring(pattern), fromExplicitArray.HasSubstring(pattern));
            Assert.Equal(
                fromConvenienceOverload.FindOccurrences(pattern).ToHashSet(),
                fromExplicitArray.FindOccurrences(pattern).ToHashSet());
        }
    }

    [Fact]
    public void HasSubstring_MatchesBruteForceSubstringCheckAcrossVariedInputsAndPatterns()
    {
        string[] texts = [BananaText, AbcAbdText, AaaaText, MississippiText, AabaabaaabText, SingleCharZText];

        foreach (var text in texts)
        {
            var tree = new SuffixTreeStructure(text);

            foreach (var pattern in CandidatePatterns(text))
            {
                var expectedContains = text.Contains(pattern, StringComparison.Ordinal);
                var actualHasSubstring = tree.HasSubstring(pattern);

                Assert.Equal(expectedContains, actualHasSubstring);
            }
        }
    }

    [Fact]
    public void FindOccurrences_MatchesBruteForceScanAcrossVariedInputsAndPatterns()
    {
        string[] texts = [BananaText, AbcAbdText, AaaaText, MississippiText, AabaabaaabText];

        foreach (var text in texts)
        {
            var tree = new SuffixTreeStructure(text);

            foreach (var pattern in CandidatePatterns(text).Where(p => p.Length > 0))
            {
                var expected = BruteForceFindOccurrences(text, pattern);
                var actual = tree.FindOccurrences(pattern).ToHashSet();

                Assert.Equal(expected, actual);
            }
        }
    }

    private static IEnumerable<string> CandidatePatterns(string text)
    {
        yield return string.Empty;

        for (var start = 0; start < text.Length; start++)
        {
            for (var length = 1; start + length <= text.Length; length++)
            {
                yield return text.Substring(start, length);
            }
        }

        yield return text + NonSubstringSuffix;
        yield return AbsentPattern;
    }

    private static HashSet<int> BruteForceFindOccurrences(string text, string pattern)
    {
        var occurrences = new HashSet<int>();

        for (var start = 0; start + pattern.Length <= text.Length; start++)
        {
            if (text.AsSpan(start, pattern.Length).SequenceEqual(pattern))
            {
                occurrences.Add(start);
            }
        }

        return occurrences;
    }
}
