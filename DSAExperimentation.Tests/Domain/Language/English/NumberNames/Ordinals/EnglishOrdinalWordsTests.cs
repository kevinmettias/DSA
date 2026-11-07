using DSAExperimentation.Domain.Language;
using DSAExperimentation.Domain.Language.English.NumberNames;
using DSAExperimentation.Domain.Language.English.NumberNames.Ordinals;

namespace DSAExperimentation.Tests.Domain.Language.English.NumberNames.Ordinals;

// The seven exceptions are the fact, written out; every other word takes the suffix rule.
public sealed partial class EnglishOrdinalWordsTests
{
    public static TheoryData<IrregularOrdinal, long, string> Exceptions => new()
    {
        { EnglishOrdinalWords.First, 1, "first" },
        { EnglishOrdinalWords.Second, 2, "second" },
        { EnglishOrdinalWords.Third, 3, "third" },
        { EnglishOrdinalWords.Fifth, 5, "fifth" },
        { EnglishOrdinalWords.Eighth, 8, "eighth" },
        { EnglishOrdinalWords.Ninth, 9, "ninth" },
        { EnglishOrdinalWords.Twelfth, 12, "twelfth" },
    };

    public static TheoryData<NumberWord, string> Regular => new()
    {
        { EnglishNumberWords.Zero, "zeroth" },
        { EnglishNumberWords.Four, "fourth" },
        { EnglishNumberWords.Eleven, "eleventh" },
        { EnglishNumberWords.Eighteen, "eighteenth" },
        { EnglishNumberWords.Twenty, "twentieth" },
        { EnglishNumberWords.Ninety, "ninetieth" },
        { EnglishNumberWords.Hundred, "hundredth" },
        { EnglishNumberWords.Quintillion, "quintillionth" },
    };

    [Theory]
    [MemberData(nameof(Exceptions))]
    public void Exception_IsItsNumberWordAndOrdinal(IrregularOrdinal exception, long number, string ordinal)
    {
        Assert.Equal(number, exception.Cardinal.Value);
        Assert.Equal(ordinal, exception.Ordinal);
        Assert.Equal(ordinal, EnglishOrdinalWords.OrdinalOf(exception.Cardinal));
    }

    [Theory]
    [MemberData(nameof(Regular))]
    public void OrdinalOf_RegularWord_TakesTheSuffix(NumberWord word, string ordinal) =>
        Assert.Equal(ordinal, EnglishOrdinalWords.OrdinalOf(word));

    [Fact]
    public void Irregular_IsTheSevenExceptionsInOrder() =>
        Assert.Equal(["first", "second", "third", "fifth", "eighth", "ninth", "twelfth"], EnglishOrdinalWords.Irregular.Select(word => word.Ordinal));
}
