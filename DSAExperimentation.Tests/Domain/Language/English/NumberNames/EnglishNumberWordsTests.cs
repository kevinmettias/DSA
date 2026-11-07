using DSAExperimentation.Domain.Language;
using DSAExperimentation.Domain.Language.English.NumberNames;

namespace DSAExperimentation.Tests.Domain.Language.English.NumberNames;

// The words are the fact itself, so the tests state every one with the number it names, and check
// that each table holds its words in order of number.
public sealed partial class EnglishNumberWordsTests
{
    private static readonly string[] OwnWordsInOrder =
    [
        "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine",
        "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen",
    ];

    public static TheoryData<NumberWord, long, string> NamedWords => new()
    {
        { EnglishNumberWords.Zero, 0, "zero" },
        { EnglishNumberWords.Seventeen, 17, "seventeen" },
        { EnglishNumberWords.Twenty, 20, "twenty" },
        { EnglishNumberWords.Forty, 40, "forty" },
        { EnglishNumberWords.Ninety, 90, "ninety" },
        { EnglishNumberWords.Hundred, 100, "hundred" },
        { EnglishNumberWords.Thousand, 1_000, "thousand" },
        { EnglishNumberWords.Quintillion, 1_000_000_000_000_000_000, "quintillion" },
    };

    [Theory]
    [MemberData(nameof(NamedWords))]
    public void NamedWord_IsItsNumberAndWord(NumberWord word, long value, string spelled) =>
        Assert.Equal(new NumberWord(value, spelled), word);

    [Fact]
    public void OwnWords_AreZeroToNineteenEachAtItsNumber() =>
        Assert.Equal(
            OwnWordsInOrder.Select((word, number) => new NumberWord(number, word)),
            EnglishNumberWords.OwnWords);

    [Fact]
    public void Tens_AreTwentyToNinetyInOrder() =>
        Assert.Equal<NumberWord>(
            [
                new(20, "twenty"), new(30, "thirty"), new(40, "forty"), new(50, "fifty"),
                new(60, "sixty"), new(70, "seventy"), new(80, "eighty"), new(90, "ninety"),
            ],
            EnglishNumberWords.Tens);

    [Fact]
    public void ShortScale_IsEachPowerOfAThousandInOrder() =>
        Assert.Equal<NumberWord>(
            [
                new(1_000, "thousand"), new(1_000_000, "million"), new(1_000_000_000, "billion"),
                new(1_000_000_000_000, "trillion"), new(1_000_000_000_000_000, "quadrillion"),
                new(1_000_000_000_000_000_000, "quintillion"),
            ],
            EnglishNumberWords.ShortScale);

    [Fact]
    public void ShortScale_EachWordIsAThousandOfThePrevious() =>
        Assert.All(
            EnglishNumberWords.ShortScale.Zip(EnglishNumberWords.ShortScale.Skip(1)),
            pair => Assert.Equal(pair.First.Value * EnglishNumberWords.Thousand.Value, pair.Second.Value));
}
