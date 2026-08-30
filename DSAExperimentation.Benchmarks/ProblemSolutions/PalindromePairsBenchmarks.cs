using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Palindrome Pairs (LC 336): the O(n^2*k) brute force that concatenates and checks
// every ordered word pair directly vs. the O(n*k^2) approach using this repo's own
// HashMap<TKey,TValue> as a reversed-complement lookup for every prefix/suffix split
// - the same complement-lookup shape TwoSumBenchmarks already demonstrates, applied
// to strings.
[MemoryDiagnoser]
public class PalindromePairsBenchmarks
{
    [Params(80, 400)]
    public int WordCount;

    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var unique = new HashSet<string>();

        while (unique.Count < WordCount)
        {
            var length = random.Next(1, 9);
            var chars = new char[length];

            for (var i = 0; i < length; i++)
            {
                chars[i] = (char)('a' + random.Next(3));
            }

            unique.Add(new string(chars));
        }

        _words = [.. unique];
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var count = 0;

        for (var i = 0; i < _words.Length; i++)
        {
            for (var j = 0; j < _words.Length; j++)
            {
                if (i != j && IsPalindrome(_words[i] + _words[j]))
                {
                    count++;
                }
            }
        }

        return count;
    }

    [Benchmark]
    public int HashMapComplementLookup()
    {
        var indexOf = new HashMap<string, int>();
        for (var i = 0; i < _words.Length; i++)
        {
            indexOf.Set(_words[i], i);
        }

        var count = 0;
        for (var i = 0; i < _words.Length; i++)
        {
            var word = _words[i];
            for (var cut = 0; cut <= word.Length; cut++)
            {
                var prefix = word[..cut];
                var suffix = word[cut..];

                if (IsPalindrome(prefix)
                    && indexOf.TryGetValue(Reverse(suffix), out var suffixMatch)
                    && suffixMatch != i)
                {
                    count++;
                }

                if (cut != word.Length
                    && IsPalindrome(suffix)
                    && indexOf.TryGetValue(Reverse(prefix), out var prefixMatch)
                    && prefixMatch != i)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool IsPalindrome(string s)
    {
        var left = 0;
        var right = s.Length - 1;
        while (left < right)
        {
            if (s[left++] != s[right--])
            {
                return false;
            }
        }

        return true;
    }

    private static string Reverse(string s)
    {
        var chars = s.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }
}
