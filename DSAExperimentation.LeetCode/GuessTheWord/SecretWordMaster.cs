namespace DSAExperimentation.LeetCode.GuessTheWord;

// LeetCode hands LC 843 an interactive `Master` object rather than the answer: the
// solver may only call guess(word), which reports how many positions of that word
// match the hidden secret, and must find the secret within 10 calls. This is that
// judge, modelled the way GuessNumberHigherOrLowerSolution models guess(num) - a
// fixed secret plus a call counter - so the harnesses can assert both the word found
// AND that the guess budget was respected.
//
// A witness for this problem alone, so it lives beside the solution (ARCHITECTURE.md
// section 17.3) rather than in Domain/.
internal sealed class SecretWordMaster(string secret)
{
    // How many times Guess has been called - LeetCode's 10-call budget is the whole
    // difficulty of the problem, so it has to be observable.
    public int GuessCount { get; private set; }

    public int Guess(string word)
    {
        GuessCount++;

        return WordMatch.ExactPositionMatches(word, secret);
    }
}
