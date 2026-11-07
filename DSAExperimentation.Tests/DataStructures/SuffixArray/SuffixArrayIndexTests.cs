using DSAExperimentation.DataStructures.SuffixArray;
using SuffixArrayStructure = DSAExperimentation.DataStructures.SuffixArray.SuffixArray;

namespace DSAExperimentation.Tests.DataStructures.SuffixArray;

// Worked examples on "banana", whose suffixes sort as a, ana, anana, banana, na, nana. Every text
// and pattern against a scan is SubstringIndexTests' job; these name each member and pin its
// answers by hand.
public sealed partial class SuffixArrayIndexTests
{
    private const string Banana = "banana";
    private const string Ana = "ana";
    private const string Nab = "nab";
    private const string Bananas = "bananas";
    private const int AnaOccurrences = 2;

    [Fact]
    public void HasSubstring_Banana_FindsAnaAndNotNab() =>
        Assert.Equal((true, false), (Build().HasSubstring(Ana), Build().HasSubstring(Nab)));

    [Fact]
    public void HasSubstring_PatternRunningPastTheText_IsAbsent() =>
        Assert.False(Build().HasSubstring(Bananas));

    [Fact]
    public void CountOccurrences_Banana_CountsAnaTwiceThoughTheyOverlap() =>
        Assert.Equal(AnaOccurrences, Build().CountOccurrences(Ana));

    [Fact]
    public void CountOccurrences_EmptyPattern_CountsEveryInsertionPoint() =>
        Assert.Equal(Banana.Length + 1, Build().CountOccurrences(string.Empty));

    // "ana" sorts before "anana", so its start, 3, comes first.
    [Fact]
    public void FindOccurrences_Banana_ListsAnaInTheSuffixesSortedOrder() =>
        Assert.Equal([3, 1], Build().FindOccurrences(Ana));

    [Fact]
    public void FindOccurrences_AbsentPattern_IsEmpty() =>
        Assert.Empty(Build().FindOccurrences(Nab));

    [Fact]
    public void SuffixArray_GivenArray_IsTheOneSearched()
    {
        var suffixArray = new SuffixArrayStructure(Banana);

        Assert.Same(suffixArray, new SuffixArrayIndex(Banana, suffixArray).SuffixArray);
    }

    private static SuffixArrayIndex Build() => new(Banana);
}
