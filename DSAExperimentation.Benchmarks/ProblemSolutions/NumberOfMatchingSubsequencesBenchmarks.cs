using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using WaitingQueue = DSAExperimentation.DataStructures.Queue.Queue<(string Word, int Index)>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Matching Subsequences (LC 792): the natural approach - a two-pointer
// subsequence scan through s, once per word - vs. this repo's HashMap<TKey,TValue>
// + Queue<T> bucket approach, which advances every word in a single left-to-right
// pass over s instead of re-scanning s once per word. s is drawn from a 25-letter
// alphabet and every word ends in the excluded 26th letter, so no word ever
// matches: TwoPointerPerWord is forced through its full O(WordCount * s.Length)
// worst case instead of short-circuiting on an early per-word match, which is
// exactly the case the bucket approach's O(s.Length + total word length) wins.
[MemoryDiagnoser]
public class NumberOfMatchingSubsequencesBenchmarks
{
    private const int TextLength = 20_000;
    private const char UnreachableChar = 'z';

    [Params(200, 2_000)]
    public int WordCount;

    private string _s = null!;
    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _s = GenerateText(random, TextLength);
        _words = Enumerable.Range(0, WordCount).Select(_ => GenerateText(random, 4) + UnreachableChar).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int TwoPointerPerWord()
    {
        var matches = 0;

        foreach (var word in _words)
        {
            if (IsSubsequence(word, _s))
            {
                matches++;
            }
        }

        return matches;
    }

    [Benchmark]
    public int HashMapQueueBuckets() => NumMatchingSubseq(_s, _words);

    private static bool IsSubsequence(string word, string s)
    {
        var w = 0;

        for (var i = 0; i < s.Length && w < word.Length; i++)
        {
            if (s[i] == word[w])
            {
                w++;
            }
        }

        return w == word.Length;
    }

    private static int NumMatchingSubseq(string s, string[] words)
    {
        var buckets = new HashMap<char, WaitingQueue>();

        foreach (var word in words)
        {
            Enqueue(buckets, word, 0);
        }

        var matches = 0;

        foreach (var c in s)
        {
            if (!buckets.TryGetValue(c, out var waiting))
            {
                continue;
            }

            var pending = waiting.Count;

            for (var i = 0; i < pending; i++)
            {
                waiting.TryDequeue(out var entry);
                var nextIndex = entry.Index + 1;

                if (nextIndex == entry.Word.Length)
                {
                    matches++;
                }
                else
                {
                    Enqueue(buckets, entry.Word, nextIndex);
                }
            }
        }

        return matches;
    }

    private static void Enqueue(HashMap<char, WaitingQueue> buckets, string word, int index)
    {
        var c = word[index];

        if (!buckets.TryGetValue(c, out var waiting))
        {
            waiting = new WaitingQueue();
            buckets.Set(c, waiting);
        }

        waiting.Enqueue((word, index));
    }

    private static string GenerateText(Random random, int length)
    {
        var chars = new char[length];

        for (var i = 0; i < length; i++)
        {
            chars[i] = (char)('a' + random.Next(25));
        }

        return new string(chars);
    }
}
