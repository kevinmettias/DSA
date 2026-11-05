using DSAExperimentation.LeetCode.Solutions.WordsWithinTwoEditsOfDictionary;

namespace DSAExperimentation.LeetCode.Solutions.Tests.WordsWithinTwoEditsOfDictionary;

public sealed partial class WordsWithinTwoEditsOfDictionaryPromiseTests
{
    public static TheoryData<string[], string[], bool> Inputs =>
        new()
        {
            { ["word", "note", "ants", "wood"], ["wood", "joke", "moat"], true },
            { ["yes"], ["not"], true },
            { ["elefant", "giraff", "hippo"], ["elephant", "giraffe", "hippo"], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryWordHasOneLength(string[] queries, string[] dictionary, bool expected)
        => Assert.Equal(expected, WordsWithinTwoEditsOfDictionaryPromise.IsKeptBy(queries, dictionary));
}
