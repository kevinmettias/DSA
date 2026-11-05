using System.Collections.Immutable;
using DSAExperimentation.Domain.Language.English;
using DSAExperimentation.Domain.Language.English.Orthography;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 567 - a random haystack drawn from the consonants, the lowercase
// letters that are not vowels, so it holds no letter of the benchmark's fixed pattern, the vowels,
// and neither strategy ever finds a match: both are forced through their full worst-case scan.
internal static class PermutationInStringWorkloads
{
    private static readonly ImmutableArray<char> Consonants =
        [.. EnglishLowercase.Letters.Where(letter => !EnglishVowels.Has(letter))];

    public static string BuildHaystack(int length, int seed)
    {
        var random = new Random(seed);
        var chars = new char[length];

        for (var i = 0; i < length; i++)
        {
            chars[i] = Consonants[random.Next(Consonants.Length)];
        }

        return new string(chars);
    }
}
