using DSAExperimentation.LeetCode.Solutions.GuessTheWord;

namespace DSAExperimentation.LeetCode.Solutions.Tests.GuessTheWord;

// Harness only: both candidate-pool strategies live in GuessTheWordSolution and the
// oracle is its SecretWordMaster. Each example asserts both halves of LC 843 - the
// secret was actually found, and it was found inside LeetCode's 10-call budget - so
// a strategy that brute-forced the pool would fail even while returning the right
// word. Each strategy's LeetCode-shaped overload is held to the judge's verdict both
// ways: within the budget, and one call short of what the strategy needs.
public sealed partial class GuessTheWordSolutionTests
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

    // LeetCode's two examples, and a secret found on the second call with exactly two
    // allowed: "acckzz" scores 4 against "abcczz", and only "abcczz" itself scores 4
    // against "acckzz".
    public static TheoryData<string, string[], int> GuessedWithinBudget =>
        new()
        {
            { "acckzz", ["acckzz", "ccbazz", "eiowzz", "abcczz"], GuessBudget },
            { "hamada", ["hamada", "khaled"], GuessBudget },
            { "abcczz", ["acckzz", "ccbazz", "eiowzz", "abcczz"], 2 },
        };

    // Each secret is found on the second call, one more than the case allows.
    public static TheoryData<string, string[], int> GuessedPastBudget =>
        new()
        {
            { "abcczz", ["acckzz", "ccbazz", "eiowzz", "abcczz"], 1 },
            { "khaled", ["hamada", "khaled"], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSecretWordByInPlaceListRemoval_LeetCodeExamples_FindsSecretWithinGuessBudget(
        string[] wordList, string secret)
    {
        var master = MasterOver(secret, wordList);

        var found = GuessTheWordSolution.FindSecretWordByInPlaceListRemoval(wordList, master);

        Assert.Equal(secret, found);
        Assert.True(master.GuessCount <= GuessBudget);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSecretWordByShrinkingPool_LeetCodeExamples_FindsSecretWithinGuessBudget(
        string[] wordList, string secret)
    {
        var master = MasterOver(secret, wordList);

        var found = GuessTheWordSolution.FindSecretWordByShrinkingPool(wordList, master);

        Assert.Equal(secret, found);
        Assert.True(master.GuessCount <= GuessBudget);
    }

    [Theory]
    [MemberData(nameof(GuessedWithinBudget))]
    public void FindSecretWordByInPlaceListRemoval_LeetCodeShapeWithinBudget_ReportsSecretGuessed(
        string secret, string[] words, int allowedGuesses)
        => Assert.Equal(SecretWordMaster.GuessedVerdict, GuessTheWordSolution.FindSecretWordByInPlaceListRemoval(secret, words, allowedGuesses));

    [Theory]
    [MemberData(nameof(GuessedPastBudget))]
    public void FindSecretWordByInPlaceListRemoval_LeetCodeShapePastBudget_ReportsTooManyGuesses(
        string secret, string[] words, int allowedGuesses)
        => Assert.Equal(SecretWordMaster.NotGuessedVerdict, GuessTheWordSolution.FindSecretWordByInPlaceListRemoval(secret, words, allowedGuesses));

    [Theory]
    [MemberData(nameof(GuessedWithinBudget))]
    public void FindSecretWordByShrinkingPool_LeetCodeShapeWithinBudget_ReportsSecretGuessed(
        string secret, string[] words, int allowedGuesses)
        => Assert.Equal(SecretWordMaster.GuessedVerdict, GuessTheWordSolution.FindSecretWordByShrinkingPool(secret, words, allowedGuesses));

    [Theory]
    [MemberData(nameof(GuessedPastBudget))]
    public void FindSecretWordByShrinkingPool_LeetCodeShapePastBudget_ReportsTooManyGuesses(
        string secret, string[] words, int allowedGuesses)
        => Assert.Equal(SecretWordMaster.NotGuessedVerdict, GuessTheWordSolution.FindSecretWordByShrinkingPool(secret, words, allowedGuesses));

    private static SecretWordMaster MasterOver(string secret, string[] wordList)
    {
        var words = new HashSet<string>(wordList);

        return new SecretWordMaster(secret, words);
    }
}
