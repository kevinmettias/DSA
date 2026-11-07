using DSAExperimentation.Domain.Language.English.NumberNames;
using DSAExperimentation.Domain.Language.English.NumberNames.Readings;

namespace DSAExperimentation.Tests.Domain.Language.English.NumberNames.Readings;

public sealed partial class EnglishMathematicalWordsTests
{
    [Fact]
    public void Readings_AreEnglishsWords()
    {
        Assert.Equal("infinity", EnglishMathematicalWords.Infinity);
        Assert.Equal("negative infinity", EnglishMathematicalWords.NegativeInfinity);
        Assert.Equal("not a number", EnglishMathematicalWords.NotANumber);
        Assert.Equal("i", EnglishMathematicalWords.ImaginaryUnit);
        Assert.Equal("omega", EnglishMathematicalWords.Omega);
        Assert.Equal("aleph-null", EnglishMathematicalWords.AlephNull);
    }
}
