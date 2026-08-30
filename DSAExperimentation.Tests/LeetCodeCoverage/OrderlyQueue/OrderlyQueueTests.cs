using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.DataStructures.SuffixArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OrderlyQueue;

// LeetCode 899. Orderly Queue: k == 1 only allows rotating the string (moving the
// front character to the back), so the reachable set is exactly its n rotations -
// found here via this repo's own SuffixArray over s+s (the smallest suffix starting
// below n) instead of comparing all n rotations by hand. k > 1 is known to make
// every permutation reachable (two adjacent characters can always be swapped via a
// rotate-forward/rotate-back pair), so the answer there is simply s sorted - done
// with this repo's own MergeSort over ArrayIndexedSequence, the same primitive
// HIndex/ThreeSum already use for character/number sorting.
public sealed partial class OrderlyQueueTests
{
    [Fact]
    public void OrderlyQueue_KEqualsOne_ReturnsSmallestRotation()
    {
        Assert.Equal("acb", OrderlyQueue("cba", 1));
    }

    [Fact]
    public void OrderlyQueue_KGreaterThanOne_ReturnsSortedString()
    {
        Assert.Equal("aaabc", OrderlyQueue("baaca", 3));
    }

    [Fact]
    public void OrderlyQueue_SingleCharacter_ReturnsSameString()
    {
        Assert.Equal("z", OrderlyQueue("z", 1));
    }

    private static string OrderlyQueue(string s, int k)
    {
        if (k > 1)
        {
            var chars = s.ToCharArray();
            MergeSort.Sort<char, ArrayIndexedSequence<char>>(new ArrayIndexedSequence<char>(chars));
            return new string(chars);
        }

        var doubled = s + s;
        var suffixArray = new SuffixArray(doubled);

        foreach (var start in suffixArray.Suffixes)
        {
            if (start < s.Length)
            {
                return doubled.Substring(start, s.Length);
            }
        }

        return s;
    }
}
