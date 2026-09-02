using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.RollingHash;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestCommonSubpath;

// LeetCode 1923. Longest Common Subpath: binary search over the candidate length L,
// screened at each L by this repo's own RollingHash - city ids are first
// coordinate-compressed into a shared char alphabet via this repo's own
// HashMap<int,char> (the same compression
// FormArrayByConcatenatingSubarraysOfAnotherArrayTests already uses, since
// RollingHash itself only operates over ReadOnlySpan<char>), then every path's
// length-L window hashes are intersected across all n paths using this repo's own
// Set<T> (HashMap-backed). RollingHash's own doc comment names exactly this
// cross-text "longest-common-substring" use case as the reason a caller may compare
// RollingHashValues across independently-built instances (same comparer, same lanes
// - guaranteed here since every path is encoded through one shared HashMap<int,char>
// and RollingHash's default constructor, so all instances agree). A zero-length
// window always hashes to the same (0,0) value on every path (see RollingHash.Hash's
// own doc comment - the combination formula self-cancels), so Check(0) is trivially
// true with no special-casing: the binary search's floor is exactly right without an
// extra branch.
public sealed partial class LongestCommonSubpathTests
{
    [Fact]
    public void LongestCommonSubpath_ClassicExample_ReturnsSharedRunLength()
    {
        int[][] paths = [[0, 1, 2, 3, 4], [2, 3, 4], [4, 0, 1, 2, 3]];

        var length = FindLongestCommonSubpath(paths);

        Assert.Equal(2, length);
    }

    [Fact]
    public void LongestCommonSubpath_NoOverlapAtAll_ReturnsZero()
    {
        int[][] paths = [[0], [1], [2]];

        var length = FindLongestCommonSubpath(paths);

        Assert.Equal(0, length);
    }

    [Fact]
    public void LongestCommonSubpath_OnlySingleCityShared_ReturnsOne()
    {
        int[][] paths = [[0, 1, 2, 3, 4], [4, 3, 2, 1, 0]];

        var length = FindLongestCommonSubpath(paths);

        Assert.Equal(1, length);
    }

    private static int FindLongestCommonSubpath(int[][] paths)
    {
        var codes = BuildCharCodes(paths);
        var encoded = EncodeAll(paths, codes);
        var high = ShortestPathLength(encoded);

        return BinarySearchLongestLength(encoded, high);
    }

    private static char[][] EncodeAll(int[][] paths, HashMap<int, char> codes)
    {
        var encoded = new char[paths.Length][];

        for (var i = 0; i < paths.Length; i++)
        {
            encoded[i] = Encode(paths[i], codes);
        }

        return encoded;
    }

    private static int ShortestPathLength(char[][] encoded)
    {
        var shortest = int.MaxValue;

        foreach (var path in encoded)
        {
            shortest = Math.Min(shortest, path.Length);
        }

        return shortest;
    }

    private static int BinarySearchLongestLength(char[][] encoded, int high)
    {
        var low = 0;

        while (low < high)
        {
            var mid = low + ((high - low + 1) / 2);

            if (HasCommonSubpathOfLength(encoded, mid))
            {
                low = mid;
            }
            else
            {
                high = mid - 1;
            }
        }

        return low;
    }

    private static HashMap<int, char> BuildCharCodes(int[][] paths)
    {
        var codes = new HashMap<int, char>();

        foreach (var path in paths)
        {
            foreach (var value in path)
            {
                if (!codes.HasKey(value))
                {
                    codes.Set(value, (char)codes.Count);
                }
            }
        }

        return codes;
    }

    private static char[] Encode(int[] path, HashMap<int, char> codes)
    {
        var encoded = new char[path.Length];

        for (var i = 0; i < path.Length; i++)
        {
            codes.TryGetValue(path[i], out encoded[i]);
        }

        return encoded;
    }

    private static bool HasCommonSubpathOfLength(char[][] encoded, int length)
    {
        var candidates = CollectWindowHashes(encoded[0], length);

        for (var i = 1; i < encoded.Length; i++)
        {
            var pathHashes = new Set<RollingHashValue>();
            var hash = new RollingHash(encoded[i]);

            for (var start = 0; start + length <= encoded[i].Length; start++)
            {
                var windowHash = hash.Hash(start, length);
                pathHashes.TryAdd(windowHash);
            }

            candidates = IntersectWithPath(candidates, pathHashes);

            if (candidates.Count == 0)
            {
                return false;
            }
        }

        return candidates.Count > 0;
    }

    private static List<RollingHashValue> CollectWindowHashes(char[] text, int length)
    {
        var seen = new Set<RollingHashValue>();
        var distinct = new List<RollingHashValue>();
        var hash = new RollingHash(text);

        for (var start = 0; start + length <= text.Length; start++)
        {
            var value = hash.Hash(start, length);

            if (seen.TryAdd(value))
            {
                distinct.Add(value);
            }
        }

        return distinct;
    }

    private static List<RollingHashValue> IntersectWithPath(List<RollingHashValue> candidates, Set<RollingHashValue> pathHashes)
    {
        var remaining = new List<RollingHashValue>();

        foreach (var candidate in candidates)
        {
            if (pathHashes.Has(candidate))
            {
                remaining.Add(candidate);
            }
        }

        return remaining;
    }
}
