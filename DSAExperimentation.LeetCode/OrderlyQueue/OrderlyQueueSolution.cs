using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.DataStructures.SuffixArray;

namespace DSAExperimentation.LeetCode.OrderlyQueue;

// LeetCode 899. Orderly Queue: repeatedly move any one of the first
// movablePrefixLength characters of text to the back; return the lexicographically
// smallest string reachable.
//
// The problem splits on movablePrefixLength. A value of 1 only allows rotating the
// string (the front character goes to the back), so the reachable set is exactly its
// n rotations. Any larger value makes every permutation reachable - two adjacent
// characters can always be swapped via a rotate-forward/rotate-back pair - so the
// answer is simply text sorted. Both strategies below agree on the
// movablePrefixLength > 1 half and differ only in how they find the smallest
// rotation when it is 1.
internal static class OrderlyQueueSolution
{
    // movablePrefixLength == 1 is the rotation-only case; anything larger reaches
    // every permutation.
    private const int RotationOnly = 1;

    // The textbook answer: materialize each of the n rotations and keep the
    // smallest, O(n) rotations x O(n) comparison each. Deliberately written with
    // nothing but the BCL - it is the arm the composed strategy below has to
    // justify itself against.
    public static string SmallestStringByBruteForceRotations(string text, int movablePrefixLength)
    {
        if (movablePrefixLength > RotationOnly)
        {
            var chars = text.ToCharArray();
            Array.Sort(chars);
            return new string(chars);
        }

        var best = text;

        for (var start = 1; start < text.Length; start++)
        {
            var head = text.AsSpan(start);
            var tail = text.AsSpan(0, start);
            var rotation = string.Concat(head, tail);

            if (string.CompareOrdinal(rotation, best) < 0)
            {
                best = rotation;
            }
        }

        return best;
    }

    // This repo's own primitives on both halves: MergeSort over an
    // ArrayIndexedSequence<char> for the movablePrefixLength > 1 sort (the same composition
    // HIndex/ThreeSum use for character/number sorting), and a SuffixArray over
    // text + text for the rotation-only case - the first suffix in lexicographic order
    // whose start lies below text.Length is exactly the smallest rotation, found in
    // O(n log^2 n) instead of comparing all n rotations by hand.
    public static string SmallestStringBySuffixArray(string text, int movablePrefixLength)
    {
        if (movablePrefixLength > RotationOnly)
        {
            var chars = text.ToCharArray();
            MergeSort.Sort<char, ArrayIndexedSequence<char>>(new ArrayIndexedSequence<char>(chars));
            return new string(chars);
        }

        var doubled = text + text;
        var suffixArray = new SuffixArray(doubled);

        foreach (var start in suffixArray.Suffixes)
        {
            if (start < text.Length)
            {
                return doubled.Substring(start, text.Length);
            }
        }

        return text;
    }
}
