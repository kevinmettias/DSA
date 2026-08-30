using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfValidWordsForEachPuzzle;

// LeetCode 1178. Number of Valid Words for Each Puzzle: each word/puzzle collapses
// to a 26-bit letter-presence mask; this repo's own HashMap<int,int> counts words by
// mask, then each puzzle's answer sums the counts of every submask of its own mask
// that still contains the puzzle's first letter (the standard submask-enumeration
// trick: subset = (subset - 1) & mask).
public sealed partial class NumberOfValidWordsForEachPuzzleTests
{
    [Fact]
    public void FindNumOfValidWords_ClassicExample_ReturnsMatchingWordCountsPerPuzzle()
    {
        string[] words = ["aaaa", "asas", "able", "ability", "actt", "actor", "access"];
        string[] puzzles = ["aboveyz", "abrodyz", "abslute", "absoryz", "actresz", "gaswxyz"];

        var counts = FindNumOfValidWords(words, puzzles);

        Assert.Equal([1, 1, 3, 2, 4, 0], counts);
    }

    private static List<int> FindNumOfValidWords(string[] words, string[] puzzles)
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
            result.Add(CountValidWords(wordCountsByMask, LetterMask(puzzle), 1 << (puzzle[0] - 'a')));
        }

        return result;
    }

    private static int CountValidWords(HashMap<int, int> wordCountsByMask, int puzzleMask, int firstLetterBit)
    {
        var total = 0;
        var submask = puzzleMask;

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

    private static int LetterMask(string value)
    {
        var mask = 0;
        foreach (var ch in value)
        {
            mask |= 1 << (ch - 'a');
        }

        return mask;
    }
}
