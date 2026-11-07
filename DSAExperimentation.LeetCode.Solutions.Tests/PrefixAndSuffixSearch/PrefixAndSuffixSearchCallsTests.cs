using DSAExperimentation.LeetCode.Solutions.PrefixAndSuffixSearch;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PrefixAndSuffixSearch;

// One word whose first and last letters differ, so a query reads differently with its halves swapped.
public sealed partial class PrefixAndSuffixSearchCallsTests
{
    private const string Head = "a";
    private const string Tail = "b";

    [Fact]
    public void F_PrefThenSuffOfTheWord_IsItsIndex()
    {
        var filter = Filter();

        Assert.Equal(0, PrefixAndSuffixSearchCalls.F(filter, Head, Tail));
    }

    [Fact]
    public void F_SuffThenPrefOfTheWord_IsMinusOne()
    {
        var filter = Filter();

        Assert.Equal(-1, PrefixAndSuffixSearchCalls.F(filter, Tail, Head));
    }

    private static IWordFilter Filter() => PrefixAndSuffixSearchSolution.CreateByLinearScan([Head + Tail]);
}
