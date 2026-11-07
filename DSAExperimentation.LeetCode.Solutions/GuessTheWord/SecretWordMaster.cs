namespace DSAExperimentation.LeetCode.Solutions.GuessTheWord;

// LeetCode hands LC 843 an interactive `Master` object rather than the answer: the
// solver may only call guess(word), which reports how many positions of that word
// match the hidden secret, or -1 for a word the list does not hold, and the judge
// counts every call against the case's allowedGuesses. This is that judge, modelled the
// way GuessNumberHigherOrLowerSolution models guess(num) - a fixed secret plus a call
// counter - so the harnesses can assert both the word found AND that the guess budget
// was respected, and can print LeetCode's verdict (IsSecretGuessedWithin).
//
// It takes the list as a set because it answers membership on every call, and a
// benchmark builds that set once, outside its timed loop, where it builds a fresh
// Master per invocation for the per-run counter.
//
// A witness for this problem alone, so it lives beside the solution (ARCHITECTURE.md
// section 17.3) rather than at the tier's root, which holds only what LeetCode fixes
// for every problem (section 17.11).
internal sealed class SecretWordMaster(string secret, IReadOnlySet<string> words)
{
    // What guess(word) answers for a word that is not in the list.
    public const int NotInWords = -1;

    // The two sentences LeetCode's judge prints once findSecretWord returns.
    public const string GuessedVerdict = "You guessed the secret word correctly.";
    public const string NotGuessedVerdict = "Either you took too many guesses, or you did not find the secret word.";

    // How many times Guess has been called, a word outside the list included -
    // LeetCode's allowedGuesses budget is the whole difficulty of the problem, so it
    // has to be observable.
    public int GuessCount { get; private set; }

    // Whether some call so far guessed the secret itself, the other half of LeetCode's
    // verdict.
    public bool HasGuessedSecret { get; private set; }

    public int Guess(string word)
    {
        GuessCount++;

        if (!words.Contains(word))
        {
            return NotInWords;
        }

        HasGuessedSecret |= word == secret;

        return WordMatch.ExactPositionMatches(word, secret);
    }

    // Whether LeetCode's judge would call the secret found: some call guessed it, and there were at most
    // allowedGuesses calls in all.
    public bool IsSecretGuessedWithin(int allowedGuesses) => HasGuessedSecret && GuessCount <= allowedGuesses;

    // The sentence LeetCode's judge prints for the calls made so far.
    public string VerdictWithin(int allowedGuesses) => IsSecretGuessedWithin(allowedGuesses) ? GuessedVerdict : NotGuessedVerdict;
}
