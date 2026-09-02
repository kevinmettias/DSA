using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// First Unique Character in a String (LC 387): the classic O(n^2) brute force
// (for every index, rescan the WHOLE string counting occurrences - no early break on
// the first duplicate found, or the 26-letter alphabet's pigeonhole would let most
// inner scans terminate almost immediately and hide the real complexity gap) vs. the
// O(n) two-pass HashMap<char,int> frequency count. Length is kept well past 26 so no
// character ever occurs exactly once, forcing BOTH strategies through their full
// worst-case scan - the same "force the real worst case" convention
// TwoSumBenchmarks/LongestSubstringWithoutRepeatingCharactersBenchmarks already use.
[MemoryDiagnoser]
public class FirstUniqueCharacterInAStringBenchmarks
{
    private const int AlphabetSize = 26;

    [Params(500, 5_000)]
    public int Length;

    private string _value = string.Empty;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(0, AlphabetSize));
        }

        _value = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        for (var i = 0; i < _value.Length; i++)
        {
            var occurrences = 0;

            for (var j = 0; j < _value.Length; j++)
            {
                if (_value[j] == _value[i])
                {
                    occurrences++;
                }
            }

            if (occurrences == 1)
            {
                return i;
            }
        }

        return -1;
    }

    [Benchmark]
    public int HashMapTwoPass()
    {
        var counts = new HashMap<char, int>();

        foreach (var c in _value)
        {
            counts.TryGetValue(c, out var count);
            counts.Set(c, count + 1);
        }

        for (var i = 0; i < _value.Length; i++)
        {
            counts.TryGetValue(_value[i], out var count);

            if (count == 1)
            {
                return i;
            }
        }

        return -1;
    }
}
