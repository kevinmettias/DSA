using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimizeHammingDistanceAfterSwapOperations;

// LeetCode 1722. Minimize Hamming Distance After Swap Operations: DisjointSet over the
// array's own index positions (the SmallestStringWithSwapsTests shape - union every
// allowed-swap pair, then group indices by root), then per component compare the
// multiset of source values against target values with a HashMap<value,int> frequency
// count instead of sorting - any permutation is reachable within a component via some
// sequence of swaps, so the minimum achievable Hamming distance for that component is
// just how many values target needs that source's own multiset can't already supply.
public sealed partial class MinimizeHammingDistanceAfterSwapOperationsTests
{
    [Fact]
    public void MinimumHammingDistance_ClassicExample_MatchesExpectedDistance()
    {
        int[] source = [1, 2, 3, 4];
        int[] target = [2, 1, 4, 5];
        int[][] allowedSwaps = [[0, 1], [2, 3]];

        var hammingDistance = MinimumHammingDistance(source, target, allowedSwaps);
        Assert.Equal(1, hammingDistance);
    }

    [Fact]
    public void MinimumHammingDistance_NoAllowedSwaps_CountsPositionalMismatchesOnly()
    {
        int[] source = [1, 2, 3, 4];
        int[] target = [1, 3, 2, 4];
        int[][] allowedSwaps = [];

        var hammingDistance = MinimumHammingDistance(source, target, allowedSwaps);
        Assert.Equal(2, hammingDistance);
    }

    [Fact]
    public void MinimumHammingDistance_TransitiveSwapsMergeAllIndices_ReachesZero()
    {
        int[] source = [5, 1, 2, 4, 3];
        int[] target = [1, 5, 4, 2, 3];
        int[][] allowedSwaps = [[0, 4], [4, 2], [1, 3], [1, 4]];

        var hammingDistance = MinimumHammingDistance(source, target, allowedSwaps);
        Assert.Equal(0, hammingDistance);
    }

    private static int MinimumHammingDistance(int[] source, int[] target, int[][] allowedSwaps)
    {
        var components = BuildComponents(source.Length, allowedSwaps);
        var indicesByRoot = GroupIndicesByRoot(source, components);

        return SumComponentDistances(indicesByRoot, source, target);
    }

    private static DisjointSet BuildComponents(int length, int[][] allowedSwaps)
    {
        var components = new DisjointSet(length);

        foreach (var swap in allowedSwaps)
        {
            components.Union(swap[0], swap[1]);
        }

        return components;
    }

    private static HashMap<int, List<int>> GroupIndicesByRoot(int[] source, DisjointSet components)
    {
        var indicesByRoot = new HashMap<int, List<int>>();
        for (var i = 0; i < source.Length; i++)
        {
            var root = components.Find(i);
            if (!indicesByRoot.TryGetValue(root, out var indices))
            {
                indices = [];
                indicesByRoot.Set(root, indices);
            }

            indices.Add(i);
        }

        return indicesByRoot;
    }

    private static int SumComponentDistances(HashMap<int, List<int>> indicesByRoot, int[] source, int[] target)
    {
        var distance = 0;
        foreach (var root in indicesByRoot.Keys)
        {
            indicesByRoot.TryGetValue(root, out var indices);
            distance += ComponentHammingDistance(indices!, source, target);
        }

        return distance;
    }

    // A component's minimum contribution to the Hamming distance: how many
    // target values that component's own source multiset can't already supply.
    private static int ComponentHammingDistance(List<int> indices, int[] source, int[] target)
    {
        var availableCounts = new HashMap<int, int>();
        foreach (var i in indices)
        {
            availableCounts.TryGetValue(source[i], out var count);
            availableCounts.Set(source[i], count + 1);
        }

        var distance = 0;
        foreach (var i in indices)
        {
            if (availableCounts.TryGetValue(target[i], out var count) && count > 0)
            {
                availableCounts.Set(target[i], count - 1);
            }
            else
            {
                distance++;
            }
        }

        return distance;
    }
}
