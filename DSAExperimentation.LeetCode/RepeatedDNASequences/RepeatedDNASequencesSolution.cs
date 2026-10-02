using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.RepeatedDNASequences;

// LeetCode 187. Repeated DNA Sequences: every 10-letter substring of a DNA string
// that occurs more than once, each reported exactly once, in order of its first
// occurrence.
//
// Two strategies for the same windows: slide a fixed 10-character string window
// over the sequence, or encode each base to two bits and roll a 20-bit integer
// mask, which keys the windows with no substring per position. Both do two
// passes - one to mark which windows repeat, one to report them in first-
// occurrence order.
internal static class RepeatedDNASequencesSolution
{
    private const int SequenceLength = 10;
    private const int BitsPerBase = 2;
    private const int TenBaseMask = (1 << (SequenceLength * BitsPerBase)) - 1;

    // Rolls a 20-bit mask as the window slides: A/C/G/T map to the two-bit codes
    // 0..3, so a 10-base window is exactly 20 bits and each new base is one shift,
    // one or, and one mask. The integer is the seen-key, so no substring is
    // allocated per position - against the fixed-window arm's 10-character
    // substring, at the cost of the per-base encoding and bit arithmetic.
    public static List<string> FindByRollingTwoBitMask(string sequence)
    {
        var repeated = RepeatedMasks(sequence);

        if (repeated.Count == 0)
        {
            return [];
        }

        return CollectFirstOccurrences(sequence, repeated);
    }

    // First pass for the mask arm: roll the window once, recording every mask met
    // more than once.
    private static Set<int> RepeatedMasks(string sequence)
    {
        var seen = new Set<int>();
        var repeated = new Set<int>();
        var mask = 0;

        for (var i = 0; i < sequence.Length; i++)
        {
            mask = Roll(mask, sequence[i]);

            if (i >= SequenceLength - 1 && !seen.TryAdd(mask))
            {
                repeated.TryAdd(mask);
            }
        }

        return repeated;
    }

    // Second pass for the mask arm: roll the window again, reporting each repeated
    // mask the first time it is met, so the result keeps first-occurrence order
    // and holds no duplicates.
    private static List<string> CollectFirstOccurrences(string sequence, Set<int> repeated)
    {
        var added = new Set<int>();
        var result = new List<string>();
        var mask = 0;

        for (var i = 0; i < sequence.Length; i++)
        {
            mask = Roll(mask, sequence[i]);

            if (i >= SequenceLength - 1 && repeated.Has(mask) && added.TryAdd(mask))
            {
                result.Add(sequence.Substring(i - SequenceLength + 1, SequenceLength));
            }
        }

        return result;
    }

    // One base folded into the rolling window: shift the previous 20 bits up by
    // two, or in this base's code, and keep only the low 20 bits.
    private static int Roll(int mask, char nucleotide) =>
        ((mask << BitsPerBase) | Encode(nucleotide)) & TenBaseMask;

    private static int Encode(char nucleotide) => nucleotide switch
    {
        'A' => 0,
        'C' => 1,
        'G' => 2,
        _ => 3,
    };

    // The fixed-window arm: a fixed 10-character window slid across the string in
    // two passes (the first collects which windows repeat, the second walks the
    // string again reporting each repeated window the first time it is met) backed
    // by this repo's own Set<string>.
    public static List<string> FindByFixedWindowSet(string sequence)
    {
        var seen = new Set<string>();
        var repeated = new Set<string>();

        for (var i = 0; i + SequenceLength <= sequence.Length; i++)
        {
            var window = sequence.Substring(i, SequenceLength);

            if (!seen.TryAdd(window))
            {
                repeated.TryAdd(window);
            }
        }

        if (repeated.Count == 0)
        {
            return [];
        }

        return CollectFirstOccurrences(sequence, repeated);
    }

    // The second pass: walk the string again, reporting each repeated window the first
    // time it is met, so the result keeps first-occurrence order and holds no duplicates.
    private static List<string> CollectFirstOccurrences(string sequence, Set<string> repeated)
    {
        var added = new Set<string>();
        var result = new List<string>();

        for (var i = 0; i + SequenceLength <= sequence.Length; i++)
        {
            var window = sequence.Substring(i, SequenceLength);

            if (repeated.Has(window) && added.TryAdd(window))
            {
                result.Add(window);
            }
        }

        return result;
    }
}
