using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for InterleavingStringWorkloads (ARCHITECTURE 17.7). LC 97's benchmark pins both
// arms to true, which rests on the target being an interleaving of the two sources. The fixture
// hands back the interleaving it used, so that claim is checked here by following it: taking each
// target letter from the source it names, in order, must spell the target and use up both sources
// exactly. The sizes are LeetCode's caps - 100 letters per source, 200 in the target.
public sealed partial class InterleavingStringWorkloadsTests
{
    private const int SourceLength = 100;
    private const int Seed = 97; // LC problem number
    private const string Alphabet = "ab";

    [Fact]
    public void Build_SourceLength_ReturnsTwoSourcesOfThatLengthOverTheNarrowAlphabet()
    {
        var (first, second, target, _) = Build();

        Assert.Equal(SourceLength, first.Length);
        Assert.Equal(SourceLength, second.Length);
        Assert.Equal(first.Length + second.Length, target.Length);
        Assert.All(first + second + target, letter => Assert.Contains(letter, Alphabet));
    }

    [Fact]
    public void Build_FromFirst_SpellsTheTargetFromBothSourcesInOrder()
    {
        var (first, second, target, fromFirst) = Build();
        var (nextFirst, nextSecond) = (0, 0);

        Assert.Equal(target.Length, fromFirst.Length);

        for (var i = 0; i < target.Length; i++)
        {
            if (fromFirst[i])
            {
                Assert.Equal(first[nextFirst++], target[i]);
            }
            else
            {
                Assert.Equal(second[nextSecond++], target[i]);
            }
        }

        Assert.Equal((first.Length, second.Length), (nextFirst, nextSecond));
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameStrings()
    {
        var (first, second, target, _) = Build();

        Assert.Equal((first, second, target), (Build().First, Build().Second, Build().Target));
    }

    private static (string First, string Second, string Target, bool[] FromFirst) Build() =>
        InterleavingStringWorkloads.Build(SourceLength, new Random(Seed));
}
