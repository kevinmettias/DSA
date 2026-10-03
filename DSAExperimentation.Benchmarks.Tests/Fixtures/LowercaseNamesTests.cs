using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for LowercaseNames (ARCHITECTURE 17.7): the workloads that use it rely on two
// things - that distinct indices never share a name, since the problems promise distinct foods,
// tables or tokens, and that a name holds lowercase letters only, since the problems promise that
// too. The bijective base-26 order is what keeps each name as short as its index allows.
public sealed partial class LowercaseNamesTests
{
    // More names than any workload asks for: LC 2353's 2 * 10^4 foods.
    private const int Count = 20_000;

    private const int LastOneLetterIndex = 25;
    private const int FirstTwoLetterIndex = 26;
    private const int LastTwoLetterIndex = 701;
    private const int FirstThreeLetterIndex = 702;

    // LC 1656's value length.
    private const int Width = 5;

    [Fact]
    public void Of_EveryIndexBelowCount_GetsItsOwnName() =>
        Assert.Equal(Count, Enumerable.Range(0, Count).Select(LowercaseNames.Of).Distinct().Count());

    [Fact]
    public void Of_EveryIndexBelowCount_SpellsLowercaseLettersOnly() =>
        Assert.All(
            Enumerable.Range(0, Count).Select(LowercaseNames.Of),
            name => Assert.All(name, letter => Assert.InRange(letter, 'a', 'z')));

    [Theory]
    [InlineData(0, "a")]
    [InlineData(LastOneLetterIndex, "z")]
    [InlineData(FirstTwoLetterIndex, "aa")]
    [InlineData(LastTwoLetterIndex, "zz")]
    [InlineData(FirstThreeLetterIndex, "aaa")]
    public void Of_IndexAtALengthBoundary_IsTheShortestNameLeft(int index, string expected) =>
        Assert.Equal(expected, LowercaseNames.Of(index));

    [Fact]
    public void OfWidth_EveryIndexBelowCount_GetsItsOwnNameOfThatWidth()
    {
        var names = Enumerable.Range(0, Count).Select(index => LowercaseNames.OfWidth(index, Width)).ToList();

        Assert.Equal(Count, names.Distinct().Count());
        Assert.All(names, name => Assert.Equal(Width, name.Length));
        Assert.All(names, name => Assert.All(name, letter => Assert.InRange(letter, 'a', 'z')));
    }

    [Theory]
    [InlineData(0, "aaaaa")]
    [InlineData(LastOneLetterIndex, "aaaaz")]
    [InlineData(FirstTwoLetterIndex, "aaaba")]
    public void OfWidth_Index_IsThatIndexInBaseTwentySix(int index, string expected) =>
        Assert.Equal(expected, LowercaseNames.OfWidth(index, Width));
}
