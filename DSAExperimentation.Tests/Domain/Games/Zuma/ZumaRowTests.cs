using DSAExperimentation.Domain.Games.Zuma;

namespace DSAExperimentation.Tests.Domain.Games.Zuma;

public sealed partial class ZumaRowTests
{
    public static TheoryData<string, string> ClassicRows => new()
    {
        // Nothing to collapse: an empty row, pairs, and balls of one colour kept apart.
        { "", "" },
        { "RRBB", "RRBB" },
        { "RBR", "RBR" },

        // Three or more of a colour vanish, however many there are.
        { "RRR", "" },
        { "RRRRR", "" },

        // The balls either side then touch: two stay, and three vanish in turn.
        { "WRRRW", "WW" },
        { "BWWRRRWB", "BB" },
    };

    [Theory]
    [MemberData(nameof(ClassicRows))]
    public void Collapse_ClassicZumaRow_LeavesNoGroupOfThreeOrMore(string row, string settled) =>
        Assert.Equal(settled, ZumaRow<ClassicZuma>.Collapse(row));

    // The rule's length is what decides: a group of three stays when four are needed.
    [Fact]
    public void Collapse_RuleOfFour_KeepsAGroupOfThreeAndClearsFour()
    {
        Assert.Equal("RRR", ZumaRow<ClearsFour>.Collapse("RRR"));
        Assert.Equal("", ZumaRow<ClearsFour>.Collapse("RRRR"));
    }

    private readonly struct ClearsFour : IZumaRule
    {
        public static int ShortestVanishingGroup => 4;
    }
}
