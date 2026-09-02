namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 187 - a random DNA string (alphabet
// A/C/G/T) at a given length, so the fixed-window scan has real substrings to
// walk regardless of how many of them happen to repeat.
internal static class DnaSequenceWorkloads
{
    private const string Alphabet = "ACGT";

    public static string BuildSequence(int length, int seed)
    {
        var random = new Random(seed);
        var bases = new char[length];

        for (var i = 0; i < length; i++)
        {
            bases[i] = Alphabet[random.Next(Alphabet.Length)];
        }

        return new string(bases);
    }
}
