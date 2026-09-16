using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for DecodeStringWorkloads (ARCHITECTURE 17.7). What the reading depends on is
// that the requested length is actually reached and that the encoded groups nest only one level
// deep, which is what keeps the recursive-descent baseline's recursion depth constant as Length
// grows instead of overflowing the stack.
public sealed partial class DecodeStringWorkloadsTests
{
    private const int Length = 4_096;
    private const string EncodedTile = "2[ab]";
    private const int MaxNestingDepth = 1;

    [Fact]
    public void BuildEncoded_RequestedLength_ReturnsAtLeastThatManyCharacters() =>
        Assert.True(DecodeStringWorkloads.BuildEncoded(Length).Length >= Length);

    [Fact]
    public void BuildEncoded_EveryPrefix_KeepsNestingAtMostOneLevelDeep()
    {
        var depth = 0;

        foreach (var character in DecodeStringWorkloads.BuildEncoded(Length))
        {
            depth += NestingStep(character);
            Assert.InRange(depth, 0, MaxNestingDepth);
        }
    }

    [Fact]
    public void BuildEncoded_Text_IsTheRepeatedSingleLevelTile()
    {
        var encoded = DecodeStringWorkloads.BuildEncoded(Length);

        Assert.Equal(0, encoded.Length % EncodedTile.Length);
        Assert.All(Tiles(encoded), tile => Assert.Equal(EncodedTile, tile));
    }

    [Fact]
    public void BuildEncoded_SameLength_ReturnsTheSameExpression() =>
        Assert.Equal(DecodeStringWorkloads.BuildEncoded(Length), DecodeStringWorkloads.BuildEncoded(Length));

    private static IEnumerable<string> Tiles(string encoded) =>
        Enumerable.Range(0, encoded.Length / EncodedTile.Length)
            .Select(index => encoded.Substring(index * EncodedTile.Length, EncodedTile.Length));

    private static int NestingStep(char character) =>
        character switch
        {
            '[' => 1,
            ']' => -1,
            _ => 0,
        };
}
