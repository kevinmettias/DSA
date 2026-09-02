using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestStringWithSwaps;

// LeetCode 1202. Smallest String With Swaps: DisjointSet over the string's own
// indices (union every swappable pair - the same LexicographicallySmallestEquivalentString
// shape, but over index positions instead of the 26 letters), then one pass grouping
// indices by root and sorting each group's own characters into its own sorted index
// slots - any permutation is reachable within a connected component via some sequence
// of swaps, so sorting each component independently is optimal.
public sealed partial class SmallestStringWithSwapsTests
{
    [Fact]
    public void SmallestString_TwoDisjointPairs_SwapsWithinEachPairOnly()
    {
        var result = SmallestString("dcab", [[0, 3], [1, 2]]);

        Assert.Equal("bacd", result);
    }

    [Fact]
    public void SmallestString_TransitivelyConnectedPairs_MergesAllThreeIndices()
    {
        var result = SmallestString("dcab", [[0, 3], [1, 2], [0, 2]]);

        Assert.Equal("abcd", result);
    }

    [Fact]
    public void SmallestString_FullyConnectedChain_SortsEntireString()
    {
        var result = SmallestString("cba", [[0, 1], [1, 2]]);

        Assert.Equal("abc", result);
    }

    private static string SmallestString(string s, int[][] pairs)
    {
        var components = BuildComponents(s.Length, pairs);
        var groups = GroupIndicesByRoot(components, s.Length);
        return ApplySortedGroups(s, groups);
    }

    private static DisjointSet BuildComponents(int length, int[][] pairs)
    {
        var components = new DisjointSet(length);

        foreach (var pair in pairs)
        {
            components.Union(pair[0], pair[1]);
        }

        return components;
    }

    private static Dictionary<int, List<int>> GroupIndicesByRoot(DisjointSet components, int length)
    {
        var groups = new Dictionary<int, List<int>>();
        for (var i = 0; i < length; i++)
        {
            var root = components.Find(i);
            if (!groups.TryGetValue(root, out var indices))
            {
                indices = [];
                groups[root] = indices;
            }

            indices.Add(i);
        }

        return groups;
    }

    private static string ApplySortedGroups(string s, Dictionary<int, List<int>> groups)
    {
        var result = s.ToCharArray();
        foreach (var indices in groups.Values)
        {
            var chars = indices.Select(i => s[i]).OrderBy(c => c).ToArray();
            for (var j = 0; j < indices.Count; j++)
            {
                result[indices[j]] = chars[j];
            }
        }

        return new string(result);
    }
}
