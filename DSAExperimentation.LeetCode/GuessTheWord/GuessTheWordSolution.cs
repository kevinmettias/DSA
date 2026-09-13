using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.GuessTheWord;

// LeetCode 843. Guess the Word: find the hidden secret by calling master.Guess, which
// only ever reports how many positions of the guessed word match. Every word whose
// match count against that guess differs from the reported one is impossible, so each
// round shrinks the candidate pool; the secret is found when a guess scores the full
// word length.
//
// Both strategies pick the same guess every round - candidate 0, in the original
// wordList order - so they converge on the identical secret in the identical number of
// rounds. The only thing they differ in is how the pool is narrowed.
internal static class GuessTheWordSolution
{
    // The textbook answer: keep the pool in a BCL List<string> and delete the
    // eliminated words from it in place, walking back to front so the indices stay
    // valid. Removing near the front shifts every trailing element, so a round that
    // eliminates most of the pool costs O(poolSize^2) rather than O(poolSize).
    public static string FindSecretWordByInPlaceListRemoval(string[] wordList, SecretWordMaster master)
    {
        var candidates = new List<string>(wordList);

        while (candidates.Count > 0)
        {
            var guess = candidates[0];
            var matches = master.Guess(guess);

            if (matches == guess.Length)
            {
                return guess;
            }

            for (var i = candidates.Count - 1; i >= 0; i--)
            {
                if (WordMatch.ExactPositionMatches(candidates[i], guess) != matches)
                {
                    candidates.RemoveAt(i);
                }
            }
        }

        return string.Empty;
    }

    // This repo's own DynamicArray<string> holding the shrinking pool, rebuilt round
    // by round via a single Add pass instead of being edited in place - one O(poolSize)
    // sweep with no shifting, however many candidates the round eliminates.
    public static string FindSecretWordByShrinkingPool(string[] wordList, SecretWordMaster master)
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

            candidates = NarrowCandidates(candidates, guess, matches);
        }

        return string.Empty;
    }

    // Keep exactly the words that would have produced the same match count against
    // this guess - the secret is always one of them.
    private static DynamicArray<string> NarrowCandidates(DynamicArray<string> candidates, string guess, int matches)
    {
        var next = new DynamicArray<string>();

        for (var i = 0; i < candidates.Count; i++)
        {
            var candidate = candidates.Get(i);

            if (WordMatch.ExactPositionMatches(candidate, guess) == matches)
            {
                next.Add(candidate);
            }
        }

        return next;
    }
}
