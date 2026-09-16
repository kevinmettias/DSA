using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for DnaSequenceWorkloads (ARCHITECTURE 17.7): a sequence over the DNA
// alphabet at the requested length, so the fixed-window scan has real substrings to walk, and
// rebuilt identically from the same seed.
public sealed partial class DnaSequenceWorkloadsTests
{
    private const int Length = 256;
    private const int Seed = 187; // LC problem number
    private const string Alphabet = "ACGT";

    [Fact]
    public void BuildSequence_Length_ReturnsOneBasePerPosition() =>
        Assert.Equal(Length, DnaSequenceWorkloads.BuildSequence(Length, Seed).Length);

    [Fact]
    public void BuildSequence_EveryBase_ComesFromTheDnaAlphabet()
    {
        var sequence = DnaSequenceWorkloads.BuildSequence(Length, Seed);

        Assert.All(sequence, nucleotide => Assert.True(Alphabet.Contains(nucleotide)));
    }

    [Fact]
    public void BuildSequence_SameSeed_ReturnsTheSameSequence() =>
        Assert.Equal(
            DnaSequenceWorkloads.BuildSequence(Length, Seed),
            DnaSequenceWorkloads.BuildSequence(Length, Seed));
}
