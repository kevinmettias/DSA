using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for CircularArrayWorkloads (ARCHITECTURE 17.7): a ring of filler words with
// the target at exactly one index - the one diametrically opposite the scenario's start index -
// which is the worst case that forces both search directions through real work.
public sealed partial class CircularArrayWorkloadsTests
{
    private const int Length = 32;
    private const int MiddlePositionDivisor = 2; // the target sits at Length / 2, opposite start index 0
    private const string Filler = "filler-word";

    [Fact]
    public void BuildWords_Length_ReturnsOneWordPerPosition() =>
        Assert.Equal(Length, CircularArrayWorkloads.BuildWords(Length).Length);

    [Fact]
    public void BuildWords_MiddlePosition_HoldsTheOnlyTargetWord()
    {
        var words = CircularArrayWorkloads.BuildWords(Length);

        Assert.Equal(CircularArrayScenario.Target, words[Length / MiddlePositionDivisor]);
        Assert.Single(words, word => word == CircularArrayScenario.Target);
    }

    [Fact]
    public void BuildWords_EveryOtherPosition_HoldsTheFillerWord()
    {
        var words = CircularArrayWorkloads.BuildWords(Length);

        Assert.All(
            Enumerable.Range(0, Length).Where(index => index != Length / MiddlePositionDivisor),
            index => Assert.Equal(Filler, words[index]));
    }

    [Fact]
    public void BuildWords_SameLength_ReturnsTheSameWords() =>
        Assert.Equal(CircularArrayWorkloads.BuildWords(Length), CircularArrayWorkloads.BuildWords(Length));
}
