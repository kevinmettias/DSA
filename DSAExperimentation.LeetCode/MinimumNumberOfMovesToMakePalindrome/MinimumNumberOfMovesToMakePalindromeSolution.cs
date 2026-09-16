using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MinimumNumberOfMovesToMakePalindrome;

// LeetCode 2193. Minimum Number of Moves to Make Palindrome: the fewest adjacent
// swaps that turn `text` into a palindrome.
//
// Both strategies run the same greedy two-pointer argument - for each outer pair,
// find the match for `text[leftIndex]` nearest the far end and walk it in one
// adjacent step at a time; a character with no match left in
// (leftIndex, rightIndex] is the eventual middle of an odd-length palindrome, so
// nudge it one step inward and retry the same leftIndex. What differs is the
// buffer the walk mutates: a BCL List<char> moved with RemoveAt/Insert versus this
// repo's ArrayIndexedSequence<char> moved with direct Get/Set swaps. Same O(n^2)
// algorithm and same answer, different real cost purely from the representation -
// the "performance independence" case ARCHITECTURE.md #8 names.
internal static class MinimumNumberOfMovesToMakePalindromeSolution
{
    // The textbook answer: a BCL List<char>, moving a matched character to its
    // destination with one RemoveAt/Insert pair and charging the distance it
    // travelled. Deliberately written without this repo's sequence contracts - it
    // is the arm the composed strategy below has to justify itself against.
    public static int MinMovesByListRemoveInsert(string text)
    {
        var chars = new List<char>(text);
        var moves = 0;
        int leftIndex = 0, rightIndex = chars.Count - 1;

        while (leftIndex < rightIndex)
        {
            (leftIndex, rightIndex, moves) = ResolveOuterPair(chars, leftIndex, rightIndex, moves);
        }

        return moves;
    }

    private static (int I, int J, int Moves) ResolveOuterPair(
        List<char> chars, int leftIndex, int rightIndex, int moves)
    {
        if (chars[leftIndex] == chars[rightIndex])
        {
            return (leftIndex + 1, rightIndex - 1, moves);
        }

        var matchIndex = rightIndex;

        while (matchIndex > leftIndex && chars[matchIndex] != chars[leftIndex])
        {
            matchIndex--;
        }

        return matchIndex == leftIndex
            ? NudgeMiddleCharacter(chars, leftIndex, rightIndex, moves)
            : WalkMatchInward(chars, matchIndex, (leftIndex, rightIndex), moves);
    }

    // No match for chars[leftIndex] anywhere in (leftIndex, rightIndex]: it is the lone
    // middle character of an odd-length palindrome, so it moves one step inward and the
    // same leftIndex is retried against the new occupant.
    private static (int I, int J, int Moves) NudgeMiddleCharacter(
        List<char> chars, int leftIndex, int rightIndex, int moves)
    {
        var middle = chars[leftIndex];
        chars.RemoveAt(leftIndex);
        chars.Insert(leftIndex + 1, middle);

        return (leftIndex, rightIndex, moves + 1);
    }

    private static (int I, int J, int Moves) WalkMatchInward(
        List<char> chars, int matchIndex, (int I, int J) outer, int moves)
    {
        var match = chars[matchIndex];
        chars.RemoveAt(matchIndex);
        chars.Insert(outer.J, match);

        return (outer.I + 1, outer.J - 1, moves + (outer.J - matchIndex));
    }

    // The same greedy walk over IIndexedSequence's doubled O(1) Get/Set contract -
    // ArrayIndexedSequence<char>, the same composition ArrayPartition uses for
    // MergeSort - so moving a character costs a run of adjacent swaps rather than
    // a shift of the whole tail.
    public static int MinMovesByIndexedSequenceSwap(string text)
    {
        var sequence = new ArrayIndexedSequence<char>(text.ToCharArray());
        var moves = 0;
        int leftIndex = 0, rightIndex = sequence.Length - 1;

        while (leftIndex < rightIndex)
        {
            (leftIndex, rightIndex, moves) = ResolveOuterPair(sequence, leftIndex, rightIndex, moves);
        }

        return moves;
    }

    private static (int I, int J, int Moves) ResolveOuterPair(
        ArrayIndexedSequence<char> sequence, int leftIndex, int rightIndex, int moves)
    {
        if (sequence.Get(leftIndex) == sequence.Get(rightIndex))
        {
            return (leftIndex + 1, rightIndex - 1, moves);
        }

        var matchIndex = rightIndex;

        while (matchIndex > leftIndex && sequence.Get(matchIndex) != sequence.Get(leftIndex))
        {
            matchIndex--;
        }

        return matchIndex == leftIndex
            ? NudgeMiddleCharacter(sequence, leftIndex, rightIndex, moves)
            : WalkMatchInward(sequence, matchIndex, (leftIndex, rightIndex), moves);
    }

    private static (int I, int J, int Moves) NudgeMiddleCharacter(
        ArrayIndexedSequence<char> sequence, int leftIndex, int rightIndex, int moves)
    {
        Swap(sequence, leftIndex, leftIndex + 1);

        return (leftIndex, rightIndex, moves + 1);
    }

    private static (int I, int J, int Moves) WalkMatchInward(
        ArrayIndexedSequence<char> sequence, int matchIndex, (int I, int J) outer, int moves)
    {
        while (matchIndex < outer.J)
        {
            Swap(sequence, matchIndex, matchIndex + 1);
            moves++;
            matchIndex++;
        }

        return (outer.I + 1, outer.J - 1, moves);
    }

    private static void Swap(ArrayIndexedSequence<char> sequence, int first, int second)
    {
        (var firstValue, var secondValue) = (sequence.Get(first), sequence.Get(second));
        sequence.Set(first, secondValue);
        sequence.Set(second, firstValue);
    }
}
