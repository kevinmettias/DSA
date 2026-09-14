using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.RollingHash;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.LongestCommonSubpath;

// LeetCode 1923. Longest Common Subpath: the length of the longest contiguous run of
// cities that every one of the given paths contains.
//
// Both strategies binary-search the candidate length L - "is there a run of length L
// shared by all n paths" is monotone in L, so the answer is the largest L that
// passes - and screen each L by intersecting one path's length-L window keys into
// the next until either the intersection empties or every path has been folded in.
// The only thing that differs is what a single window's key costs to produce:
// O(length), by materializing the window itself, or O(1).
//
// City ids are first coordinate-compressed into a shared char alphabet through this
// repo's own HashMap<int,char> (the same compression
// FormArrayByConcatenatingSubarraysOfAnotherArray and SmallestUniqueSubarray use,
// since RollingHash itself only operates over ReadOnlySpan<char>), then every path's
// window hashes are intersected using this repo's own Set<T>. RollingHash's own doc
// comment names exactly this cross-text "longest-common-substring" use case as the
// reason a caller may compare RollingHashValues across independently-built instances
// (same comparer, same lanes - guaranteed here since every path is encoded through
// one shared HashMap<int,char> and RollingHash's default constructor, so all
// instances agree). A zero-length window always hashes to the same (0,0) value on
// every path (see RollingHash.Hash's own doc comment - the combination formula
// self-cancels), so length 0 is trivially shared with no special-casing: the binary
// search's floor is exactly right without an extra branch.
internal static class LongestCommonSubpathSolution
{
    // The binary search takes the upper half whenever the midpoint length passes, so
    // it rounds the midpoint up and can never stall on low == mid.
    private const int BinarySearchMidpointDivisor = 2;

    // Separates city ids inside a naive window key so two different runs can never
    // spell the same string (12,3 vs 1,23).
    private const char CityIdSeparator = ',';

    // The textbook answer: key each length-L window by the window's own text, built
    // fresh per window with string.Join over the raw city ids, and intersect through
    // a BCL HashSet<string>. Deliberately written without this repo's primitives -
    // it is the O(length)-per-window arm the hashed strategy below has to justify
    // itself against.
    public static int LongestCommonSubpathByNaiveWindowKeys(int[][] paths)
    {
        var high = ShortestPathLength(paths);

        return BinarySearchLongestSharedLength(high, length => IsSharedByNaiveKeys(paths, length));
    }

    // This repo's own RollingHash turns "what does this window hash to" into an O(1)
    // prefix lookup, so the per-window key cost stops depending on L at all - the
    // same asymmetry DistinctEchoSubstrings demonstrates for a single string, here
    // applied across n paths at once.
    public static int LongestCommonSubpathByRollingHash(int[][] paths)
    {
        var codes = BuildCharCodes(paths);
        var encoded = EncodeAll(paths, codes);
        var high = ShortestPathLength(encoded);

        return BinarySearchLongestSharedLength(high, length => IsSharedByWindowHashes(encoded, length));
    }

    // Shared between both strategies, so the only thing they differ in is the
    // per-window key: the largest length every path still shares.
    private static int BinarySearchLongestSharedLength(int maxLength, Func<int, bool> isShared)
    {
        var low = 0;
        var high = maxLength;

        while (low < high)
        {
            var mid = low + ((high - low + 1) / BinarySearchMidpointDivisor);

            if (isShared(mid))
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

    private static bool IsSharedByNaiveKeys(int[][] paths, int length)
    {
        var candidates = new HashSet<string>(NaiveWindowKeys(paths[0], length));

        for (var i = 1; i < paths.Length; i++)
        {
            candidates = IntersectNaiveKeys(candidates, paths[i], length);

            if (candidates.Count == 0)
            {
                return false;
            }
        }

        return candidates.Count > 0;
    }

    private static HashSet<string> IntersectNaiveKeys(HashSet<string> candidates, int[] path, int length)
    {
        var remaining = new HashSet<string>();

        foreach (var key in NaiveWindowKeys(path, length))
        {
            if (candidates.Contains(key))
            {
                remaining.Add(key);
            }
        }

        return remaining;
    }

    private static IEnumerable<string> NaiveWindowKeys(int[] path, int length)
    {
        for (var start = 0; start + length <= path.Length; start++)
        {
            var window = new ArraySegment<int>(path, start, length);

            yield return string.Join(CityIdSeparator, window);
        }
    }

    private static bool IsSharedByWindowHashes(char[][] encoded, int length)
    {
        var candidates = CollectWindowHashes(encoded[0], length);

        for (var i = 1; i < encoded.Length; i++)
        {
            var pathHashes = HashAllWindows(encoded[i], length);
            candidates = IntersectWindowHashes(candidates, pathHashes);

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

    private static Set<RollingHashValue> HashAllWindows(char[] text, int length)
    {
        var hashes = new Set<RollingHashValue>();
        var hash = new RollingHash(text);

        for (var start = 0; start + length <= text.Length; start++)
        {
            var value = hash.Hash(start, length);
            hashes.TryAdd(value);
        }

        return hashes;
    }

    private static List<RollingHashValue> IntersectWindowHashes(
        List<RollingHashValue> candidates,
        Set<RollingHashValue> pathHashes)
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

    // One shared code per distinct city id across every path, so two windows drawn
    // from two different paths encode to the same chars exactly when they visit the
    // same cities in the same order.
    private static HashMap<int, char> BuildCharCodes(int[][] paths)
    {
        var codes = new HashMap<int, char>();

        foreach (var path in paths)
        {
            AssignCodes(path, codes);
        }

        return codes;
    }

    private static void AssignCodes(int[] path, HashMap<int, char> codes)
    {
        foreach (var value in path)
        {
            if (!codes.HasKey(value))
            {
                codes.Set(value, (char)codes.Count);
            }
        }
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

    private static char[] Encode(int[] path, HashMap<int, char> codes)
    {
        var encoded = new char[path.Length];

        for (var i = 0; i < path.Length; i++)
        {
            codes.TryGetValue(path[i], out encoded[i]);
        }

        return encoded;
    }

    // No common subpath can be longer than the shortest path, so that is the binary
    // search's ceiling.
    private static int ShortestPathLength(int[][] paths)
    {
        var shortest = int.MaxValue;

        foreach (var path in paths)
        {
            shortest = Math.Min(shortest, path.Length);
        }

        return shortest;
    }

    private static int ShortestPathLength(char[][] paths)
    {
        var shortest = int.MaxValue;

        foreach (var path in paths)
        {
            shortest = Math.Min(shortest, path.Length);
        }

        return shortest;
    }
}
