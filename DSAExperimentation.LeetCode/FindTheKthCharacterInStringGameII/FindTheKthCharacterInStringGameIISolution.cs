namespace DSAExperimentation.LeetCode.FindTheKthCharacterInStringGameII;

// LeetCode 3307. Find the K-th Character in String Game II: word starts as
// "a"; operations[i] == 0 doubles word onto itself unchanged, operations[i]
// == 1 appends a +1-shifted copy (the same rule Game I, LC 3304, always
// applies). k can reach 10^16, so the doubling word itself may never actually
// be built - only its length is tracked round by round, and the target
// position is traced backward through the operations to the single seed
// character it descends from.
internal static class FindTheKthCharacterInStringGameIISolution
{
    private const int AlphabetSize = 26;

    // The textbook answer: build the actual string exactly as the operations
    // describe, then index it directly. Correct for any input, but the word
    // doubles on every operations[i] == 0, so this only stays usable while
    // the operation count is small - the arm the backward trace below has to
    // agree with wherever both remain tractable.
    public static char KthCharacterByBruteForceSimulation(long k, int[] operations)
    {
        var word = new List<char> { 'a' };

        foreach (var operation in operations)
        {
            var roundLength = word.Count;

            for (var i = 0; i < roundLength; i++)
            {
                word.Add(operation == 1 ? NextChar(word[i]) : CharAt(word, i));
            }
        }

        return word[(int)(k - 1)];
    }

    private static char NextChar(char c) => c == 'z' ? 'a' : ShiftedChar(c);

    // One letter on in the alphabet.
    private static char ShiftedChar(char c) => (char)(c + 1);

    // The character already sitting at this position of the round's word.
    private static char CharAt(List<char> word, int index) => word[index];

    // Never materializes word. First replays the length word would reach
    // after each operation - frozen once it exceeds k, since position (which
    // only ever shrinks from k) can then never fall in the untouched first
    // half again, so the exact magnitude beyond that point is irrelevant and
    // freezing it is what keeps the running total from overflowing a long
    // across up to 100 doublings. The position is then traced back through
    // the operations those lengths came from.
    public static char KthCharacterByBackwardTrace(long k, int[] operations)
    {
        var lengths = new long[operations.Length + 1];
        lengths[0] = 1;

        for (var i = 0; i < operations.Length; i++)
        {
            var isRoundFrozen = lengths[i] > k;
            lengths[i + 1] = isRoundFrozen ? LengthAt(lengths, i) : DoubledLengthAt(lengths, i);
        }

        var shift = TraceShiftBackward(k, operations, lengths);

        return (char)('a' + shift % AlphabetSize);
    }

    // The round's length as it stands, and the same length one doubling on.
    private static long LengthAt(long[] lengths, int index) => lengths[index];

    private static long DoubledLengthAt(long[] lengths, int index) => lengths[index] * 2;

    // Walks the operations backward from the final length: at each step position k
    // either sits in the untouched first half (left alone) or the appended second half
    // (folded back into the first half's coordinates, recording one more shift if that
    // round was a +1 shift). What position finally folds down to is always 1 - the
    // original seed 'a' - after however many shifts were collected along the way.
    private static int TraceShiftBackward(long k, int[] operations, long[] lengths)
    {
        var position = k;
        var shift = 0;

        for (var i = operations.Length - 1; i >= 0; i--)
        {
            var lengthBeforeOperation = lengths[i];

            if (position > lengthBeforeOperation)
            {
                position -= lengthBeforeOperation;

                if (operations[i] == 1)
                {
                    shift++;
                }
            }
        }

        return shift;
    }
}
