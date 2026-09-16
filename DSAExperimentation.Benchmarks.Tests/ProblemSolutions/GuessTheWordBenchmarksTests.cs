using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GuessTheWordBenchmarks (ARCHITECTURE 17.9): both arms are
// GuessTheWordSolution's - in-place removal from a BCL List against this repo's DynamicArray pool
// rebuilt each round - so a harness whose arms disagree is timing two different searches. Both arms
// take candidate 0 every round, so they converge on the identical secret in the identical number of
// rounds, and both answer with the secret word itself. The expected secret is derived here from
// [GlobalSetup]'s own documented draw sequence - one Random seeded with the problem number, six
// lowercase letters per candidate, the first WordCount distinct words kept in draw order and the
// secret at index WordCount / 2 - rather than read back out of either arm, so the assertion says
// the search found the hidden word and not merely that the two arms chose the same wrong one.
// Neither arm mutates the pool (the in-place arm copies the array into a List first), so one
// harness is safe to call twice in either order.
public sealed partial class GuessTheWordBenchmarksTests
{
    private const int SmallestWordCount = 100;
    private const int RandomSeed = 843;
    private const int WordLength = 6;
    private const int MiddleIndexDivisor = 2;
    private const int AlphabetSize = 26;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSamePool() =>
        Assert.Equal(BuildHarness().InPlaceListRemoval(), BuildHarness().InPlaceListRemoval());

    [Fact]
    public void InPlaceListRemoval_SeededWordPool_AgreesWithShrinkingCandidatePool()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedSecretWord(), harness.InPlaceListRemoval());
        Assert.Equal(harness.ShrinkingCandidatePool(), harness.InPlaceListRemoval());
    }

    [Fact]
    public void ShrinkingCandidatePool_SeededWordPool_AgreesWithInPlaceListRemoval()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedSecretWord(), harness.ShrinkingCandidatePool());
        Assert.Equal(harness.InPlaceListRemoval(), harness.ShrinkingCandidatePool());
    }

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
