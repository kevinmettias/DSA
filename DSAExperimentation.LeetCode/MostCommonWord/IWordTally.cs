namespace DSAExperimentation.LeetCode.MostCommonWord;

// LC 819's one decision, named: the pair of structures that answers the two questions
// the counting pass asks about each token - "is this word banned" and "what is this
// word's new tally". Both strategies walk the same tokens with the same running best,
// so this pair is the entire difference between them, and the interface is where its
// contract gets written down, which the bare `Func<string, bool>` / `Func<string, int>`
// pair the pass used to take had nowhere to put.
internal interface IWordTally
{
    // Whether `word` is on the banned list, so the pass can skip it before counting.
    // `word` is a token already lower-cased and stripped of punctuation by the
    // tokenizer, and the banned list was lower-cased the same way when this tally was
    // built - which is what makes the two comparable at all. The answer is a pure read:
    // asking twice changes nothing, and asking never records the word.
    bool IsBanned(string word);

    // Counts one further sighting of `word` and answers that word's tally now. This is
    // the one method with a side effect: calling it twice for one token counts the token
    // twice, so the pass calls it exactly once per token it did not skip. The answer is
    // this word's own count, never the sum across words.
    int Count(string word);
}
