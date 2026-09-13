using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.LargestComponentSizeByCommonFactor;

// LeetCode 952. Largest Component Size by Common Factor: values are vertices, two
// values are adjacent when they share a factor greater than 1, and the answer is the
// size of the largest connected component. Both strategies partition with a union-find
// and then tally component sizes - what separates them is how they decide that two
// values belong together:
//
//   PairwiseGcd asks the question literally, computing Gcd(nums[i], nums[j]) for every
//   pair - O(n^2 log(maxValue)) - over a plain int[]-backed union-find with neither path
//   compression nor rank, so Find/relink degrade to O(n). This is the textbook arm, and
//   it exists here rather than in the benchmark so that it is actually asserted.
//
//   PrimeFactorUnion never compares two values at all. Each value is factorized by trial
//   division, and each factor is unioned with the FIRST index that ever carried it
//   (tracked in a HashMap<factor,index>) - the same union-on-shared-key shape
//   AccountsMerge applies to emails, applied to prime factors instead. Sharing a factor
//   is discovered in O(1) the moment the second value reaches it, giving
//   O(n*sqrt(maxValue)*alpha(n)) over this repo's own DisjointSet.
internal static class LargestComponentSizeByCommonFactorSolution
{
    private const int SmallestPrimeFactor = 2;

    // The naive arm: decide adjacency by computing a gcd for every pair, and union
    // through a hand-rolled parent array. Internals are deliberately all BCL - this is
    // what you would write without this repo.
    public static int LargestComponentSizeByPairwiseGcd(int[] nums)
    {
        var parent = InitializeParent(nums.Length);
        UnionPairsSharingFactor(nums, parent);

        var counts = new int[nums.Length];
        var largest = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            var root = Find(parent, i);
            counts[root]++;
            largest = Math.Max(largest, counts[root]);
        }

        return largest;
    }

    private static int[] InitializeParent(int length)
    {
        var parent = new int[length];

        for (var i = 0; i < parent.Length; i++)
        {
            parent[i] = i;
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
    public static int LargestComponentSizeByPrimeFactorUnion(int[] nums)
    {
        var components = new DisjointSet(nums.Length);
        UnionSharedFactors(nums, components);

        var sizeByRoot = new HashMap<int, int>();
        var largest = 1;

        for (var i = 0; i < nums.Length; i++)
        {
            var count = TallyComponent(components, sizeByRoot, i);
            largest = Math.Max(largest, count);
        }

        return largest;
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

    private static int TallyComponent(DisjointSet components, HashMap<int, int> sizeByRoot, int index)
    {
        var root = components.Find(index);
        sizeByRoot.TryGetValue(root, out var count);
        count++;
        sizeByRoot.Set(root, count);

        return count;
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
