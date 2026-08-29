using DSAExperimentation.DataStructures.SuffixTree;
using SuffixTreeStructure = DSAExperimentation.DataStructures.SuffixTree.SuffixTree;

namespace DSAExperimentation.Tests.DataStructures.SuffixTree;

public sealed partial class SuffixTreeTests
{
    // Hand-verified worked example from the design pass: "banana" never triggers a split (only the
    // leaf-becomes-internal path), so this alone can't catch a split-ordering bug - see the
    // "abcabd" tests below for that.
    [Fact]
    public void HasSubstring_Banana_FindsKnownSubstrings()
    {
        var tree = new SuffixTreeStructure("banana");

        Assert.True(tree.HasSubstring("nan"));
        Assert.True(tree.HasSubstring("ana"));
        Assert.True(tree.HasSubstring("banana"));
        Assert.True(tree.HasSubstring("a"));
        Assert.True(tree.HasSubstring(""));
    }

    [Fact]
    public void HasSubstring_Banana_RejectsAbsentSubstrings()
    {
        var tree = new SuffixTreeStructure("banana");

        Assert.False(tree.HasSubstring("bananas"));
        Assert.False(tree.HasSubstring("xyz"));
        Assert.False(tree.HasSubstring("nx"));
    }

    [Fact]
    public void FindOccurrences_Banana_ReturnsAllStartsForRepeatedSubstring()
    {
        var tree = new SuffixTreeStructure("banana");

        var occurrences = tree.FindOccurrences("ana");

        Assert.Equal(new HashSet<int> { 1, 3 }, occurrences.ToHashSet());
    }

    [Fact]
    public void FindOccurrences_AbsentSubstring_ReturnsEmpty()
    {
        var tree = new SuffixTreeStructure("banana");

        Assert.Empty(tree.FindOccurrences("xyz"));
    }

    [Fact]
    public void FindOccurrences_EmptyPattern_ReturnsEveryRealSuffixStart()
    {
        var tree = new SuffixTreeStructure("banana");

        var occurrences = tree.FindOccurrences("");

        Assert.Equal(Enumerable.Range(0, 6).ToHashSet(), occurrences.ToHashSet());
    }

    // "abcabd" (repeated substring "ab" at positions 0 and 3) specifically exercises the
    // mid-edge split branch - "banana" alone never triggers it.
    [Fact]
    public void HasSubstring_AbcAbd_FindsSubstringsAcrossASplitEdge()
    {
        var tree = new SuffixTreeStructure("abcabd");

        Assert.True(tree.HasSubstring("ab"));
        Assert.True(tree.HasSubstring("abc"));
        Assert.True(tree.HasSubstring("abd"));
        Assert.True(tree.HasSubstring("cab"));
        Assert.True(tree.HasSubstring("abcabd"));
    }

    [Fact]
    public void FindOccurrences_AbcAbd_ReturnsBothOccurrencesOfSplitSubstring()
    {
        var tree = new SuffixTreeStructure("abcabd");

        var occurrences = tree.FindOccurrences("ab");

        Assert.Equal(new HashSet<int> { 0, 3 }, occurrences.ToHashSet());
    }

    [Fact]
    public void HasSubstring_EmptyText_OnlyEmptyPatternMatches()
    {
        var tree = new SuffixTreeStructure("");

        Assert.True(tree.HasSubstring(""));
        Assert.False(tree.HasSubstring("a"));
    }

    [Fact]
    public void FindOccurrences_EmptyText_EmptyPatternReturnsEmpty()
    {
        var tree = new SuffixTreeStructure("");

        Assert.Empty(tree.FindOccurrences(""));
    }

    [Fact]
    public void Constructor_WithExplicitSuffixArray_ProducesTheSameResultAsTheConvenienceOverload()
    {
        const string text = "mississippi";
        var suffixArray = new DSAExperimentation.DataStructures.SuffixArray.SuffixArray(text);

        var fromExplicitArray = new SuffixTreeStructure(text, suffixArray);
        var fromConvenienceOverload = new SuffixTreeStructure(text);

        foreach (var pattern in new[] { "iss", "ssi", "ppi", "mis", "xyz" })
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
        string[] texts = ["banana", "abcabd", "aaaa", "mississippi", "aabaabaaab", "z"];

        foreach (var text in texts)
        {
            var tree = new SuffixTreeStructure(text);

            foreach (var pattern in CandidatePatterns(text))
            {
                Assert.Equal(text.Contains(pattern, StringComparison.Ordinal), tree.HasSubstring(pattern));
            }
        }
    }

    [Fact]
    public void FindOccurrences_MatchesBruteForceScanAcrossVariedInputsAndPatterns()
    {
        string[] texts = ["banana", "abcabd", "aaaa", "mississippi", "aabaabaaab"];

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
        yield return "";

        for (var start = 0; start < text.Length; start++)
        {
            for (var length = 1; start + length <= text.Length; length++)
            {
                yield return text.Substring(start, length);
            }
        }

        yield return text + "x";
        yield return "xyz";
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
