using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GuessTheWord;

// LeetCode 843. Guess the Word: this repo's own DynamicArray<string> holds the
// shrinking pool of candidates still consistent with every match-count response so
// far - rebuilt round by round via a single filter pass, until the observed match
// count equals the word length. The Master oracle here is the interactive-guessing
// counterpart to GuessNumberHigherOrLowerTests' Guess()-comparer fixture, wrapping a
// fixed secret and a call counter instead of a numeric target.
public sealed partial class GuessTheWordTests
{
    [Fact]
    public void FindSecretWord_LeetCodeExampleOne_FindsSecretWithinTenGuesses()
    {
        string[] wordList = ["acckzz", "ccbazz", "eiowzz", "abcczz"];
        var master = new Master("acckzz");

        var found = FindSecretWord(wordList, master);

        Assert.Equal("acckzz", found);
        Assert.True(master.GuessCount <= 10);
    }

    [Fact]
    public void FindSecretWord_LeetCodeExampleTwo_FindsSecretWithinTenGuesses()
    {
        string[] wordList = ["hamada", "khaled"];
        var master = new Master("hamada");

        var found = FindSecretWord(wordList, master);

        Assert.Equal("hamada", found);
        Assert.True(master.GuessCount <= 10);
    }

    private static string FindSecretWord(string[] wordList, Master master)
    {
        var candidates = new DynamicArray<string>();

        foreach (var word in wordList)
        {
            candidates.Add(word);
        }

        while (candidates.Count > 0)
        {
            var guess = candidates.Get(0);
            var matches = master.Guess(guess);

            if (matches == guess.Length)
            {
                return guess;
            }

            var next = new DynamicArray<string>();

            for (var i = 0; i < candidates.Count; i++)
            {
                var candidate = candidates.Get(i);

                if (MatchCount(candidate, guess) == matches)
                {
                    next.Add(candidate);
                }
            }

            candidates = next;
        }

        return string.Empty;
    }

    private static int MatchCount(string first, string second)
    {
        var count = 0;

        for (var i = 0; i < first.Length; i++)
        {
            if (first[i] == second[i])
            {
                count++;
            }
        }

        return count;
    }

    private sealed class Master(string secret)
    {
        public int GuessCount { get; private set; }

        public int Guess(string word)
        {
            GuessCount++;
            return MatchCount(word, secret);
        }
    }
}
