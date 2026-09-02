using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfMovesToMakePalindrome;

// LeetCode 2193. Minimum Number of Moves to Make Palindrome: the greedy two-pointer
// solution (for each outer pair, find the matching character nearest the far end and
// walk it in with adjacent swaps; an unmatched lone character - the eventual middle of
// an odd-length palindrome - gets nudged one step inward instead) needs nothing but
// indexed Get/Set over a mutable buffer, which is exactly Sorting's
// IIndexedSequence<Element> contract - ArrayIndexedSequence<char> here, same
// composition ArrayPartitionTests already uses for MergeSort.
public sealed partial class MinimumNumberOfMovesToMakePalindromeTests
{
    [Fact]
    public void MinMoves_ClassicExample_ReturnsMinimumAdjacentSwaps()
    {
        Assert.Equal(2, MinMoves("aabb"));
    }

    [Fact]
    public void MinMoves_OddLengthWithMiddleShift_ReturnsMinimumAdjacentSwaps()
    {
        Assert.Equal(2, MinMoves("letelt"));
    }

    [Fact]
    public void MinMoves_AlreadyAPalindrome_ReturnsZero()
    {
        Assert.Equal(0, MinMoves("ababa"));
    }

    private static int MinMoves(string s)
    {
        var sequence = new ArrayIndexedSequence<char>(s.ToCharArray());
        var moves = 0;
        int i = 0, j = sequence.Length - 1;

        while (i < j)
        {
            moves += ResolveOuterPair(sequence, ref i, ref j);
        }

        return moves;
    }

    private static int ResolveOuterPair(ArrayIndexedSequence<char> sequence, ref int i, ref int j)
    {
        if (sequence.Get(i) == sequence.Get(j))
        {
            i++;
            j--;
            return 0;
        }

        var k = j;
        while (k > i && sequence.Get(k) != sequence.Get(i))
        {
            k--;
        }

        return k == i
            ? NudgeMiddleCharacter(sequence, i)
            : WalkMatchInward(sequence, k, ref i, ref j);
    }

    private static int NudgeMiddleCharacter(ArrayIndexedSequence<char> sequence, int i)
    {
        // No match for chars[i] anywhere in (i, j]: it's the lone middle
        // character of an odd-length palindrome - nudge it one step inward
        // and retry the same i.
        Swap(sequence, i, i + 1);
        return 1;
    }

    private static int WalkMatchInward(ArrayIndexedSequence<char> sequence, int k, ref int i, ref int j)
    {
        var moves = 0;
        while (k < j)
        {
            Swap(sequence, k, k + 1);
            moves++;
            k++;
        }

        i++;
        j--;

        return moves;
    }

    private static void Swap(ArrayIndexedSequence<char> sequence, int first, int second)
    {
        (var a, var b) = (sequence.Get(first), sequence.Get(second));
        sequence.Set(first, b);
        sequence.Set(second, a);
    }
}
