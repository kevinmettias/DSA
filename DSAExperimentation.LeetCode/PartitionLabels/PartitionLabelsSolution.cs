using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.PartitionLabels;

// LeetCode 763. Partition Labels: split a string into the fewest contiguous
// partitions such that every letter appears in at most one partition, returning
// each partition's length.
//
// A partition can close the instant the scan reaches the furthest last-occurrence
// seen among the letters admitted so far; the two strategies differ only in how
// each letter's last occurrence is found - re-derived with a backward scan every
// time a partition's frontier expands, or looked up in one HashMap<char,int> built
// up front (TwoSumSolution's exact shape).
internal static class PartitionLabelsSolution
{
    // The textbook answer: no precomputed index, so extending a partition's
    // frontier means rescanning the string backward for each letter it now
    // contains - O(n) per lookup, O(n^2) worst case. Written without this
    // repo's primitives - the arm the one-pass strategy below has to justify
    // itself against.
    public static List<int> PartitionLabelSizesByBruteForceRescan(string text)
    {
        var sizes = new List<int>();
        var start = 0;

        while (start < text.Length)
        {
            start = ExtendPartitionFromStart(text, start, sizes);
        }

        return sizes;
    }

    private static int ExtendPartitionFromStart(string text, int start, List<int> sizes)
    {
        var end = start;
        var i = start;

        while (i <= end)
        {
            var last = LastIndexOf(text, text[i]);

            if (last > end)
            {
                end = last;
            }

            i++;
        }

        sizes.Add(end - start + 1);
        return end + 1;
    }

    private static int LastIndexOf(string text, char target)
    {
        for (var j = text.Length - 1; j >= 0; j--)
        {
            if (text[j] == target)
            {
                return j;
            }
        }

        return -1;
    }

    // Every letter's last occurrence is recorded once, up front, in this repo's
    // own HashMap<char,int>; the scan then only ever grows the partition's end
    // to the furthest last occurrence already known, one lookup per character.
    public static List<int> PartitionLabelSizesByHashMapOnePass(string text)
    {
        var lastIndex = new HashMap<char, int>();

        for (var i = 0; i < text.Length; i++)
        {
            lastIndex.Set(text[i], i);
        }

        var sizes = new List<int>();
        var start = 0;
        var end = 0;

        for (var i = 0; i < text.Length; i++)
        {
            lastIndex.TryGetValue(text[i], out var furthest);
            end = Math.Max(end, furthest);
            start = ClosePartitionIfComplete(end, i, start, sizes);
        }

        return sizes;
    }

    private static int ClosePartitionIfComplete(int end, int index, int start, List<int> sizes)
    {
        if (index != end)
        {
            return start;
        }

        sizes.Add(end - start + 1);
        return index + 1;
    }
}
