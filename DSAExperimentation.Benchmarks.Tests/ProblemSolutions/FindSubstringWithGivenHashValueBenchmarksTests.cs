using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindSubstringWithGivenHashValueBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - recomputing each window's hash from its characters
// against reading it off this repo's precomputed RollingHash table - so a harness whose arms disagree
// has searched for two different hash values. Both answers are one bool, so they are compared
// directly.
//
// LC 2156 guarantees an answer exists, so Setup plants the target as the hash of the text's last
// window, and both arms find it. A bool alone cannot show that they swept every window to get there,
// so that claim - no earlier window shares the planted hash - is checked here on its own, by
// recomputing every window's hash over the same seeded text.
public sealed partial class FindSubstringWithGivenHashValueBenchmarksTests
{
    // The smaller of Setup's [Params(500, 20_000)] text lengths.
    private const int SmallestLength = 500;
    private const int LargestLength = 20_000;

    // Mirrors the benchmark's own text and hash: its seed, alphabet, window length, power and LC
    // 2156's largest modulus.
    private const int RandomSeed = 2156;
    private const int AlphabetSize = 26;
    private const int WindowLength = 20;
    private const int Power = 7;
    private const int Modulo = 1_000_000_000;

    // LC 2156 guarantees a window carries the requested hash.
    private const bool ExpectedMatch = true;

    [Fact]
    public void Setup_SameLength_RebuildsThePlantedSearch() =>
        Assert.Equal(
            BuildHarness().TryFindSubstringByWindowRehash(),
            BuildHarness().TryFindSubstringByWindowRehash());

    [Theory]
    [InlineData(SmallestLength)]
    [InlineData(LargestLength)]
    public void Setup_PlantedHash_IsCarriedFirstByTheLastWindow(int length)
    {
        var text = SeededText(length);
        var hashes = Enumerable.Range(0, length - WindowLength + 1).Select(start => WindowHash(text, start)).ToArray();

        Assert.Equal(hashes.Length - 1, Array.IndexOf(hashes, hashes[^1]));
    }

    [Fact]
    public void TryFindSubstringByWindowRehash_PlantedHashValue_AgreesWithTryFindSubstringByRollingHash()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMatch, harness.TryFindSubstringByWindowRehash());
        Assert.Equal(
            harness.TryFindSubstringByRollingHash(),
            harness.TryFindSubstringByWindowRehash());
    }

    [Fact]
    public void TryFindSubstringByRollingHash_PlantedHashValue_AgreesWithTryFindSubstringByWindowRehash()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMatch, harness.TryFindSubstringByRollingHash());
        Assert.Equal(
            harness.TryFindSubstringByWindowRehash(),
            harness.TryFindSubstringByRollingHash());
    }

    private static FindSubstringWithGivenHashValueBenchmarks BuildHarness()
    {
        var harness = new FindSubstringWithGivenHashValueBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    private static string SeededText(int length)
    {
        var random = new Random(RandomSeed);
        var letters = new char[length];

        for (var i = 0; i < length; i++)
        {
            letters[i] = (char)('a' + random.Next(AlphabetSize));
        }

        return new string(letters);
    }

    // LC 2156's own hash, written out from its statement: the window's first letter on power^0, each
    // letter valued by its alphabet index plus one.
    private static long WindowHash(string text, int start)
    {
        var hash = 0L;
        var powerTerm = 1L;

        foreach (var letter in text.AsSpan(start, WindowLength))
        {
            var letterValue = letter - 'a' + 1;
            hash = (hash + (letterValue * powerTerm)) % Modulo;
            powerTerm = powerTerm * Power % Modulo;
        }

        return hash;
    }
}
