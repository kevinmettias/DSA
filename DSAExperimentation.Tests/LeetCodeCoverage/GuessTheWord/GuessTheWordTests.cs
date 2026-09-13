using DSAExperimentation.LeetCode.GuessTheWord;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GuessTheWord;

// Harness only: both candidate-pool strategies live in GuessTheWordSolution and the
// oracle is its SecretWordMaster. Each example asserts both halves of LC 843 - the
// secret was actually found, and it was found inside LeetCode's 10-call budget - so
// a strategy that brute-forced the pool would fail even while returning the right
// word.
public sealed class GuessTheWordTests
{
    // LeetCode allows at most 10 calls to Master.Guess.
    private const int GuessBudget = 10;

    public static TheoryData<string[], string> Examples =>
        new()
        {
            { ["acckzz", "ccbazz", "eiowzz", "abcczz"], "acckzz" },
            { ["acckzz", "ccbazz", "eiowzz", "abcczz"], "ccbazz" },
            { ["hamada", "khaled"], "hamada" },
            { ["abcdef", "abcdeg", "abcdeh", "zzzzzz"], "abcdeh" },
            { ["aaaaaa"], "aaaaaa" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSecretWordByInPlaceListRemoval_LeetCodeExamples_FindsSecretWithinGuessBudget(
        string[] wordList, string secret)
    {
        var master = new SecretWordMaster(secret);

        var found = GuessTheWordSolution.FindSecretWordByInPlaceListRemoval(wordList, master);

        Assert.Equal(secret, found);
        Assert.True(master.GuessCount <= GuessBudget);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSecretWordByShrinkingPool_LeetCodeExamples_FindsSecretWithinGuessBudget(
        string[] wordList, string secret)
    {
        var master = new SecretWordMaster(secret);

        var found = GuessTheWordSolution.FindSecretWordByShrinkingPool(wordList, master);

        Assert.Equal(secret, found);
        Assert.True(master.GuessCount <= GuessBudget);
    }
}
