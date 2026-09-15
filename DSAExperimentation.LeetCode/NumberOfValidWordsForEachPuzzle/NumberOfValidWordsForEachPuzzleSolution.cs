using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.NumberOfValidWordsForEachPuzzle;

// LeetCode 1178. Number of Valid Words for Each Puzzle: a word is valid for a
// puzzle when it contains the puzzle's first letter and every one of its own
// letters appears in the puzzle. Only letter *presence* matters, so each word and
// each puzzle collapses to a 26-bit mask and the answer is one count per puzzle.
//
// Both strategies agree on the mask encoding and differ only in what they search:
// the baseline re-tests every word against every puzzle, while the composed arm
// groups words by mask in this repo's own HashMap<int,int> and then enumerates the
// puzzle mask's 2^7 submasks instead of the word list.
internal static class NumberOfValidWordsForEachPuzzleSolution
{
    private const char AlphabetStart = 'a';

    // The textbook answer: for each puzzle, walk every word and test the two
    // conditions directly. Deliberately BCL-only - it is the arm the composed
    // strategy below has to justify itself against. O(PuzzleCount * WordCount).
    public static List<int> CountValidWordsByMaskComparison(string[] words, string[] puzzles)
    {
        var result = new List<int>(puzzles.Length);

        foreach (var puzzle in puzzles)
        {
            var puzzleMask = LetterMask(puzzle);
            var firstLetterBit = FirstLetterBit(puzzle);
            var validCount = 0;

            foreach (var word in words)
            {
                var wordMask = LetterMask(word);

                if ((wordMask & firstLetterBit) != 0 && (wordMask & puzzleMask) == wordMask)
                {
                    validCount++;
                }
            }

            result.Add(validCount);
        }

        return result;
    }

    // This repo's own HashMap<int,int> counts words by letter mask once, after
    // which a puzzle never touches the word list again: every valid word's mask is
    // a submask of the puzzle's own 7-bit mask, so summing the counts of the 2^7
    // submasks that keep the first letter answers the puzzle outright.
    // O(WordCount + PuzzleCount * 128).
    public static List<int> CountValidWordsByMaskSubsetEnumeration(string[] words, string[] puzzles)
    {
        var wordCountsByMask = new HashMap<int, int>();

        foreach (var word in words)
        {
            var mask = LetterMask(word);
            wordCountsByMask.TryGetValue(mask, out var existing);
            wordCountsByMask.Set(mask, existing + 1);
        }

        var result = new List<int>(puzzles.Length);

        foreach (var puzzle in puzzles)
        {
            var validCount = SumSubmaskCounts(wordCountsByMask, LetterMask(puzzle), FirstLetterBit(puzzle));
            result.Add(validCount);
        }

        return result;
    }

    // The standard submask enumeration - subset = (subset - 1) & mask walks every
    // subset of puzzleMask exactly once, descending, and terminates after 0.
    private static int SumSubmaskCounts(HashMap<int, int> wordCountsByMask, int puzzleMask, int firstLetterBit)
    {
        var total = 0;
        var submask = puzzleMask;

        // Stops when the descending submask walk reaches 0: that last submask is counted, then the total is returned.
        while (true)
        {
            if ((submask & firstLetterBit) != 0 && wordCountsByMask.TryGetValue(submask, out var wordCount))
            {
                total += wordCount;
            }

            if (submask == 0)
            {
                break;
            }

            submask = (submask - 1) & puzzleMask;
        }

        return total;
    }

    private static int FirstLetterBit(string puzzle) => 1 << (puzzle[0] - AlphabetStart);

    private static int LetterMask(string value)
    {
        var mask = 0;

        foreach (var ch in value)
        {
            mask |= 1 << (ch - AlphabetStart);
        }

        return mask;
    }
}
