using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Vowels of All Substrings (LC 2063): SubstringScan is the textbook O(n^2) brute
// force - for every start index, extend the substring one character at a time and
// add its running vowel count. ContributionFormula instead sums each vowel's own
// (index+1)*(n-index) contribution (the count of substrings containing it) in a
// single O(n) pass - no repo primitive applies, the same "closed-form single pass
// over the bare string/array" category GasStation/Candy/MaximumProductSubarray
// already established in this repo.
[MemoryDiagnoser]
public class VowelsOfAllSubstringsBenchmarks
{
    private const int RandomSeed = 3;
    private const int LowercaseAlphabetSize = 26;

    [Params(200, 5_000)]
    public int Length;

    private string _word = null!;

    [GlobalSetup]
    public void Setup() => _word = BuildRandomLowercaseWord(Length);

    [Benchmark(Baseline = true)]
    public long SubstringScan()
    {
        long total = 0;

        for (var start = 0; start < _word.Length; start++)
        {
            var vowelsInRun = 0;

            for (var end = start; end < _word.Length; end++)
            {
                if (IsVowel(_word[end]))
                {
                    vowelsInRun++;
                }

                total += vowelsInRun;
            }
        }

        return total;
    }

    [Benchmark]
    public long ContributionFormula()
    {
        var length = _word.Length;
        long total = 0;

        for (var i = 0; i < length; i++)
        {
            if (IsVowel(_word[i]))
            {
                total += (long)(i + 1) * (length - i);
            }
        }

        return total;
    }

    private static bool IsVowel(char c) => c is 'a' or 'e' or 'i' or 'o' or 'u';

    private static string BuildRandomLowercaseWord(int length)
    {
        var random = new Random(RandomSeed);
        var chars = new char[length];

        for (var i = 0; i < length; i++)
        {
            chars[i] = (char)('a' + random.Next(0, LowercaseAlphabetSize));
        }

        return new string(chars);
    }
}
