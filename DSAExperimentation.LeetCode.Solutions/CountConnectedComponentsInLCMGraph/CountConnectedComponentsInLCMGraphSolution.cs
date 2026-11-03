using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.Solutions.CountConnectedComponentsInLCMGraph;

// LeetCode 3378. Count Connected Components in LCM Graph: nums is a set of
// distinct values; an (undirected) edge joins nums[i] and nums[j] whenever
// lcm(nums[i], nums[j]) <= threshold. Count connected components.
[LeetCodeQuestion("count-connected-components-in-lcm-graph", LeetCodeAnswerOrder.AsWritten)]
internal static class CountConnectedComponentsInLCMGraphSolution
{
    // Baseline: the graph exactly as LeetCode defines it. Every pair gets a real
    // lcm check (via a private Euclid's-algorithm gcd - no repo primitive for that
    // exists, the same as this catalogue's other unmigrated gcd-based baselines),
    // and only a verified edge is ever unioned. The union-find is a hand-written pair
    // of parent and rank arrays over index space, because that's all a true
    // edge-by-edge scan needs. O(n^2 log(maxValue)).
    public static int CountComponentsByPairwiseLcmScan(int[] nums, int threshold)
    {
        var n = nums.Length;
        var parent = SingletonParents(n);
        var rank = new int[n];

        for (var i = 0; i < n; i++)
        {
            for (var j = i + 1; j < n; j++)
            {
                if (Lcm(nums[i], nums[j]) <= threshold)
                {
                    UnionByRank(parent, rank, i, j);
                }
            }
        }

        var roots = new HashSet<int>();

        for (var i = 0; i < n; i++)
        {
            var root = FindRoot(parent, i);
            roots.Add(root);
        }

        return roots.Count;
    }

    // Every index starts as the root of its own one-element set.
    private static int[] SingletonParents(int count)
    {
        var parent = new int[count];

        for (var i = 0; i < count; i++)
        {
            parent[i] = i;
        }

        return parent;
    }

    private static long Lcm(long firstValue, long secondValue) =>
        firstValue / Gcd(firstValue, secondValue) * secondValue;

    // Hangs the lower-ranked root under the higher one, so no tree grows taller than
    // log n; a tie keeps the first root on top and raises its rank.
    private static void UnionByRank(int[] parent, int[] rank, int first, int second)
    {
        var firstRoot = FindRoot(parent, first);
        var secondRoot = FindRoot(parent, second);

        if (firstRoot == secondRoot)
        {
            return;
        }

        if (rank[firstRoot] < rank[secondRoot])
        {
            (firstRoot, secondRoot) = (secondRoot, firstRoot);
        }

        parent[secondRoot] = firstRoot;

        if (rank[firstRoot] == rank[secondRoot])
        {
            rank[firstRoot]++;
        }
    }

    // Composed: DisjointSet(threshold + 1) over the VALUE space rather than index
    // space. For a present value x, lcm(x, k*x) is k*x by construction, so
    // unioning x with every multiple of x up to threshold only ever adds real
    // edges. Conversely, whenever lcm(a, b) <= threshold, that lcm m is a common
    // multiple of both a and b, so the union performed while processing a bridges
    // a-to-m and the union performed while processing b bridges b-to-m - the two
    // meet at m and a, b land in the same component. No pairwise lcm check is ever
    // needed. O(threshold log threshold) instead of O(n^2 log(maxValue)).
    public static int CountComponentsByMultipleUnion(int[] nums, int threshold)
    {
        var forest = new DisjointSet(threshold + 1);
        UnionMultiplesOfPresentValues(forest, nums, threshold);

        var roots = new HashSet<int>();
        var isolatedAboveThreshold = 0;

        foreach (var value in nums)
        {
            if (value > threshold)
            {
                // No multiple of any other value can reach past threshold to meet
                // it, and its own multiples all exceed threshold too - it can only
                // ever be its own component.
                isolatedAboveThreshold++;
            }
            else
            {
                roots.Add(forest.Find(value));
            }
        }

        return roots.Count + isolatedAboveThreshold;
    }

    // Every present value at or below threshold is unioned with each of its own
    // multiples, and nothing else: lcm(x, k*x) is k*x by construction, so each of
    // those unions is a real edge.
    private static void UnionMultiplesOfPresentValues(DisjointSet forest, int[] nums, int threshold)
    {
        foreach (var value in nums)
        {
            if (value > threshold)
            {
                continue;
            }

            for (var multiple = 2 * value; multiple <= threshold; multiple += value)
            {
                forest.Union(value, multiple);
            }
        }
    }

    // Walks up to the root, halving the path on the way so later finds take fewer steps.
    private static int FindRoot(int[] parent, int id)
    {
        while (parent[id] != id)
        {
            parent[id] = parent[parent[id]];
            id = parent[id];
        }

        return id;
    }

    private static long Gcd(long firstValue, long secondValue) =>
        secondValue == 0 ? firstValue : Gcd(secondValue, firstValue % secondValue);

}
