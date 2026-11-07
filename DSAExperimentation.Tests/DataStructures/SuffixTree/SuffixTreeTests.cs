using DSAExperimentation.DataStructures.SubstringIndex;
using DSAExperimentation.DataStructures.SuffixTree;
using DSAExperimentation.Tests.DataStructures.SubstringIndex.Fixtures;
using SuffixArrayStructure = DSAExperimentation.DataStructures.SuffixArray.SuffixArray;
using SuffixTreeStructure = DSAExperimentation.DataStructures.SuffixTree.SuffixTree;

namespace DSAExperimentation.Tests.DataStructures.SuffixTree;

public sealed partial class SuffixTreeTests
{
    public static TheoryData<string> TextNames => new(SubstringIndexTexts.Names);

    // Hand-verified worked example from the design pass: "banana" never triggers a split (only the
    // leaf-becomes-internal path), so this alone can't catch a split-ordering bug - see the
    // "abcabd" tests below for that.
    [Fact]
    public void HasSubstring_Banana_FindsKnownSubstrings()
    {
        var tree = new SuffixTreeStructure(Fixtures.BananaText);

        Assert.True(tree.HasSubstring(Fixtures.KnownSubstringNan));
        Assert.True(tree.HasSubstring(Fixtures.KnownSubstringAna));
        Assert.True(tree.HasSubstring(Fixtures.BananaText));
        Assert.True(tree.HasSubstring(Fixtures.SingleCharacterPattern));
        Assert.True(tree.HasSubstring(string.Empty));
    }

    [Fact]
    public void HasSubstring_Banana_RejectsAbsentSubstrings()
    {
        var tree = new SuffixTreeStructure(Fixtures.BananaText);

        Assert.False(tree.HasSubstring(Fixtures.AbsentSubstringWithTrailingS));
        Assert.False(tree.HasSubstring(Fixtures.AbsentPattern));
        Assert.False(tree.HasSubstring(Fixtures.KnownSubstringNx));
    }

    [Fact]
    public void FindOccurrences_Banana_ReturnsAllStartsForRepeatedSubstring()
    {
        var tree = new SuffixTreeStructure(Fixtures.BananaText);

        var occurrences = tree.FindOccurrences(Fixtures.KnownSubstringAna);

        Assert.Equal(
            new HashSet<int> { 1, Fixtures.SecondAnaOccurrenceIndex }, occurrences.ToHashSet());
    }

    [Fact]
    public void FindOccurrences_AbsentSubstring_ReturnsEmpty()
    {
        var tree = new SuffixTreeStructure(Fixtures.BananaText);

        Assert.Empty(tree.FindOccurrences(Fixtures.AbsentPattern));
    }

    // The empty pattern occurs at every insertion point, the text's end included, though no leaf
    // stands for the empty suffix there.
    [Fact]
    public void FindOccurrences_EmptyPattern_ReturnsEveryInsertionPoint()
    {
        var tree = new SuffixTreeStructure(Fixtures.BananaText);

        var occurrences = tree.FindOccurrences(string.Empty);

        var insertionPoints = Enumerable.Range(0, Fixtures.BananaText.Length + 1).ToHashSet();

        Assert.Equal(insertionPoints, occurrences.ToHashSet());
    }

    // "ana" ends mid-edge, on the way to the node for "anana"'s branch, and both suffixes that
    // start with it run through that node.
    [Fact]
    public void CountOccurrences_Banana_CountsAnaTwiceThoughTheyOverlap() =>
        Assert.Equal(
            Fixtures.AnaOccurrences,
            new SuffixTreeStructure(Fixtures.BananaText)
                .CountOccurrences(Fixtures.KnownSubstringAna));

    [Fact]
    public void CountOccurrences_AbsentPattern_IsZero() =>
        Assert.Equal(
            0,
            new SuffixTreeStructure(Fixtures.BananaText).CountOccurrences(Fixtures.AbsentPattern));

