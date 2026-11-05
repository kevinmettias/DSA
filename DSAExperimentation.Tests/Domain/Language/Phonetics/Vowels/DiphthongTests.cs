using DSAExperimentation.Domain.Language.Phonetics.Vowels;

namespace DSAExperimentation.Tests.Domain.Language.Phonetics.Vowels;

public sealed partial class DiphthongTests
{
    [Fact]
    public void Symbol_IsTheStartThenTheEnd() =>
        Assert.Equal("aɪ", new Diphthong(IpaVowels.OpenFrontUnrounded, IpaVowels.NearCloseNearFrontUnrounded).Symbol);
}
