using System.Text;

namespace DSAExperimentation.Tests.DataStructures.SubstringIndex.Fixtures;

// The texts every substring index is held to, by name, so a failing case reads as the text it
// failed on. The adversarial ones each press on a place an index tends to break: no text at all,
// one letter repeated (the deepest tree, the most overlap), a run broken at either end (an
// occurrence that ends at the text's end), short periods with and without a break, the Fibonacci
// and Thue-Morse words (many repeats, and none of a cube), all-distinct letters (no repeat at all),
// a repeat at both ends that does not overlap, and letters that differ only in case. The random
// ones draw from two to four letters, so repeats are common and long.
internal static class SubstringIndexTexts
{
    private const int RunLength = 24;
    private const int FibonacciWordLength = 55;
    private const int ThueMorseLength = 32;
    private const int SmallestRandomLength = 40;
    private const int RandomLengthStep = 7;
    private const int SeedsPerAlphabet = 3;
    private const int SmallestRandomAlphabet = 2;
    private const int LargestRandomAlphabet = 4;
    private const char FirstLetter = 'a';
    private const char SecondLetter = 'b';

    private static readonly Dictionary<string, string> TextsByName = BuildTexts();

    /// <summary>Gets every text's name, in a fixed order.</summary>
    public static IReadOnlyList<string> Names { get; } = [.. TextsByName.Keys];

    /// <summary>Finds a text by the name it is listed under.</summary>
    /// <param name="name">One of <see cref="Names"/>.</param>
    /// <returns>The text.</returns>
    public static string Named(string name) => TextsByName[name];

    /// <summary>Pairs each of several structures with every text, one theory case a pair.</summary>
    /// <param name="structures">The names a test builds its structures by.</param>
    /// <returns>A case for each structure and text.</returns>
    public static TheoryData<string, string> EveryTextFor(IEnumerable<string> structures)
    {
        var cases = new TheoryData<string, string>();

        foreach (var structure in structures)
        {
            foreach (var name in Names)
            {
                cases.Add(structure, name);
            }
        }

        return cases;
    }

    private static Dictionary<string, string> BuildTexts()
    {
        var texts = AdversarialTexts();
        AddRandomTexts(texts);
        return texts;
    }

    private static Dictionary<string, string> AdversarialTexts() =>
        new(StringComparer.Ordinal)
        {
            ["empty"] = string.Empty,
            ["one letter"] = "a",
            ["one letter run"] = new string(FirstLetter, RunLength),
            ["run broken at the end"] = new string(FirstLetter, RunLength) + SecondLetter,
            ["run broken at the start"] = SecondLetter + new string(FirstLetter, RunLength),
            ["period two"] = "abababababababab",
            ["period three broken"] = "abcabcabcabcabd",
            ["fibonacci word"] = FibonacciWord(FibonacciWordLength),
            ["thue-morse word"] = ThueMorseWord(ThueMorseLength),
            ["all distinct"] = "zyxwvutsrqponm",
            ["repeat at both ends"] = "abcdefgabc",
            ["case differs"] = "aAaAAaaAa",
            ["banana"] = "banana",
            ["mississippi"] = "mississippi",
            ["split edge"] = "abcabd",
        };

    private static void AddRandomTexts(Dictionary<string, string> texts)
    {
        for (var alphabet = SmallestRandomAlphabet; alphabet <= LargestRandomAlphabet; alphabet++)
        {
            for (var seed = 0; seed < SeedsPerAlphabet; seed++)
            {
                var length = SmallestRandomLength + (RandomLengthStep * seed);
                var name = $"random {alphabet} letters seed {seed}";
                texts[name] = RandomText(alphabet, length, seed);
            }
        }
    }

    // Each word is the previous followed by the one before it: a, ab, aba, abaab, ...
    private static string FibonacciWord(int length)
    {
        var previous = "a";
        var current = "ab";

        while (current.Length < length)
        {
            (previous, current) = (current, current + previous);
        }

        return current[..length];
    }

    // Letter i is 'b' when i has an odd number of set bits.
    private static string ThueMorseWord(int length)
    {
        var word = new StringBuilder(length);

        for (var i = 0; i < length; i++)
        {
            var hasOddBits = int.PopCount(i) % 2 == 1;
            word.Append(hasOddBits ? SecondLetter : FirstLetter);
        }

        return word.ToString();
    }

    private static string RandomText(int alphabet, int length, int seed)
    {
        var random = new Random(seed);
        var letters = new char[length];

        for (var i = 0; i < length; i++)
        {
            letters[i] = (char)(FirstLetter + random.Next(alphabet));
        }

        return new string(letters);
    }
}