    [Fact]
    public void CountOccurrences_EmptyPattern_CountsEveryInsertionPoint() =>
        Assert.Equal(
            Fixtures.BananaText.Length + 1,
            new SuffixTreeStructure(Fixtures.BananaText).CountOccurrences(string.Empty));

    // "ab" at 0 and 3 is the deepest node two suffixes pass through, and it was made by a split.
    [Fact]
    public void FindLongestRepeat_AbcAbd_IsTheSplitNodesAb()
    {
        var repeat = new SuffixTreeStructure(Fixtures.AbcAbdText).FindLongestRepeat();

        Assert.Equal(
            Fixtures.AbSubstring, Fixtures.AbcAbdText.Substring(repeat.Start, repeat.Length));
    }

    [Fact]
    public void FindLongestRepeat_EmptyText_IsTheEmptySubstring() =>
        Assert.Equal(
            SubstringOccurrence.Empty, new SuffixTreeStructure(string.Empty).FindLongestRepeat());

    // The tree's leaves in character order are the suffix array, and the depths between neighbours
    // its common prefix lengths: the conversion must agree with the array built from the text.
    [Theory]
    [MemberData(nameof(TextNames))]
    public void ToSuffixArray_EveryText_IsTheSuffixArrayOfTheText(string textName)
    {
        var text = SubstringIndexTexts.Named(textName);
        var expected = new SuffixArrayStructure(text);

        var converted = new SuffixTreeStructure(text).ToSuffixArray();

        Assert.Equal(ArraysOf(expected), ArraysOf(converted));
    }

    // Array to tree is the constructor; tree to array is ToSuffixArray. A round trip changes
    // nothing.
    [Theory]
    [MemberData(nameof(TextNames))]
    public void ToSuffixArray_TreeBuiltFromAnArray_GivesThatArrayBack(string textName)
    {
        var text = SubstringIndexTexts.Named(textName);
        var original = new SuffixArrayStructure(text);

        var roundTrip = new SuffixTreeStructure(text, original).ToSuffixArray();

        Assert.Equal(ArraysOf(original), ArraysOf(roundTrip));
    }

    // "abcabd" (repeated substring "ab" at positions 0 and 3) specifically exercises the
    // mid-edge split branch - "banana" alone never triggers it.
    [Fact]
    public void HasSubstring_AbcAbd_FindsSubstringsAcrossASplitEdge()
    {
        var tree = new SuffixTreeStructure(Fixtures.AbcAbdText);

        Assert.True(tree.HasSubstring(Fixtures.AbSubstring));
        Assert.True(tree.HasSubstring(Fixtures.KnownSubstringAbc));
        Assert.True(tree.HasSubstring(Fixtures.KnownSubstringAbd));
        Assert.True(tree.HasSubstring(Fixtures.KnownSubstringCab));
        Assert.True(tree.HasSubstring(Fixtures.AbcAbdText));
    }

    [Fact]
    public void FindOccurrences_AbcAbd_ReturnsBothOccurrencesOfSplitSubstring()
    {
        var tree = new SuffixTreeStructure(Fixtures.AbcAbdText);

        var occurrences = tree.FindOccurrences(Fixtures.AbSubstring);

        Assert.Equal(
            new HashSet<int> { 0, Fixtures.SecondAbOccurrenceIndex }, occurrences.ToHashSet());
    }

    [Fact]
    public void HasSubstring_EmptyText_OnlyEmptyPatternMatches()
    {
        var tree = new SuffixTreeStructure(string.Empty);

        Assert.True(tree.HasSubstring(string.Empty));
        Assert.False(tree.HasSubstring(Fixtures.SingleCharacterPattern));
    }

    [Fact]
    public void FindOccurrences_EmptyText_EmptyPatternOccursOnceAtZero()
    {
        var tree = new SuffixTreeStructure(string.Empty);

        Assert.Equal([0], tree.FindOccurrences(string.Empty));
    }

