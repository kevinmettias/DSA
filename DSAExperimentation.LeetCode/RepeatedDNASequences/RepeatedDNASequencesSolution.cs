using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.RepeatedDNASequences;

// LeetCode 187. Repeated DNA Sequences: every 10-letter substring of a DNA string
// that occurs more than once, each reported exactly once, in order of its first
// occurrence.
//
// One strategy: the original inline test's private helper was already exactly
// this - a fixed 10-character window slid across the string in two passes (the
// first collects which windows repeat, the second walks the string again
// reporting each repeated window the first time it is met) backed by this repo's
// own Set<string>. The original benchmark's two [Benchmark] arms were both
// `return 1` placeholders, not a second algorithm to reconcile against.
internal static class RepeatedDNASequencesSolution
{
    private const int SequenceLength = 10;

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
