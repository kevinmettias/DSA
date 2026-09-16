using DSAExperimentation.DataStructures.RollingHash;

namespace DSAExperimentation.LeetCode.LongestChunkedPalindromeDecomposition;

// LeetCode 1147. Longest Chunked Palindrome Decomposition: split text into the
// greatest number of chunks whose sequence reads the same forwards and backwards -
// chunk k must equal chunk (count - 1 - k).
//
// Both strategies are the same greedy two-pointer walk: grow a pending chunk from
// the left and its mirror from the right in lockstep, and the moment the two are
// equal, pair them off and start a fresh pending chunk. Any leftover pending chunk
// once the pointers meet or cross becomes exactly one final, unpaired middle chunk.
// Greedy is optimal here because the shortest matching prefix/suffix pair can
// always be peeled first without costing a later pair.
//
// They differ only in how the growing pair is compared. LongestDecompositionBy-
// StringConcatenation is the textbook form: rebuild both sides as strings on every
// growth step and compare them with ==, O(len) per step. LongestDecompositionBy-
// RollingHash uses this repo's own RollingHash for an O(1) equality SCREEN over
// text's own indices, with screened hits still confirmed by a real SequenceEqual -
// the same screen-then-verify shape RollingHashSearch.FindAll uses, since a hash
// match is "probably equal," not "definitely equal" (RollingHashValue's own doc
// comment).
internal static class LongestChunkedPalindromeDecompositionSolution
{
    // A matched left/right pair contributes two chunks to the decomposition.
    private const int MatchedPairChunkCount = 2;

    // Real answer: RollingHash screens each growing pair in O(1), SequenceEqual
    // confirms, and no substring is ever materialized.
    public static int LongestDecompositionByRollingHash(string text)
    {
        var hash = new RollingHash(text);
        var state = new DecompositionState(MatchStart: 0, I: 0, J: text.Length - 1, Count: 0);

        while (state.I < state.J)
        {
            state = AdvanceHashStep(text, hash, state);
        }

        return state.Count + (state.MatchStart <= state.J ? 1 : 0);
    }

    // Grows the pending chunk by one character on each side; once the growing
    // left/right chunks match, pairs them off and starts a fresh pending chunk.
    private static DecompositionState AdvanceHashStep(string text, RollingHash hash, DecompositionState state)
    {
        var length = state.I - state.MatchStart + 1;
        var matchStart = state.MatchStart;
        var count = state.Count;

        if (IsChunkPairEqual(text, hash, new ChunkPair(matchStart, state.J, length)))
        {
            count += MatchedPairChunkCount;
            matchStart = state.I + 1;
        }

        return state with { MatchStart = matchStart, I = state.I + 1, J = state.J - 1, Count = count };
    }

    private static bool IsChunkPairEqual(string text, RollingHash hash, ChunkPair pair)
    {
        if (hash.Hash(pair.LeftStart, pair.Length) != hash.Hash(pair.RightStart, pair.Length))
        {
            return false;
        }

        var leftChunk = text.AsSpan(pair.LeftStart, pair.Length);
        var rightChunk = text.AsSpan(pair.RightStart, pair.Length);
        return leftChunk.SequenceEqual(rightChunk);
    }

    // The textbook answer: grow `left` and `right` by string concatenation and
    // compare them with ==. Deliberately written without this repo's primitives -
    // it is the arm the composed strategy above has to justify itself against.
    public static int LongestDecompositionByStringConcatenation(string text)
    {
        var state = new ConcatenationState(string.Empty, string.Empty, Count: 0, I: 0, J: text.Length - 1);

        while (state.I < state.J)
        {
            state = AdvanceConcatenationStep(text, state);
        }

        return state.Count + (state.Left.Length > 0 || state.I == state.J ? 1 : 0);
    }

    private static ConcatenationState AdvanceConcatenationStep(string text, ConcatenationState state)
    {
        var left = state.Left + text[state.I];
        var right = text[state.J] + state.Right;
        var count = state.Count;

        if (left == right)
        {
            count += MatchedPairChunkCount;
            left = string.Empty;
            right = string.Empty;
        }

        return state with { Left = left, Right = right, Count = count, I = state.I + 1, J = state.J - 1 };
    }

    // The two-pointer walk's loop-carried state: the pending unmatched chunk's
    // start, the inward-crawling left/right pointers, and the chunk count so far.
    private readonly record struct DecompositionState(int MatchStart, int I, int J, int Count);

    // Left/right start of a candidate chunk pair, plus how long that candidate
    // chunk currently is - the unit IsChunkPairEqual screens then verifies for equality.
    private readonly record struct ChunkPair(int LeftStart, int RightStart, int Length);

    // The same walk carrying its pending chunks as materialized strings instead of
    // as indices into text.
    private readonly record struct ConcatenationState(string Left, string Right, int Count, int I, int J);
}
