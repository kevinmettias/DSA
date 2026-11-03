using DSAExperimentation.Algorithms.Sampling;

namespace DSAExperimentation.LeetCode.RandomPickWithWeight;

// LeetCode 528. Random Pick with Weight: given weights w, repeatedly return an
// index chosen with probability proportional to its weight.
//
// A Design problem's whole point is a sequence of calls against one instance, so
// "every strategy for the problem" (§17.3) takes the form of classes implementing a
// shared IRandomPickWithWeight surface, the same shape RandomPickIndexSolution
// already uses for its own Design-category problem. The weights are a weighted
// die (Domain/Games/Dice), and turning a uniform draw into a face is a sampler, an
// algorithm with interchangeable implementations: a linear scan of the prefix
// sums, the library's PrefixSumSampler (BinarySearch.UpperBound over them, the
// pairing RandomPointInNonOverlappingRectanglesSolution uses for area-weighted
// sampling), or its AliasSampler, Vose's O(1) alias table.
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

        public RandomPickWithWeightByLinearScan(int[] weights, Random random)
        {
            _random = random;
            _prefixSums = BuildPrefixSums(weights);
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

    // The library's PrefixSumSampler: BinarySearch.UpperBound over the same prefix sums finds the
    // same index in O(log n) per PickIndex, from the same single draw.
    internal sealed class RandomPickWithWeightByBinarySearchUpperBound : IRandomPickWithWeight
    {
        private readonly PrefixSumSampler _sampler;
        private readonly Random _random;

        public RandomPickWithWeightByBinarySearchUpperBound(int[] weights, Random random)
        {
            _random = random;
            _sampler = new PrefixSumSampler(weights);
        }

        public int PickIndex() => _sampler.IndexFor(_random.Next(_sampler.Total));
    }

    // Vose's alias table, the library's AliasSampler: a column drawn uniformly, then a ticket within
    // it, decide the index in O(1) per pick after O(n) to build, where the prefix sums take O(log n).
    internal sealed class RandomPickWithWeightByAliasTable : IRandomPickWithWeight
    {
        private readonly AliasSampler _sampler;
        private readonly Random _random;

        public RandomPickWithWeightByAliasTable(int[] weights, Random random)
        {
            _random = random;
            _sampler = new AliasSampler(weights);
        }

        public int PickIndex() => _sampler.IndexFor(_random.Next(_sampler.Columns), _random.Next(_sampler.Total));
    }

    private static int[] BuildPrefixSums(int[] weights)
    {
        var prefixSums = new int[weights.Length];
        var running = 0;

        for (var i = 0; i < weights.Length; i++)
        {
            running += weights[i];
            prefixSums[i] = running;
        }

        return prefixSums;
    }
}
