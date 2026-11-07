using DSAExperimentation.DataStructures.SubstringIndex;

namespace DSAExperimentation.Tests.DataStructures.SubstringIndex;

public sealed partial class SubstringOccurrenceTests
{
    [Fact]
    public void Empty_IsNoCharactersAtTheStart() =>
        Assert.Equal(new SubstringOccurrence(0, 0), SubstringOccurrence.Empty);
}
