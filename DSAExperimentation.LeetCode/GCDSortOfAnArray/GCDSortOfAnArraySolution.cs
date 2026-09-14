using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.GCDSortOfAnArray;

// LeetCode 1998. GCD Sort of an Array: nums[i] and nums[j] may be swapped whenever
// gcd(nums[i], nums[j]) > 1, any number of times. Repeated swaps inside a connected
// component of that relation can realize any permutation of the values in it, so the
// array is sortable exactly when every position's original value and its sorted value
// already lie in the same component. That makes this a union-find question plus a sort,
// and the two strategies differ in how they build each half:
//
//   PairwiseGcdUnionFind asks the adjacency question literally - for every pair of
//   DISTINCT values present in nums, compute the gcd and union when it exceeds 1,
//   O(distinctValues^2 * log(maxValue)) - over a plain int[] parent array with neither
//   path compression nor rank, then sorts the comparison copy with BCL Array.Sort. The
//   textbook arm, first-class here rather than hidden in a benchmark so that it is
//   actually asserted.
//
//   PrimeFactorDisjointSet never compares two values against each other. Each value is
//   trial-divided and unioned directly with each of its own prime factors, so two values
//   sharing a factor join transitively through that factor's node - the same
//   union-on-shared-key idiom LargestComponentSizeByCommonFactorSolution uses, keyed
//   value<->factor instead of index<->owner because here it is the values themselves, not
//   their positions, that need a shared component. Both halves are this repo's own:
//   DisjointSet for the partition, MergeSort over an ArrayIndexedSequence for the sort.
//
// Both arms ask the same final question, so they agree position by position; the ids in
// play are the values (and, for the second arm, their factors), which is why the forest
// is sized to max(nums) + 1 rather than to nums.Length.
internal static class GCDSortOfAnArraySolution
{
    private const int SmallestPrimeFactor = 2;

    // The naive arm: literal pairwise gcd adjacency over a hand-rolled parent array,
    // sorted with Array.Sort. Internals are deliberately all BCL.
    public static bool CanBeSortedByPairwiseGcdUnionFind(int[] nums)
    {
        var parent = InitializeParent(nums.Max() + 1);
        UnionDistinctValuesSharingFactor(parent, nums.Distinct().ToArray());

        var sorted = (int[])nums.Clone();
        Array.Sort(sorted);

        return SameComponentAtEveryPosition(parent, nums, sorted);
    }

    private static int[] InitializeParent(int count)
    {
        var parent = new int[count];

        for (var id = 0; id < parent.Length; id++)
        {
            parent[id] = id;
        }

        return parent;
    }

    private static void UnionDistinctValuesSharingFactor(int[] parent, int[] distinctValues)
    {
        for (var i = 0; i < distinctValues.Length; i++)
        {
            for (var j = i + 1; j < distinctValues.Length; j++)
            {
                if (Gcd(distinctValues[i], distinctValues[j]) > 1)
                {
                    Union(parent, distinctValues[i], distinctValues[j]);
                }
            }
        }
    }

    private static bool SameComponentAtEveryPosition(int[] parent, int[] nums, int[] sorted)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            if (Find(parent, nums[i]) != Find(parent, sorted[i]))
            {
                return false;
            }
        }

        return true;
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

    // This repo's DisjointSet, unioned value<->prime factor so no two values are ever
    // compared, with the comparison copy produced by this repo's own MergeSort.
    public static bool CanBeSortedByPrimeFactorDisjointSet(int[] nums)
    {
        var components = BuildFactorComponents(nums);
        var sorted = SortedCopy(nums);

        for (var i = 0; i < nums.Length; i++)
        {
            if (!components.IsConnected(nums[i], sorted[i]))
            {
                return false;
            }
        }

        return true;
    }

    private static DisjointSet BuildFactorComponents(int[] nums)
    {
        var components = new DisjointSet(nums.Max() + 1);

        foreach (var value in nums)
        {
            foreach (var factor in PrimeFactors(value))
            {
                components.Union(value, factor);
            }
        }

        return components;
    }

    private static int[] SortedCopy(int[] nums)
    {
        var sorted = (int[])nums.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        return sorted;
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
