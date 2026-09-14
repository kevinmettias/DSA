using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.GreatestCommonDivisorTraversal;

// LeetCode 2709. Greatest Common Divisor Traversal: indices i and j are adjacent when
// gcd(nums[i], nums[j]) > 1, and the answer is whether EVERY pair of indices is joined
// by some sequence of such steps - that is, whether the whole array is a single
// connected component. gcd(a, b) > 1 exactly when a and b share a prime factor, so both
// strategies partition the indices with a union-find and then check that index 0 reaches
// every other one. What separates them is how adjacency is discovered:
//
//   PairwiseGcd asks the question literally, computing Gcd(nums[i], nums[j]) for every
//   pair - O(n^2 log(maxValue)) - over a plain int[]-backed union-find with neither path
//   compression nor rank. The textbook arm, first-class here rather than hidden in a
//   benchmark so that it is actually asserted.
//
//   PrimeFactorUnion never compares two values at all. Each value is factorized by trial
//   division and each factor is unioned with the FIRST index that ever carried it
//   (tracked in a HashMap<factor,index>), so sharing a factor is discovered in O(1) the
//   moment the second value reaches it - O(n*sqrt(maxValue)*alpha(n)) over this repo's
//   own DisjointSet. This is LargestComponentSizeByCommonFactorSolution's union applied
//   to a full-connectivity check instead of a largest-component tally.
//
// A value of 1 has no prime factors at all, so it can never join anything: an array of
// length greater than one that contains a 1 is always false, on both arms.
internal static class GreatestCommonDivisorTraversalSolution
{
    private const int SmallestPrimeFactor = 2;

    // The naive arm: decide adjacency by computing a gcd for every pair, and union
    // through a hand-rolled parent array. Internals are deliberately all BCL - this is
    // what you would write without this repo.
    public static bool CanTraverseAllPairsByPairwiseGcd(int[] nums)
    {
        var parent = InitializeParent(nums.Length);
        UnionPairsSharingFactor(nums, parent);

        for (var i = 1; i < nums.Length; i++)
        {
            if (Find(parent, i) != Find(parent, 0))
            {
                return false;
            }
        }

        return true;
    }

    private static int[] InitializeParent(int length)
    {
        var parent = new int[length];

        for (var id = 0; id < parent.Length; id++)
        {
            parent[id] = id;
        }

        return parent;
    }

    private static void UnionPairsSharingFactor(int[] nums, int[] parent)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                if (Gcd(nums[i], nums[j]) > 1)
                {
                    Union(parent, i, j);
                }
            }
        }
    }

    private static int Gcd(int first, int second) => second == 0 ? first : Gcd(second, first % second);

    private static int Find(int[] parent, int id)
    {
        while (parent[id] != id)
        {
            id = parent[id];
        }

        return id;
    }

    private static void Union(int[] parent, int first, int second)
    {
        var firstRoot = Find(parent, first);
        var secondRoot = Find(parent, second);

        if (firstRoot != secondRoot)
        {
            parent[firstRoot] = secondRoot;
        }
    }

    // This repo's DisjointSet, unioned by first-seen owner per prime factor so no two
    // values are ever compared directly.
    public static bool CanTraverseAllPairsByPrimeFactorUnion(int[] nums)
    {
        var components = new DisjointSet(nums.Length);
        UnionSharedFactors(nums, components);

        for (var i = 1; i < nums.Length; i++)
        {
            if (!components.IsConnected(0, i))
            {
                return false;
            }
        }

        return true;
    }

    private static void UnionSharedFactors(int[] nums, DisjointSet components)
    {
        var firstIndexWithFactor = new HashMap<int, int>();

        for (var i = 0; i < nums.Length; i++)
        {
            foreach (var factor in PrimeFactors(nums[i]))
            {
                if (firstIndexWithFactor.TryGetValue(factor, out var owner))
                {
                    components.Union(i, owner);
                }
                else
                {
                    firstIndexWithFactor.Set(factor, i);
                }
            }
        }
    }

    private static IEnumerable<int> PrimeFactors(int value)
    {
        for (var factor = SmallestPrimeFactor; factor * factor <= value; factor++)
        {
            if (value % factor != 0)
            {
                continue;
            }

            yield return factor;

            while (value % factor == 0)
            {
                value /= factor;
            }
        }

        if (value > 1)
        {
            yield return value;
        }
    }
}
