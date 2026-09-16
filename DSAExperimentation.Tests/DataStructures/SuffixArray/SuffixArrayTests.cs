using SuffixArrayStructure = DSAExperimentation.DataStructures.SuffixArray.SuffixArray;

namespace DSAExperimentation.Tests.DataStructures.SuffixArray;

// Harness only. The two published expectations a suffix array exposes - the order of
// the suffixes and the longest common prefix of each neighbouring pair in that order -
// are precomputed together in one constructor, so a case carries both and every test
// reaches the structure through Build rather than constructing its own. The brute-force
// oracles below recompute each expectation the slow way for the texts no published
// reference covers.
public sealed partial class SuffixArrayTests
{
    public static TheoryData<TextExample> Examples =>
        new()
        {
            { new TextExample(Fixtures.RepeatedAa, [1, 0], [1]) },
            { new TextExample(Fixtures.Banana, Fixtures.BananaSuffixOrder, Fixtures.BananaLcpValues) },
            { new TextExample(Fixtures.Dcba, Fixtures.DcbaSuffixOrder, [0, 0, 0]) },
            { new TextExample(Fixtures.EmptyText, [], []) },
            { new TextExample(Fixtures.SingleCharacterText, [0], []) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void Suffixes_OrderEverySuffixAscending(TextExample example)
    {
        var suffixArray = Build(example.Text);

        Assert.Equal(example.SuffixOrder, suffixArray.Suffixes);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void LcpArray_ReportsTheLongestCommonPrefixOfEachNeighbouringPair(TextExample example)
    {
        var suffixArray = Build(example.Text);

        Assert.Equal(example.LcpValues, suffixArray.LongestCommonPrefixArray);
    }

    [Fact]
    public void Rank_IsInverseOfSuffixes()
    {
        var suffixArray = Build(Fixtures.Banana);

        for (var i = 0; i < suffixArray.Suffixes.Length; i++)
        {
            Assert.Equal(i, suffixArray.Rank[suffixArray.Suffixes[i]]);
        }
    }

    [Fact]
    public void Suffixes_MatchesBruteForceSortForVariedInputs()
    {
        foreach (var text in Fixtures.VariedTexts)
        {
            var suffixArray = Build(text);
            var expected = BruteForceSuffixOrder(text);

            Assert.Equal(expected, suffixArray.Suffixes);
        }
    }

    [Fact]
    public void LcpArray_MatchesBruteForceLongestCommonPrefixForVariedInputs()
    {
        foreach (var text in Fixtures.VariedTexts)
        {
            var suffixArray = Build(text);

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

        var suffixArray = Build(Fixtures.MixedCaseText, caseInsensitive);
        var expected = BruteForceSuffixOrder(Fixtures.MixedCaseText, caseInsensitive);

        Assert.Equal(expected, suffixArray.Suffixes);
    }

    // The one place a text becomes a suffix array, so a case chooses a text and a
    // comparer and gets the same O(1)-indexed structure every test asserts against.
    private static SuffixArrayStructure Build(string text) => Build(text, Comparer<char>.Default);

    private static SuffixArrayStructure Build(string text, IComparer<char> comparer) => new(text, comparer);

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

        while (PrefixesAgreeOneCharacterFurther(first, second, length))
        {
            length++;
        }

        return length;
    }

    // Whether the two spans still agree at `length` - the longest common prefix is the
    // largest length for which this holds.
    private static bool PrefixesAgreeOneCharacterFurther(ReadOnlySpan<char> first, ReadOnlySpan<char> second, int length)
        => length < first.Length
            && length < second.Length
            && first[length] == second[length];

    /// <summary>
    /// The texts these tests build suffix arrays from, and the orders and LCP values they
    /// expect back, named once so a second test does not have to reach into a neighbour's
    /// body for them.
    /// </summary>
    private static class Fixtures
    {
        public const string RepeatedAa = "aa";
        public const string Banana = "banana";
        public const string Aaaa = "aaaa";
        public const string Mississippi = "mississippi";
        public const string Abcabd = "abcabd";
        public const string Aabaabaaab = "aabaabaaab";
        public const string Dcba = "dcba";
        public const string EmptyText = "";
        public const string SingleCharacterText = "a";
        public const string MixedCaseText = "BaAb";

        public static readonly int[] BananaSuffixOrder = [5, 3, 1, 0, 4, 2];
        public static readonly int[] BananaLcpValues = [1, 3, 0, 0, 2];
        public static readonly int[] DcbaSuffixOrder = [3, 2, 1, 0];
        public static readonly string[] VariedTexts = [Banana, RepeatedAa, Aaaa, Mississippi, Abcabd, Aabaabaaab];
    }

    public readonly record struct TextExample(string Text, int[] SuffixOrder, int[] LcpValues);
}
