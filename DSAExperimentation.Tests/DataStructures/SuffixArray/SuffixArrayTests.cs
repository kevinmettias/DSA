using SuffixArrayStructure = DSAExperimentation.DataStructures.SuffixArray.SuffixArray;

namespace DSAExperimentation.Tests.DataStructures.SuffixArray;

public sealed partial class SuffixArrayTests
{
    private const string RepeatedAa = "aa";
    private const string Banana = "banana";
    private const string Aaaa = "aaaa";
    private const string Mississippi = "mississippi";
    private const string Abcabd = "abcabd";
    private const string Aabaabaaab = "aabaabaaab";
    private const string Dcba = "dcba";
    private const string EmptyText = "";
    private const string SingleCharacterText = "a";
    private const string MixedCaseText = "BaAb";

    private static readonly int[] BananaSuffixOrder = [5, 3, 1, 0, 4, 2];
    private static readonly int[] BananaLcpValues = [1, 3, 0, 0, 2];
    private static readonly int[] DcbaSuffixOrder = [3, 2, 1, 0];
    private static readonly string[] VariedTexts = [Banana, RepeatedAa, Aaaa, Mississippi, Abcabd, Aabaabaaab];

    [Fact]
    public void Suffixes_RepeatedCharacterText_OrdersShorterSuffixFirst()
    {
        var suffixArray = new SuffixArrayStructure(RepeatedAa);

        Assert.Equal([1, 0], suffixArray.Suffixes);
    }

    [Fact]
    public void LcpArray_RepeatedCharacterText_ReportsSharedPrefixLength()
    {
        var suffixArray = new SuffixArrayStructure(RepeatedAa);

        Assert.Equal([1], suffixArray.LongestCommonPrefixArray);
    }

    [Fact]
    public void Suffixes_Banana_MatchesPublishedReferenceOrder()
    {
        var suffixArray = new SuffixArrayStructure(Banana);

        Assert.Equal(BananaSuffixOrder, suffixArray.Suffixes);
    }

    [Fact]
    public void LcpArray_Banana_MatchesPublishedReferenceValues()
    {
        var suffixArray = new SuffixArrayStructure(Banana);

        Assert.Equal(BananaLcpValues, suffixArray.LongestCommonPrefixArray);
    }

    [Fact]
    public void Rank_IsInverseOfSuffixes()
    {
        var suffixArray = new SuffixArrayStructure(Banana);

        for (var i = 0; i < suffixArray.Suffixes.Length; i++)
        {
            Assert.Equal(i, suffixArray.Rank[suffixArray.Suffixes[i]]);
        }
    }

    [Fact]
    public void Suffixes_EmptyText_ReturnsEmptyArray()
    {
        var suffixArray = new SuffixArrayStructure(EmptyText);

        Assert.Empty(suffixArray.Suffixes);
    }

    [Fact]
    public void LcpArray_EmptyText_ReturnsEmptyArray()
    {
        var suffixArray = new SuffixArrayStructure(EmptyText);

        Assert.Empty(suffixArray.LongestCommonPrefixArray);
    }

    [Fact]
    public void LcpArray_SingleCharacterText_ReturnsEmptyArray()
    {
        var suffixArray = new SuffixArrayStructure(SingleCharacterText);

        Assert.Empty(suffixArray.LongestCommonPrefixArray);
    }

    [Fact]
    public void Suffixes_AllDistinctCharacters_ReturnsOrdinaryLexicographicOrder()
    {
        var suffixArray = new SuffixArrayStructure(Dcba);

        Assert.Equal(DcbaSuffixOrder, suffixArray.Suffixes);
    }

    [Fact]
    public void Suffixes_MatchesBruteForceSortForVariedInputs()
    {
        foreach (var text in VariedTexts)
        {
            var suffixArray = new SuffixArrayStructure(text);
            var expected = BruteForceSuffixOrder(text);

            Assert.Equal(expected, suffixArray.Suffixes);
        }
    }

    [Fact]
    public void LcpArray_MatchesBruteForceLongestCommonPrefixForVariedInputs()
    {
        foreach (var text in VariedTexts)
        {
            var suffixArray = new SuffixArrayStructure(text);

            for (var i = 0; i < suffixArray.LongestCommonPrefixArray.Length; i++)
            {
                var expected = BruteForceLongestCommonPrefix(
                    text.AsSpan(suffixArray.Suffixes[i]), text.AsSpan(suffixArray.Suffixes[i + 1]));

                Assert.Equal(expected, suffixArray.LongestCommonPrefixArray[i]);
            }
        }
    }

    [Fact]
    public void Suffixes_WithCaseInsensitiveComparer_OrdersCaseVariantsTogether()
    {
        var caseInsensitive = Comparer<char>.Create(
            (left, right) => char.ToUpperInvariant(left).CompareTo(char.ToUpperInvariant(right)));

        var suffixArray = new SuffixArrayStructure(MixedCaseText, caseInsensitive);
        var expected = BruteForceSuffixOrder(MixedCaseText, caseInsensitive);

        Assert.Equal(expected, suffixArray.Suffixes);
    }

    private static int[] BruteForceSuffixOrder(string text) => BruteForceSuffixOrder(text, Comparer<char>.Default);

    private static int[] BruteForceSuffixOrder(string text, IComparer<char> comparer)
    {
        var indices = new int[text.Length];

        for (var i = 0; i < text.Length; i++)
        {
            indices[i] = i;
        }

        Array.Sort(indices, (a, b) => CompareSuffixes(text, a, b, comparer));
        return indices;
    }

    private static int CompareSuffixes(string text, int a, int b, IComparer<char> comparer)
    {
        var length = Math.Min(text.Length - a, text.Length - b);

        for (var offset = 0; offset < length; offset++)
        {
            var comparison = comparer.Compare(text[a + offset], text[b + offset]);

            if (comparison != 0)
            {
                return comparison;
            }
        }

        return (text.Length - a).CompareTo(text.Length - b);
    }

    private static int BruteForceLongestCommonPrefix(ReadOnlySpan<char> first, ReadOnlySpan<char> second)
    {
        var length = 0;

        while (length < first.Length && length < second.Length && first[length] == second[length])
        {
            length++;
        }

        return length;
    }
}
