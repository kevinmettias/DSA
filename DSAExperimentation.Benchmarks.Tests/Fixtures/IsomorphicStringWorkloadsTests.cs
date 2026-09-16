using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for IsomorphicStringWorkloads (ARCHITECTURE 17.7). The reading depends on LC 205's
// pair being genuinely isomorphic, so the checker never short-circuits on an early mismatch and both
// strategies scan the full requested length.
public sealed partial class IsomorphicStringWorkloadsTests
{
    private const int Length = 256;
    private const int Seed = 205; // LC problem number
    private const char FirstLetter = 'a';
    private const char LastLetter = 'z';

    [Fact]
    public void BuildIsomorphicPair_Length_ReturnsTwoStringsOfThatLength()
    {
        var (source, target) = IsomorphicStringWorkloads.BuildIsomorphicPair(Length, Seed);

        Assert.Equal(Length, source.Length);
        Assert.Equal(Length, target.Length);
    }

    [Fact]
    public void BuildIsomorphicPair_EveryCharacter_StaysOnTheLowercaseAlphabet()
    {
        var (source, target) = IsomorphicStringWorkloads.BuildIsomorphicPair(Length, Seed);

        Assert.All(source, character => Assert.InRange(character, FirstLetter, LastLetter));
        Assert.All(target, character => Assert.InRange(character, FirstLetter, LastLetter));
    }

    // A bijection between the two strings' characters is exactly what "isomorphic" means, and it is
    // the property the workload's guarantee rests on: one source letter can never map to two
    // different target letters, or the pair would fail the very check the strategies are measuring.
    [Fact]
    public void BuildIsomorphicPair_ThePair_AdmitsABijectionBetweenItsCharacters()
    {
        var (source, target) = IsomorphicStringWorkloads.BuildIsomorphicPair(Length, Seed);
        var sourceToTarget = new Dictionary<char, char>();
        var targetToSource = new Dictionary<char, char>();

        foreach (var position in Enumerable.Range(0, Length))
        {
            Assert.True(
                MappedConsistently(sourceToTarget, source[position], target[position])
                && MappedConsistently(targetToSource, target[position], source[position]));
        }
    }

    [Fact]
    public void BuildIsomorphicPair_SameSeed_ReturnsTheSamePair()
    {
        var (source, target) = IsomorphicStringWorkloads.BuildIsomorphicPair(Length, Seed);
        var (repeatSource, repeatTarget) = IsomorphicStringWorkloads.BuildIsomorphicPair(Length, Seed);

        Assert.Equal(source, repeatSource);
        Assert.Equal(target, repeatTarget);
    }

    private static bool MappedConsistently(Dictionary<char, char> mapping, char from, char to)
    {
        if (mapping.TryGetValue(from, out var mapped))
        {
            return mapped == to;
        }

        mapping.Add(from, to);

        return true;
    }
}