    [Fact]
    public void Constructor_WithExplicitSuffixArray_ProducesTheSameResultAsTheConvenienceOverload()
    {
        var suffixArray =
            new DSAExperimentation.DataStructures.SuffixArray.SuffixArray(Fixtures.MississippiText);

        var fromExplicitArray = new SuffixTreeStructure(Fixtures.MississippiText, suffixArray);
        var fromConvenienceOverload = new SuffixTreeStructure(Fixtures.MississippiText);

        foreach (var pattern in new[]
                 {
                     Fixtures.MississippiPatternIss, Fixtures.MississippiPatternSsi,
                     Fixtures.MississippiPatternPpi, Fixtures.MississippiPatternMis,
                     Fixtures.AbsentPattern,
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
        string[] texts =
            [Fixtures.BananaText, Fixtures.AbcAbdText, Fixtures.AaaaText, Fixtures.MississippiText,
                Fixtures.AabaabaaabText, Fixtures.SingleCharZText];

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
        string[] texts =
            [Fixtures.BananaText, Fixtures.AbcAbdText, Fixtures.AaaaText, Fixtures.MississippiText,
                Fixtures.AabaabaaabText];

        foreach (var text in texts)
        {
            var tree = new SuffixTreeStructure(text);

            foreach (var pattern in CandidatePatterns(text).Where(p => p.Length > 0))
            {
                var expected = BruteForceFindOccurrences(text, new SearchPattern(pattern));
                var actual = tree.FindOccurrences(pattern).ToHashSet();

                Assert.Equal(expected, actual);
            }
        }
    }

    private static int[][] ArraysOf(SuffixArrayStructure suffixArray) =>
        [suffixArray.Suffixes, suffixArray.Rank, suffixArray.LongestCommonPrefixArray];

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

        yield return text + Fixtures.NonSubstringSuffix;
        yield return Fixtures.AbsentPattern;
    }

    // The pattern a brute-force scan counts occurrences of. Its own type because `text` and
    // `pattern` would otherwise both be `string`: the scan is not symmetric, so
    // `BruteForceFindOccurrences(pattern, text)` would compile and quietly answer a different
    // question. With the role named, that call no longer compiles.
    private readonly record struct SearchPattern(string Value);

    private static HashSet<int> BruteForceFindOccurrences(string text, SearchPattern pattern)
    {
        var occurrences = new HashSet<int>();

        for (var start = 0; start + pattern.Value.Length <= text.Length; start++)
        {
            if (text.AsSpan(start, pattern.Value.Length).SequenceEqual(pattern.Value))
            {
                occurrences.Add(start);
            }
        }

        return occurrences;
    }

    /// <summary>
    /// The texts these tests build suffix trees from, the patterns they search for, and the
    /// occurrence indices they expect back, named once so a second test does not have to reach
    /// into a neighbour's body for them. A holder of its own because twenty-four constants
    /// beside the tests are a second subject, and the tests read as tests only once they are out
    /// of the way.
    /// </summary>
    private static class Fixtures
    {
        public const string BananaText = "banana";
        public const string AbcAbdText = "abcabd";
        public const string AaaaText = "aaaa";
        public const string MississippiText = "mississippi";
        public const string AabaabaaabText = "aabaabaaab";
        public const string SingleCharZText = "z";

        public const string KnownSubstringNan = "nan";
        public const string KnownSubstringAna = "ana";
        public const string KnownSubstringNx = "nx";
        public const string KnownSubstringAbc = "abc";
        public const string KnownSubstringAbd = "abd";
        public const string KnownSubstringCab = "cab";

        public const string SingleCharacterPattern = "a";
        public const string AbsentPattern = "xyz";
        public const string AbsentSubstringWithTrailingS = "bananas";
        public const string NonSubstringSuffix = "x";
        public const string AbSubstring = "ab";

        public const string MississippiPatternIss = "iss";
        public const string MississippiPatternSsi = "ssi";
        public const string MississippiPatternPpi = "ppi";
        public const string MississippiPatternMis = "mis";

        public const int SecondAnaOccurrenceIndex = 3;
        public const int SecondAbOccurrenceIndex = 3;
        public const int AnaOccurrences = 2;
    }
}
