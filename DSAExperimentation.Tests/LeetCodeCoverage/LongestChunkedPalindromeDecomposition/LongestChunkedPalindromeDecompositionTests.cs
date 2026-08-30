using DSAExperimentation.DataStructures.RollingHash;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestChunkedPalindromeDecomposition;

// LeetCode 1147. Longest Chunked Palindrome Decomposition: the classic greedy
// two-pointer walk - grow a pending chunk from the left (matchStart..i) and its
// mirror from the right (j..matchStart+len-1's counterpart) in lockstep, and the
// moment the two are equal, pair them off and start a fresh pending chunk. This
// repo's own RollingHash gives an O(1) equality SCREEN for the growing pair
// instead of a fresh O(len) substring comparison at every growth step - screened
// hits are still confirmed with a real SequenceEqual first, the same
// screen-then-verify shape RollingHashSearch.FindAll already uses, since a hash
// match is "probably equal," not "definitely equal" (RollingHashValue's own doc
// comment). Any leftover pending/singleton chunk once the pointers meet or cross
// becomes exactly one final, unpaired middle chunk.
public sealed partial class LongestChunkedPalindromeDecompositionTests
{
    [Theory]
    [InlineData("ghiabcdefhelloadamhelloabcdefghi", 7)]
    [InlineData("merchant", 1)]
    [InlineData("antaprezatepzapreanta", 11)]
    public void LongestDecomposition_LeetCodeExamples_ReturnsMaxChunkCount(string text, int expected)
        => Assert.Equal(expected, LongestDecomposition(text));

    private static int LongestDecomposition(string text)
    {
        var hash = new RollingHash(text);
        var matchStart = 0;
        var i = 0;
        var j = text.Length - 1;
        var count = 0;

        while (i < j)
        {
            var len = i - matchStart + 1;

            if (ChunksMatch(text, hash, matchStart, j, len))
            {
                count += 2;
                matchStart = i + 1;
            }

            i++;
            j--;
        }

        if (matchStart <= j)
        {
            count++;
        }

        return count;
    }

    private static bool ChunksMatch(string text, RollingHash hash, int leftStart, int rightStart, int length)
    {
        if (hash.Hash(leftStart, length) != hash.Hash(rightStart, length))
        {
            return false;
        }

        return text.AsSpan(leftStart, length).SequenceEqual(text.AsSpan(rightStart, length));
    }
}
