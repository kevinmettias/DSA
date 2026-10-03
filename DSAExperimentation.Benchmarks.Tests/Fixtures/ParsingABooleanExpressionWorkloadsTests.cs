using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ParsingABooleanExpressionWorkloads (ARCHITECTURE 17.7). The generator's
// comment promises a full binary operator tree of the asked depth that stays inside LC 1106's
// length bound and alphabet. A depth-d tree with no negation is exactly 5 * 2^d - 4 characters
// (2^d one-character leaves, 2^d - 1 "&(,)" or "|(,)" frames), and each "!(...)" adds 3, so the
// length pins the shape: at least the unnegated size, at most the all-negated 8 * 2^d - 7.
public sealed partial class ParsingABooleanExpressionWorkloadsTests
{
    private const int Seed = 1;
    private const int SmallestDepth = 8;
    private const int LargestDepth = 11;
    private const string Alphabet = "()&|!tf,";
    private const string LeafTokens = "tf";
    private const int FrameCharacters = 4;
    private const int NegationCharacters = 3;

    // The depths the benchmark runs.
    public static TheoryData<int> Depths => new() { SmallestDepth, LargestDepth };

    [Theory]
    [MemberData(nameof(Depths))]
    public void Build_Depth_StaysBetweenTheUnnegatedAndAllNegatedTreeSizes(int depth)
    {
        var leaves = 1 << depth;
        var unnegated = leaves + (FrameCharacters * (leaves - 1));

        Assert.InRange(Build(depth).Length, unnegated, unnegated + (NegationCharacters * (leaves - 1)));
    }

    [Theory]
    [MemberData(nameof(Depths))]
    public void Build_Depth_HasOneLeafPerSlotOfAFullTree(int depth) =>
        Assert.Equal(1 << depth, Build(depth).Count(LeafTokens.Contains));

    [Fact]
    public void Build_LargestDepth_StaysInsideLeetCodesLength() =>
        Assert.True(Build(LargestDepth).Length <= ParsingABooleanExpressionWorkloads.MaxLength);

    [Fact]
    public void Build_LargestDepth_UsesOnlyLeetCodesCharacters() =>
        Assert.All(Build(LargestDepth), character => Assert.Contains(character, Alphabet));

    [Fact]
    public void Build_LargestDepth_NegatesSomeNodes() =>
        Assert.Contains('!', Build(LargestDepth));

    private static string Build(int depth) => ParsingABooleanExpressionWorkloads.Build(depth, new Random(Seed));
}
