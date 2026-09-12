namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 567 - a random haystack drawn from an
// alphabet that excludes every letter of the benchmark's fixed pattern
// "aeiou", so neither strategy ever finds a match and both are forced through
// their full worst-case scan.
internal static class PermutationInStringWorkloads
{
    private const string Alphabet = "bcdfghjklmnpqrstvwxyz";

    public static string BuildHaystack(int length, int seed)
    {
        var random = new Random(seed);
        var chars = new char[length];

        for (var i = 0; i < length; i++)
        {
            chars[i] = Alphabet[random.Next(Alphabet.Length)];
        }

        return new string(chars);
    }
}
