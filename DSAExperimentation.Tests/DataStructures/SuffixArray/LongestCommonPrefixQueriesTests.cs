using DSAExperimentation.DataStructures.SuffixArray;
using DSAExperimentation.Tests.DataStructures.SubstringIndex.Fixtures;
using SuffixArrayStructure = DSAExperimentation.DataStructures.SuffixArray.SuffixArray;

namespace DSAExperimentation.Tests.DataStructures.SuffixArray;

// Every pair of suffixes of every substring-index fixture text, in both orders, against a direct
// comparison of the two suffixes.
public sealed partial class LongestCommonPrefixQueriesTests
{
    private const string Banana = "banana";
    private const int AnaAtOne = 1;
    private const int AnaAtThree = 3;
    private const int AnaLength = 3;

    public static TheoryData<string> TextNames => new(SubstringIndexTexts.Names);

    public static TheoryData<int> PositionsOutsideBanana => new() { -1, Banana.Length };

    [Fact]
    public void LongestCommonPrefix_Banana_SharesAnaBetweenItsTwoOccurrences() =>
        Assert.Equal(AnaLength, Build(Banana).LongestCommonPrefix(AnaAtThree, AnaAtOne));

    [Theory]
    [MemberData(nameof(TextNames))]
    public void LongestCommonPrefix_EveryPairOfSuffixes_IsWhatTheyShare(string textName)
    {
        var text = SubstringIndexTexts.Named(textName);
        var queries = Build(text);

        var wrong = Enumerable.Range(0, text.Length)
            .SelectMany(first => Enumerable.Range(0, text.Length).Select(second => (first, second)))
            .Where(pair => queries.LongestCommonPrefix(pair.first, pair.second)
                != BruteForceSubstrings.LongestCommonPrefix(text, pair.first, pair.second))
            .ToList();

        Assert.Empty(wrong);
    }

    [Theory]
    [MemberData(nameof(PositionsOutsideBanana))]
    public void LongestCommonPrefix_PositionOutsideTheText_Throws(int position) =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Build(Banana).LongestCommonPrefix(position, 0));

    private static LongestCommonPrefixQueries Build(string text) =>
        new(new SuffixArrayStructure(text));
}
