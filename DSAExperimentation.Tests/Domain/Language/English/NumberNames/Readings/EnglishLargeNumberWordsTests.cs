using System.Numerics;
using DSAExperimentation.Domain.Language.English.NumberNames;
using DSAExperimentation.Domain.Language.English.NumberNames.Readings;

namespace DSAExperimentation.Tests.Domain.Language.English.NumberNames.Readings;

public sealed partial class EnglishLargeNumberWordsTests
{
    [Fact]
    public void Googol_IsTenToTheHundred() =>
        Assert.Equal(new PowerOfTenWord(100, "googol"), EnglishLargeNumberWords.Googol);

    [Fact]
    public void Googolplex_IsTenToTheGoogol()
    {
        Assert.Equal("googolplex", EnglishLargeNumberWords.Googolplex.Word);
        Assert.Equal(BigInteger.Pow(10, 100), EnglishLargeNumberWords.Googolplex.Exponent);
    }

    [Fact]
    public void All_IsGoogolThenGoogolplex() =>
        Assert.Equal<PowerOfTenWord>(
            [EnglishLargeNumberWords.Googol, EnglishLargeNumberWords.Googolplex], EnglishLargeNumberWords.All);
}
