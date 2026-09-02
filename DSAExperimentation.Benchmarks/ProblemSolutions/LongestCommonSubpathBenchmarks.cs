using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.RollingHash;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Common Subpath (LC 1923): the naive baseline screens every candidate
// window with a real O(length) key (string.Join over the raw ints, built fresh per
// window) against this repo's own RollingHash - city ids first coordinate-compressed
// into a shared char alphabet via this repo's own HashMap<int,char> (the same
// compression LongestCommonSubpathTests.cs uses, since RollingHash itself only
// operates over ReadOnlySpan<char>) - giving an O(1) window key via
// Hash(start,length) instead. Both binary-search the common length upward and
// intersect one path's window-key set into the next using this repo's own Set<T>
// (HashMap-backed), so the only thing that differs is the O(length) vs O(1)
// per-window key cost - the same asymmetry DistinctEchoSubstringsBenchmarks already
// demonstrates for a single string, here applied across n paths at once.
[MemoryDiagnoser]
public class LongestCommonSubpathBenchmarks
{
    private const int PathCount = 3;
    private const int AlphabetSize = 5;
    private const int RandomSeed = 1923; // LC 1923 problem number
    private const int BinarySearchMidpointDivisor = 2;

    [Params(100, 500)]
    public int PathLength;

    private int[][] _paths = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _paths = new int[PathCount][];

        for (var i = 0; i < PathCount; i++)
        {
            var path = new int[PathLength];

            for (var j = 0; j < PathLength; j++)
            {
                path[j] = random.Next(1, AlphabetSize + 1);
            }

            _paths[i] = path;
        }
    }

    [Benchmark(Baseline = true)]
    public int NaiveKeyedIntersection()
    {
        return BinarySearchLongestCommonLength(MinLength(_paths), HasCommonSubpathNaive);
    }

    private static int BinarySearchLongestCommonLength(int maxLength, Func<int, bool> hasCommonSubpath)
    {
        var low = 0;
        var high = maxLength;

        while (low < high)
        {
            var mid = low + ((high - low + 1) / BinarySearchMidpointDivisor);

            if (hasCommonSubpath(mid))
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

    private bool HasCommonSubpathNaive(int length)
    {
        var candidates = new HashSet<string>(NaiveWindowKeys(_paths[0], length));

        for (var i = 1; i < _paths.Length; i++)
        {
            var next = IntersectWindowKeys(candidates, _paths[i], length);

            if (next is null)
            {
                return false;
            }

            candidates = next;
        }

        return true;
    }

    private static HashSet<string>? IntersectWindowKeys(HashSet<string> candidates, int[] path, int length)
    {
        var current = new HashSet<string>();

        foreach (var window in NaiveWindowKeys(path, length))
        {
            if (candidates.Contains(window))
            {
                current.Add(window);
            }
        }

        return current.Count == 0 ? null : current;
    }

    private static IEnumerable<string> NaiveWindowKeys(int[] path, int length)
    {
        for (var start = 0; start + length <= path.Length; start++)
        {
            yield return string.Join(',', new ArraySegment<int>(path, start, length));
        }
    }

    [Benchmark]
    public int RollingHashBinarySearch()
    {
        var codes = BuildCharacterCodes(_paths);
        var encoded = EncodeAllPaths(_paths, codes);

        return BinarySearchLongestCommonLength(MinLength(encoded), length => HasCommonSubpathHashed(encoded, length));
    }

    private static HashMap<int, char> BuildCharacterCodes(int[][] paths)
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

    private static char[][] EncodeAllPaths(int[][] paths, HashMap<int, char> codes)
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

    private static bool HasCommonSubpathHashed(char[][] encoded, int length)
    {
        var candidates = CollectWindowHashes(encoded[0], length);

        for (var i = 1; i < encoded.Length; i++)
        {
            var next = IntersectWindowHashes(candidates, encoded[i], length);

            if (next is null)
            {
                return false;
            }

            candidates = next;
        }

        return candidates.Count > 0;
    }

    private static List<RollingHashValue>? IntersectWindowHashes(List<RollingHashValue> candidates, char[] path, int length)
    {
        var pathHashes = new Set<RollingHashValue>();
        var hash = new RollingHash(path);

        for (var start = 0; start + length <= path.Length; start++)
        {
            var windowHash = hash.Hash(start, length);
            pathHashes.TryAdd(windowHash);
        }

        var next = new List<RollingHashValue>();

        foreach (var candidate in candidates)
        {
            if (pathHashes.Has(candidate))
            {
                next.Add(candidate);
            }
        }

        return next.Count == 0 ? null : next;
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

    private static int MinLength(int[][] paths)
    {
        var min = int.MaxValue;

        foreach (var path in paths)
        {
            min = Math.Min(min, path.Length);
        }

        return min;
    }

    private static int MinLength(char[][] paths)
    {
        var min = int.MaxValue;

        foreach (var path in paths)
        {
            min = Math.Min(min, path.Length);
        }

        return min;
    }
}
