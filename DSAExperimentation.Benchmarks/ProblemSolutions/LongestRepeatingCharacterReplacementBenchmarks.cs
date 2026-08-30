using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Repeating Character Replacement (LC 424): a per-start-index re-scan
// (recount from scratch at every start, extending right until more than k
// characters would need replacing) vs. the O(n) single sliding-window pass using
// this repo's own HashMap<char,int> for per-character counts, where the window's
// left edge only ever advances forward instead of restarting per start index.
// _text is a single repeated character so BruteForce's inner loop never breaks
// early (every window is trivially already-repeating), forcing its full O(n^2)
// worst case instead of bottoming out after a handful of characters.
[MemoryDiagnoser]
public class LongestRepeatingCharacterReplacementBenchmarks
{
    private const int K = 2;

    [Params(200, 5_000)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        _text = new string('A', Length);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var longest = 0;

        for (var start = 0; start < _text.Length; start++)
        {
            var counts = new int[26];
            var mostFrequentCount = 0;

            for (var end = start; end < _text.Length; end++)
            {
                var index = _text[end] - 'A';
                counts[index]++;
                mostFrequentCount = Math.Max(mostFrequentCount, counts[index]);

                if (end - start + 1 - mostFrequentCount > K)
                {
                    break;
                }

                longest = Math.Max(longest, end - start + 1);
            }
        }

        return longest;
    }

    [Benchmark]
    public int SlidingWindowHashMap()
    {
        var counts = new HashMap<char, int>();
        var windowStart = 0;
        var mostFrequentCount = 0;
        var longest = 0;

        for (var windowEnd = 0; windowEnd < _text.Length; windowEnd++)
        {
            var incoming = _text[windowEnd];
            counts.TryGetValue(incoming, out var incomingCount);
            counts.Set(incoming, incomingCount + 1);
            mostFrequentCount = Math.Max(mostFrequentCount, incomingCount + 1);

            if (windowEnd - windowStart + 1 - mostFrequentCount > K)
            {
                var outgoing = _text[windowStart];
                counts.TryGetValue(outgoing, out var outgoingCount);
                counts.Set(outgoing, outgoingCount - 1);
                windowStart++;
            }

            longest = Math.Max(longest, windowEnd - windowStart + 1);
        }

        return longest;
    }
}
