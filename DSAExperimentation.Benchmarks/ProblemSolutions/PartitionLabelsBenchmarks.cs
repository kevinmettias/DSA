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
    [Params(200, 5_000)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(763);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(26));
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
            start = end + 1;
        }

        return sizes;
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

            if (i != end)
            {
                continue;
            }

            sizes.Add(end - start + 1);
            start = i + 1;
        }

        return sizes;
    }
}
