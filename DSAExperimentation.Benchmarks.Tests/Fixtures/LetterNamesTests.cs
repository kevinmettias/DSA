using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for LetterNames (ARCHITECTURE 17.7). The reading depends on two things the
// generator promises: every name is lowercase letters only, the alphabet LeetCode's name and word
// constraints allow, and no two indices share a name, since the problems that use it require
// unique names. The lengths are pinned too, because a caller sizes its names against a length cap.
public sealed partial class LetterNamesTests
{
    // More indices than any caller names, so distinctness is checked past the three-letter boundary.
    private const int CheckedIndexCount = 20_000;
    private const string LowercaseLettersOnly = "^[a-z]+$";

    // The first index of each name length: 26 one-letter names, then 26^2 two-letter names.
    private const int FirstTwoLetterIndex = 26;
    private const int FirstThreeLetterIndex = 702;

    private const string AlphabetInOrder = "abcdefghijklmnopqrstuvwxyz";
    private const string FirstNameOfTwoLetters = "aa";
    private const string LastNameOfTwoLetters = "zz";
    private const string FirstNameOfThreeLetters = "aaa";

    [Fact]
    public void Of_FirstTwentySixIndices_NamesTheAlphabetInOrder() =>
        Assert.Equal(
            AlphabetInOrder,
            string.Concat(Enumerable.Range(0, FirstTwoLetterIndex).Select(LetterNames.Of)));

    [Fact]
    public void Of_LengthBoundaries_StartAndEndEachLength()
    {
        Assert.Equal(FirstNameOfTwoLetters, LetterNames.Of(FirstTwoLetterIndex));
        Assert.Equal(LastNameOfTwoLetters, LetterNames.Of(FirstThreeLetterIndex - 1));
        Assert.Equal(FirstNameOfThreeLetters, LetterNames.Of(FirstThreeLetterIndex));
    }

    [Fact]
    public void Of_EveryIndex_IsLowercaseLettersOnly() =>
        Assert.All(
            Enumerable.Range(0, CheckedIndexCount).Select(LetterNames.Of),
            name => Assert.Matches(LowercaseLettersOnly, name));

    [Fact]
    public void Of_DistinctIndices_NameDistinctly() =>
        Assert.Equal(
            CheckedIndexCount,
            Enumerable.Range(0, CheckedIndexCount).Select(LetterNames.Of).Distinct().Count());
}
