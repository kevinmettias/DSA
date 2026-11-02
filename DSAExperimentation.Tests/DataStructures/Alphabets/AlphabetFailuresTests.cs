using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.Tests.DataStructures.Alphabets;

public sealed partial class AlphabetFailuresTests
{
    [Fact]
    public void ThrowNotALetter_NamesTheLetter() =>
        Assert.Equal("letter", Assert.Throws<ArgumentOutOfRangeException>(() => AlphabetFailures.ThrowNotALetter('!')).ParamName);

    [Fact]
    public void ThrowNotASlot_NamesTheIndex() =>
        Assert.Equal("index", Assert.Throws<ArgumentOutOfRangeException>(() => AlphabetFailures.ThrowNotASlot(-1)).ParamName);
}
