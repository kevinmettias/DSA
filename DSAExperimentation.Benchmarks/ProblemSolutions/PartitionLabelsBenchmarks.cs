using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Partition Labels (LC 763): the naive approach re-derives each letter's last
// occurrence with a fresh backward scan every time a partition's frontier expands
// (O(n) per lookup, O(n^2) worst case) vs. this repo's own HashMap<char,int>
// recording every letter's last occurrence in one O(n) pass up front (TwoSumBenchmarks'
// exact shape). Letters are random across the whole alphabet so partitions stay
// small and the naive scan is forced to search most of the string, over and over,
// instead of an early exit.
[MemoryDiagnoser]
public class PartitionLabelsBenchmarks
{
    // LC problem number, reused as the deterministic PRNG seed.
    private const int RandomSeed = 763;
    private const int AlphabetSize = 26;

    [Params(200, 5_000)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(AlphabetSize));
        }

        _text = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public List<int> BruteForceRescan() => PartitionBruteForce(_text);

    [Benchmark]
    public List<int> HashMapOnePass() => PartitionIntoLabelSizes(_text);

    private static List<int> PartitionBruteForce(string s)
    {
        var sizes = new List<int>();
        var start = 0;

        while (start < s.Length)
        {
            start = ExtendPartitionFromStart(s, start, sizes);
        }

        return sizes;
    }

    private static int ExtendPartitionFromStart(string s, int start, List<int> sizes)
    {
        var end = start;
        var i = start;

        while (i <= end)
        {
            var last = LastIndexOf(s, s[i]);

            if (last > end)
            {
                end = last;
            }

            i++;
        }

        sizes.Add(end - start + 1);
        return end + 1;
    }

    private static int LastIndexOf(string s, char target)
    {
        for (var j = s.Length - 1; j >= 0; j--)
        {
            if (s[j] == target)
            {
                return j;
            }
        }

        return -1;
    }

    private static List<int> PartitionIntoLabelSizes(string s)
    {
        var lastIndex = new HashMap<char, int>();

        for (var i = 0; i < s.Length; i++)
        {
            lastIndex.Set(s[i], i);
        }

        var sizes = new List<int>();
        var start = 0;
        var end = 0;

        for (var i = 0; i < s.Length; i++)
        {
            lastIndex.TryGetValue(s[i], out var furthest);
            end = Math.Max(end, furthest);
            start = ClosePartitionIfComplete(end, i, start, sizes);
        }

        return sizes;
    }

    private static int ClosePartitionIfComplete(int end, int i, int start, List<int> sizes)
    {
        if (i != end)
        {
            return start;
        }

        sizes.Add(end - start + 1);
        return i + 1;
    }
}
