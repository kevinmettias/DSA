using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.PartitionString;

// LeetCode 3597. Partition String: build a segment one character at a time,
// and the instant it has never been seen as a finished segment before, close
// it out and start the next one from scratch. If s ends mid-segment while
// that segment is still a repeat, the leftover is simply never closed and
// never appears in the answer - LC's own reference solutions (every language)
// do exactly this, with no special-casing at the end of the loop, and Example
// 2 ("aaaa" -> ["a","aa"], not ["a","aa","a"]) is only explicable that way.
internal static class PartitionStringSolution
{
    // Textbook baseline: BCL HashSet<string>.Add already returns whether the
    // item was newly added - exactly the "has this segment been seen before"
    // check the problem describes - so the whole algorithm is one pass with
    // no repo primitive. The arm the composed strategy below has to justify
    // itself against.
    public static List<string> PartitionByHashSetScan(string s)
    {
        var seen = new HashSet<string>();
        var segments = new List<string>();
        var current = "";

        foreach (var c in s)
        {
            current += c;

            if (seen.Add(current))
            {
                segments.Add(current);
                current = "";
            }
        }

        return segments;
    }

    // This repo's own Set<T>: TryAdd has the identical "insert and report
    // whether it was new" contract as HashSet<T>.Add, so the composition is
    // the same OpenTheLock-style swap - the exact same algorithm, this repo's
    // container standing in for the BCL one.
    public static List<string> PartitionBySetScan(string s)
    {
        var seen = new Set<string>();
        var segments = new List<string>();
        var current = "";

        foreach (var c in s)
        {
            current += c;

            if (seen.TryAdd(current))
            {
                segments.Add(current);
                current = "";
            }
        }

        return segments;
    }
}
