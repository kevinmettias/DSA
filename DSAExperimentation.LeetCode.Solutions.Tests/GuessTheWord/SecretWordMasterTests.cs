using DSAExperimentation.LeetCode.Solutions.GuessTheWord;

namespace DSAExperimentation.LeetCode.Solutions.Tests.GuessTheWord;

// SecretWordMaster is LeetCode's Master for LC 843, so each test holds it to the
// statement's own words over example 1's list: guess answers -1 for "aaaaaa", 6 for
// the secret "acckzz", 3 for "ccbazz", 2 for "eiowzz" and 4 for "abcczz", every call
// counts, and the verdict needs the secret guessed within allowedGuesses calls.
public sealed partial class SecretWordMasterTests
{
    private const string Secret = "acckzz";

    private static readonly string[] ExampleWords = ["acckzz", "ccbazz", "eiowzz", "abcczz"];

    public static TheoryData<string, int> StatedGuesses =>
        new()
        {
            { "aaaaaa", SecretWordMaster.NotInWords },
            { "acckzz", 6 },
            { "ccbazz", 3 },
            { "eiowzz", 2 },
            { "abcczz", 4 },
        };

    // The calls made, then how many the case allows.
    public static TheoryData<string[], int> SecretGuessedWithinAllowance =>
        new()
        {
            { [Secret], 1 },
            { ["ccbazz", Secret], 2 },
            { ["aaaaaa", "ccbazz", Secret], 10 },
        };

    // More calls than allowed with the secret among them, a word outside the list
    // counting as one; and calls that never guess the secret.
    public static TheoryData<string[], int> SecretNotGuessedWithinAllowance =>
        new()
        {
            { [Secret, "ccbazz"], 1 },
            { ["aaaaaa", Secret], 1 },
            { ["ccbazz", "eiowzz"], 10 },
        };

    [Theory]
    [MemberData(nameof(StatedGuesses))]
    public void Guess_ExampleOneWord_AnswersWhatTheStatementSays(string word, int expected)
        => Assert.Equal(expected, ExampleMaster().Guess(word));

    [Fact]
    public void Guess_WordNotInList_CountsTheCall()
    {
        var master = ExampleMaster();

        _ = master.Guess("aaaaaa");

        Assert.Equal(1, master.GuessCount);
    }

    [Fact]
    public void HasGuessedSecret_OnlyOtherWordsGuessed_IsFalse()
    {
        var master = ExampleMaster();

        _ = master.Guess("ccbazz");
        _ = master.Guess("aaaaaa");

        Assert.False(master.HasGuessedSecret);
    }

    [Fact]
    public void HasGuessedSecret_SecretGuessed_IsTrue()
    {
        var master = ExampleMaster();

        _ = master.Guess(Secret);

        Assert.True(master.HasGuessedSecret);
    }

    [Theory]
    [MemberData(nameof(SecretGuessedWithinAllowance))]
    public void IsSecretGuessedWithin_SecretGuessedInAllowedCalls_IsTrue(string[] guesses, int allowedGuesses)
    {
        var master = MasterAfter(guesses);

        Assert.True(master.IsSecretGuessedWithin(allowedGuesses));
    }

    [Theory]
    [MemberData(nameof(SecretNotGuessedWithinAllowance))]
    public void IsSecretGuessedWithin_TooManyCallsOrSecretMissed_IsFalse(string[] guesses, int allowedGuesses)
    {
        var master = MasterAfter(guesses);

        Assert.False(master.IsSecretGuessedWithin(allowedGuesses));
    }

    private static SecretWordMaster ExampleMaster()
    {
        var words = new HashSet<string>(ExampleWords);

        return new SecretWordMaster(Secret, words);
    }

    private static SecretWordMaster MasterAfter(string[] guesses)
    {
        var master = ExampleMaster();

        foreach (var guess in guesses)
        {
            _ = master.Guess(guess);
        }

        return master;
    }
}
