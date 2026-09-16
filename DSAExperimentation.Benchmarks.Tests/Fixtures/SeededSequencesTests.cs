using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for SeededSequences (ARCHITECTURE 17.7): the warm-up shape nine harnesses across
// six problems share, the integers 0..n-1 (or 1..n) in a seeded random order. The reading depends
// on the result being a genuine permutation - the harnesses index arrays with it, so a repeat or a
// gap would read the wrong element - and on the order actually being shuffled rather than the
// identity, since the tree harnesses measure how an insertion order degenerates.
public sealed partial class SeededSequencesTests
{
    private const int Count = 64;
    private const int Seed = 701;
    private const int FirstIndex = 0;
    private const int FirstNodeValue = 1;

    [Fact]
    public void ShuffledZeroTo_Count_ReturnsOneValuePerPosition() =>
        Assert.Equal(Count, SeededSequences.ShuffledZeroTo(Count, Seed).Length);

    [Fact]
    public void ShuffledZeroTo_Count_ReturnsEveryIndexFromZeroToCountMinusOneExactlyOnce() =>
        Assert.Equal(
            Enumerable.Range(FirstIndex, Count),
            SeededSequences.ShuffledZeroTo(Count, Seed).Order());

    [Fact]
    public void ShuffledZeroTo_Count_MovesValuesOffTheIdentityOrder() =>
        Assert.NotEqual(
            Enumerable.Range(FirstIndex, Count),
            SeededSequences.ShuffledZeroTo(Count, Seed));

    [Fact]
    public void ShuffledZeroTo_SameSeed_ReturnsTheSameOrder() =>
        Assert.Equal(
            SeededSequences.ShuffledZeroTo(Count, Seed),
            SeededSequences.ShuffledZeroTo(Count, Seed));

    // The documented reason the Random overloads exist: a harness that draws further values after
    // the shuffle has to keep ONE stream, which means this call has to consume draws from the
    // caller's Random rather than a private one of its own.
    [Fact]
    public void ShuffledZeroTo_SharedStream_ConsumesDrawsFromTheStreamItWasHanded()
    {
        var random = new Random(Seed);
        var first = SeededSequences.ShuffledZeroTo(Count, random);

        Assert.NotEqual(first, SeededSequences.ShuffledZeroTo(Count, random));
    }

    [Fact]
    public void ShuffledOneTo_Count_ReturnsOneValuePerPosition() =>
        Assert.Equal(Count, SeededSequences.ShuffledOneTo(Count, Seed).Length);

    [Fact]
    public void ShuffledOneTo_Count_ReturnsEveryNodeValueFromOneToCountExactlyOnce() =>
        Assert.Equal(
            Enumerable.Range(FirstNodeValue, Count),
            SeededSequences.ShuffledOneTo(Count, Seed).Order());

    [Fact]
    public void ShuffledOneTo_Count_MovesValuesOffTheIdentityOrder() =>
        Assert.NotEqual(
            Enumerable.Range(FirstNodeValue, Count),
            SeededSequences.ShuffledOneTo(Count, Seed));

    [Fact]
    public void ShuffledOneTo_SameSeed_ReturnsTheSameOrder() =>
        Assert.Equal(
            SeededSequences.ShuffledOneTo(Count, Seed),
            SeededSequences.ShuffledOneTo(Count, Seed));
}
