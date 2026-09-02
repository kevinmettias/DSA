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

    // The two-pointer walk's loop-carried state: the pending unmatched chunk's
    // start, the inward-crawling left/right pointers, and the chunk count so far.
    private readonly record struct DecompositionState(int MatchStart, int I, int J, int Count);

    // Left/right start of a candidate chunk pair, plus how long that candidate
    // chunk currently is - the unit ChunksMatch screens then verifies for equality.
    private readonly record struct ChunkPair(int LeftStart, int RightStart, int Length);

    private static int LongestDecomposition(string text)
    {
        var hash = new RollingHash(text);
        var state = new DecompositionState(MatchStart: 0, I: 0, J: text.Length - 1, Count: 0);

        while (state.I < state.J)
        {
            state = AdvanceStep(text, hash, state);
        }

        var count = state.Count;
        if (state.MatchStart <= state.J)
        {
            count++;
        }

        return count;
    }

    // Grows the pending chunk by one character on each side; once the growing
    // left/right chunks match, pairs them off and starts a fresh pending chunk.
    private static DecompositionState AdvanceStep(string text, RollingHash hash, DecompositionState state)
    {
        var len = state.I - state.MatchStart + 1;
        var matchStart = state.MatchStart;
        var count = state.Count;

        if (ChunksMatch(text, hash, new ChunkPair(matchStart, state.J, len)))
        {
            count += 2;
            matchStart = state.I + 1;
        }

        return state with { MatchStart = matchStart, I = state.I + 1, J = state.J - 1, Count = count };
    }

    private static bool ChunksMatch(string text, RollingHash hash, ChunkPair pair)
    {
        if (hash.Hash(pair.LeftStart, pair.Length) != hash.Hash(pair.RightStart, pair.Length))
        {
            return false;
        }

        var leftChunk = text.AsSpan(pair.LeftStart, pair.Length);
        var rightChunk = text.AsSpan(pair.RightStart, pair.Length);
        return leftChunk.SequenceEqual(rightChunk);
    }
}
