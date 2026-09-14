using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MinimumNumberOfMovesToMakePalindrome;

// LeetCode 2193. Minimum Number of Moves to Make Palindrome: the fewest adjacent
// swaps that turn s into a palindrome.
//
// Both strategies run the same greedy two-pointer argument - for each outer pair,
// find the match for s[i] nearest the far end and walk it in one adjacent step at
// a time; a character with no match left in (i, j] is the eventual middle of an
// odd-length palindrome, so nudge it one step inward and retry the same i. What
// differs is the buffer the walk mutates: a BCL List<char> moved with
// RemoveAt/Insert versus this repo's ArrayIndexedSequence<char> moved with direct
// Get/Set swaps. Same O(n^2) algorithm and same answer, different real cost purely
// from the representation - the "performance independence" case ARCHITECTURE.md #8
// names.
internal static class MinimumNumberOfMovesToMakePalindromeSolution
{
    // The textbook answer: a BCL List<char>, moving a matched character to its
    // destination with one RemoveAt/Insert pair and charging the distance it
    // travelled. Deliberately written without this repo's sequence contracts - it
    // is the arm the composed strategy below has to justify itself against.
    public static int MinMovesByListRemoveInsert(string s)
    {
        var chars = new List<char>(s);
        var moves = 0;
        int i = 0, j = chars.Count - 1;

        while (i < j)
        {
            (i, j, moves) = ResolveOuterPair(chars, i, j, moves);
        }

        return moves;
    }

    private static (int I, int J, int Moves) ResolveOuterPair(List<char> chars, int i, int j, int moves)
    {
        if (chars[i] == chars[j])
        {
            return (i + 1, j - 1, moves);
        }

        var k = j;

        while (k > i && chars[k] != chars[i])
        {
            k--;
        }

        return k == i ? NudgeMiddleCharacter(chars, i, j, moves) : WalkMatchInward(chars, k, i, j, moves);
    }

    // No match for chars[i] anywhere in (i, j]: it is the lone middle character of
    // an odd-length palindrome, so it moves one step inward and the same i is
    // retried against the new occupant.
    private static (int I, int J, int Moves) NudgeMiddleCharacter(List<char> chars, int i, int j, int moves)
    {
        var middle = chars[i];
        chars.RemoveAt(i);
        chars.Insert(i + 1, middle);

        return (i, j, moves + 1);
    }

    private static (int I, int J, int Moves) WalkMatchInward(List<char> chars, int k, int i, int j, int moves)
    {
        var match = chars[k];
        chars.RemoveAt(k);
        chars.Insert(j, match);

        return (i + 1, j - 1, moves + (j - k));
    }

    // The same greedy walk over IIndexedSequence's doubled O(1) Get/Set contract -
    // ArrayIndexedSequence<char>, the same composition ArrayPartition uses for
    // MergeSort - so moving a character costs a run of adjacent swaps rather than
    // a shift of the whole tail.
    public static int MinMovesByIndexedSequenceSwap(string s)
    {
        var sequence = new ArrayIndexedSequence<char>(s.ToCharArray());
        var moves = 0;
        int i = 0, j = sequence.Length - 1;

        while (i < j)
        {
            (i, j, moves) = ResolveOuterPair(sequence, i, j, moves);
        }

        return moves;
    }

    private static (int I, int J, int Moves) ResolveOuterPair(
        ArrayIndexedSequence<char> sequence, int i, int j, int moves)
    {
        if (sequence.Get(i) == sequence.Get(j))
        {
            return (i + 1, j - 1, moves);
        }

        var k = j;

        while (k > i && sequence.Get(k) != sequence.Get(i))
        {
            k--;
        }

        return k == i ? NudgeMiddleCharacter(sequence, i, j, moves) : WalkMatchInward(sequence, k, i, j, moves);
    }

    private static (int I, int J, int Moves) NudgeMiddleCharacter(
        ArrayIndexedSequence<char> sequence, int i, int j, int moves)
    {
        Swap(sequence, i, i + 1);

        return (i, j, moves + 1);
    }

    private static (int I, int J, int Moves) WalkMatchInward(
        ArrayIndexedSequence<char> sequence, int k, int i, int j, int moves)
    {
        while (k < j)
        {
            Swap(sequence, k, k + 1);
            moves++;
            k++;
        }

        return (i + 1, j - 1, moves);
    }

    private static void Swap(ArrayIndexedSequence<char> sequence, int first, int second)
    {
        (var a, var b) = (sequence.Get(first), sequence.Get(second));
        sequence.Set(first, b);
        sequence.Set(second, a);
    }
}
