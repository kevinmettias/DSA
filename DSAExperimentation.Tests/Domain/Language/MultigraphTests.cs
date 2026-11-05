using DSAExperimentation.Domain.Language;

namespace DSAExperimentation.Tests.Domain.Language;

public sealed partial class MultigraphTests
{
    public static TheoryData<string, int> Lengths => new() { { "sh", 2 }, { "tch", 3 }, { "sch", 3 } };

    [Theory]
    [MemberData(nameof(Lengths))]
    public void Length_IsHowManyLettersSpellTheSound(string letters, int expected) =>
        Assert.Equal(expected, new Multigraph(letters).Length);
}
