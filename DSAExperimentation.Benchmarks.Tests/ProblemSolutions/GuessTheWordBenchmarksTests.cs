using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GuessTheWordBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already holds
// the arms to agreeing on the smallest workload, so this asserts only the secret derived here from
// [GlobalSetup]'s own documented draw sequence - one Random seeded with the problem number, six
// lowercase letters per candidate, the first WordCount distinct words kept in draw order and the
// secret at index WordCount / 2 - rather than read back out of either arm, so the assertion says
// the search found the hidden word and not merely that the two arms chose the same wrong one.
public sealed partial class GuessTheWordBenchmarksTests
{
    private const int SmallestWordCount = 10;
    private const int RandomSeed = 843;
    private const int WordLength = 6;
    private const int MiddleIndexDivisor = 2;
    private const int AlphabetSize = 26;

    [Fact]
    public void InPlaceListRemoval_SeededWordPool_FindsTheHiddenWord() =>
        Assert.Equal(ExpectedSecretWord(), BuildHarness().InPlaceListRemoval());

    // [GlobalSetup]'s own draw sequence, restated: duplicates redrawn until the pool holds
    // SmallestWordCount distinct words, and the secret taken from the middle of that pool.
    private static string ExpectedSecretWord()
    {
        var random = new Random(RandomSeed);
        var seen = new HashSet<string>();
        var words = new List<string>();

        while (words.Count < SmallestWordCount)
        {
            var word = RandomWord(random);

            if (seen.Add(word))
            {
                words.Add(word);
            }
        }

        return words[SmallestWordCount / MiddleIndexDivisor];
    }

    private static string RandomWord(Random random)
    {
        var chars = new char[WordLength];

        for (var i = 0; i < WordLength; i++)
        {
            chars[i] = (char)('a' + random.Next(AlphabetSize));
        }

        return new string(chars);
    }

    private static GuessTheWordBenchmarks BuildHarness()
    {
        var harness = new GuessTheWordBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
