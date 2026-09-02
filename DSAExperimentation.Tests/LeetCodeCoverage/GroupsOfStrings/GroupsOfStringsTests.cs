using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GroupsOfStrings;

// LeetCode 2157. Groups of Strings: each word (letters are distinct within a word,
// per the problem's own constraint) collapses to a 26-bit letter-set mask. Two
// masks are "connected" when they're equal, differ by exactly one bit (add/delete
// one letter), or have equal popcount and differ in exactly two bit positions
// (replace one letter with another). Rather than the O(n^2) pairwise scan
// AccountsMergeTests/SimilarStringGroupsTests use for their own (unavoidably
// pairwise) connection tests, a mask's neighbors are enumerable directly - clear one
// set bit (delete), then clear-and-set a different bit (replace) - so a
// HashMap<mask, representativeWordIndex> (the same "first-seen owner" role
// AccountsMergeTests' HashMap<email,accountIndex> plays) turns each word's O(26^2)
// neighbor generation into an O(1) lookup per candidate instead of an O(n) scan.
// Connected words are unioned via DisjointSet over word indices, exactly
// AccountsMergeTests/SimilarStringGroupsTests' own "union whenever two elements are
// connected, tally by root" shape; group sizes are tallied with a second
// HashMap<root, size> rather than SimilarStringGroupsTests' Set<int> since this
// problem also needs the largest group's size, not just the group count.
public sealed partial class GroupsOfStringsTests
{
    [Fact]
    public void GroupSizes_LeetCodeExampleOne_GroupsAddDeleteAndReplaceConnectedWords()
    {
        string[] words = ["a", "b", "ab", "cde"];

        var result = GroupSizes(words);

        Assert.Equal([2, 3], result);
    }

    [Fact]
    public void GroupSizes_LeetCodeExampleTwo_ChainsThroughDeleteConnectionsIntoOneGroup()
    {
        string[] words = ["a", "ab", "abc"];

        var result = GroupSizes(words);

        Assert.Equal([1, 3], result);
    }

    [Fact]
    public void GroupSizes_DuplicateWords_TreatsIdenticalLetterSetsAsConnected()
    {
        string[] words = ["ab", "ab", "xyz"];

        var result = GroupSizes(words);

        Assert.Equal([2, 2], result);
    }

    private static int[] GroupSizes(string[] words)
    {
        var masks = BuildMasks(words);
        var representativeByMask = BuildRepresentativeByMask(masks);
        var components = UnionAllNeighbors(masks, representativeByMask);

        return TallyGroups(masks.Length, components);
    }

    private static int[] BuildMasks(string[] words)
    {
        var masks = new int[words.Length];

        for (var i = 0; i < words.Length; i++)
        {
            masks[i] = ComputeMask(words[i]);
        }

        return masks;
    }

    private static HashMap<int, int> BuildRepresentativeByMask(int[] masks)
    {
        var representativeByMask = new HashMap<int, int>();

        for (var i = 0; i < masks.Length; i++)
        {
            if (!representativeByMask.HasKey(masks[i]))
            {
                representativeByMask.Set(masks[i], i);
            }
        }

        return representativeByMask;
    }

    private static DisjointSet UnionAllNeighbors(int[] masks, HashMap<int, int> representativeByMask)
    {
        var components = new DisjointSet(masks.Length);

        for (var i = 0; i < masks.Length; i++)
        {
            UnionNeighbors(i, masks[i], representativeByMask, components);
        }

        return components;
    }

    private readonly record struct UnionContext(HashMap<int, int> RepresentativeByMask, DisjointSet Components);

    private static void UnionNeighbors(int i, int mask, HashMap<int, int> representativeByMask, DisjointSet components)
    {
        if (representativeByMask.TryGetValue(mask, out var sameMask) && sameMask != i)
        {
            components.Union(i, sameMask);
        }

        var context = new UnionContext(representativeByMask, components);

        for (var removedBit = 0; removedBit < 26; removedBit++)
        {
            if ((mask & (1 << removedBit)) == 0)
            {
                continue;
            }

            var withoutLetter = mask & ~(1 << removedBit);
            UnionIfPresent(i, withoutLetter, representativeByMask, components);
            UnionReplacements(i, mask, withoutLetter, context);
        }
    }

    private static void UnionReplacements(int i, int mask, int withoutLetter, UnionContext context)
    {
        for (var addedBit = 0; addedBit < 26; addedBit++)
        {
            if ((mask & (1 << addedBit)) != 0)
            {
                continue;
            }

            UnionIfPresent(i, withoutLetter | (1 << addedBit), context.RepresentativeByMask, context.Components);
        }
    }

    private static void UnionIfPresent(int i, int candidateMask, HashMap<int, int> representativeByMask, DisjointSet components)
    {
        if (representativeByMask.TryGetValue(candidateMask, out var other))
        {
            components.Union(i, other);
        }
    }

    private static int[] TallyGroups(int n, DisjointSet components)
    {
        var sizeByRoot = new HashMap<int, int>();
        var groupCount = 0;
        var largest = 0;

        for (var i = 0; i < n; i++)
        {
            var root = components.Find(i);
            var size = sizeByRoot.TryGetValue(root, out var existing) ? existing + 1 : 1;
            sizeByRoot.Set(root, size);

            if (size == 1)
            {
                groupCount++;
            }

            if (size > largest)
            {
                largest = size;
            }
        }

        return [groupCount, largest];
    }

    private static int ComputeMask(string word)
    {
        var mask = 0;

        foreach (var c in word)
        {
            mask |= 1 << (c - 'a');
        }

        return mask;
    }
}
