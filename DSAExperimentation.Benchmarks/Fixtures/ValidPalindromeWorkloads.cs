namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 125: a string of printable ASCII that is a
// palindrome once its letters and digits are read alone and lowercased, the shape
// of LeetCode's own "A man, a plan, a canal: Panama". The letters and digits are
// a seeded half mirrored onto itself, each letter cased at random on its own, and
// punctuation and spaces are scattered between them at seeded positions, so the
// answer is true and neither strategy can stop before the middle.
internal static class ValidPalindromeWorkloads
{
    private const string Alphanumerics = "abcdefghijklmnopqrstuvwxyz0123456789";
    private const string Noise = " ,.:;!?'-";

    // One character in this many is punctuation or a space.
    private const int NoiseInterval = 4;

    private const int Halves = 2;

    // A letter is cased up when a draw over this many choices comes up zero.
    private const int CaseChoices = 2;

    public static string BuildNoisyPalindrome(int length, Random random)
    {
        var noiseCount = length / NoiseInterval;
        var core = MirroredCore(length - noiseCount, random);
        var noisePositions = SeededSequences.ShuffledZeroTo(length, random).Take(noiseCount).ToHashSet();
        var text = new char[length];
        var nextCore = 0;

        for (var i = 0; i < length; i++)
        {
            text[i] = noisePositions.Contains(i) ? Noise[random.Next(Noise.Length)] : core[nextCore++];
        }

        return new string(text);
    }

    // The first half is drawn and the second half is its mirror; an odd length keeps its middle
    // character once. Each letter is then cased independently, which the lowercased reading ignores.
    private static char[] MirroredCore(int length, Random random)
    {
        var core = new char[length];

        for (var i = 0; i < (length + 1) / Halves; i++)
        {
            core[i] = Alphanumerics[random.Next(Alphanumerics.Length)];
            core[length - 1 - i] = core[i];
        }

        for (var i = 0; i < length; i++)
        {
            core[i] = random.Next(CaseChoices) == 0 ? char.ToUpperInvariant(core[i]) : core[i];
        }

        return core;
    }
}
