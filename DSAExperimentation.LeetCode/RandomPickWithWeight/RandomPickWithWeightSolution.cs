using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.RandomPickWithWeight;

// LeetCode 528. Random Pick with Weight: given weights w, repeatedly return an
// index chosen with probability proportional to its weight.
//
// A Design problem's whole point is a sequence of calls against one instance, so
// "every strategy for the problem" (§17.3) takes the form of two classes
// implementing a shared IRandomPickWithWeight surface, the same shape
// RandomPickIndexSolution already uses for its own Design-category problem. Both
// build a cumulative-sum array of w once at construction and draw one uniform
// integer over the total on every PickIndex call; they differ only in how that
// draw is turned into an index - a linear scan of the prefix sums, or this
// repo's own BinarySearch.UpperBound over an ArraySequence<int> view of the same
// array, the same prefix-sum-plus-BinarySearch pairing
// RandomPointInNonOverlappingRectanglesSolution uses for area-weighted sampling.
internal static class RandomPickWithWeightSolution
{
    internal interface IRandomPickWithWeight
    {
        int PickIndex();
    }

    // The textbook per-pick linear scan: walk the prefix sums until the running
    // total exceeds the draw. O(n) per PickIndex; the arm the binary search
    // strategy below has to justify itself against. Deliberately written without
    // this repo's search primitives - only the seeded Random the caller supplies
    // is a repo-adjacent choice, not part of the algorithm's textbook character.
    internal sealed class RandomPickWithWeightByLinearScan : IRandomPickWithWeight
    {
        private readonly int[] _prefixSums;
        private readonly Random _random;

        public RandomPickWithWeightByLinearScan(int[] w)
            : this(w, new Random())
        {
        }

        public RandomPickWithWeightByLinearScan(int[] w, Random random)
        {
            _random = random;
            _prefixSums = BuildPrefixSums(w);
        }

        public int PickIndex()
        {
            var draw = _random.Next(_prefixSums[^1]);
            var index = 0;

            while (_prefixSums[index] <= draw)
            {
                index++;
            }

            return index;
        }
    }

    // This repo's own BinarySearch.UpperBound over an ArraySequence<int> of the
    // same prefix sums finds the same index in O(log n) per PickIndex.
    internal sealed class RandomPickWithWeightByBinarySearchUpperBound : IRandomPickWithWeight
    {
        private readonly int[] _prefixSums;
        private readonly Random _random;

        public RandomPickWithWeightByBinarySearchUpperBound(int[] w)
            : this(w, new Random())
        {
        }

        public RandomPickWithWeightByBinarySearchUpperBound(int[] w, Random random)
        {
            _random = random;
            _prefixSums = BuildPrefixSums(w);
        }

        public int PickIndex()
        {
            var sequence = new ArraySequence<int>(_prefixSums);
            var draw = _random.Next(_prefixSums[^1]);

            return BinarySearch.UpperBound(sequence, draw);
        }
    }

    private static int[] BuildPrefixSums(int[] w)
    {
        var prefixSums = new int[w.Length];
        var running = 0;

        for (var i = 0; i < w.Length; i++)
        {
            running += w[i];
            prefixSums[i] = running;
        }

        return prefixSums;
    }
}
