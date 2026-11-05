using DSAExperimentation.Domain.Language;
using DSAExperimentation.Domain.Language.English;
using DSAExperimentation.Domain.Science.Time;

namespace DSAExperimentation.Tests.Domain.Language.English;

// The words are the fact itself, so the tests state every one with the unit it names, read from
// TimeUnit's lengths, and check that the list holds them from the shortest unit to the longest.
public sealed partial class EnglishTimeUnitWordsTests
{
    public static TheoryData<TimeUnitWord, long, string> NamedWords => new()
    {
        { EnglishTimeUnitWords.Microsecond, TimeUnit.Microsecond.Ticks, "microsecond" },
        { EnglishTimeUnitWords.Millisecond, TimeUnit.Millisecond.Ticks, "millisecond" },
        { EnglishTimeUnitWords.Second, TimeUnit.Second.Ticks, "second" },
        { EnglishTimeUnitWords.Minute, TimeUnit.Minute.Ticks, "minute" },
        { EnglishTimeUnitWords.Hour, TimeUnit.Hour.Ticks, "hour" },
        { EnglishTimeUnitWords.Day, TimeUnit.Day.Ticks, "day" },
        { EnglishTimeUnitWords.Week, TimeUnit.Week.Ticks, "week" },
    };

    [Theory]
    [MemberData(nameof(NamedWords))]
    public void NamedWord_IsItsUnitsLengthAndWord(TimeUnitWord word, long ticks, string spelled) =>
        Assert.Equal(new TimeUnitWord(ticks, spelled), word);

    [Fact]
    public void All_IsEveryWordFromTheShortestUnitToTheLongest() =>
        Assert.Equal(
            ["microsecond", "millisecond", "second", "minute", "hour", "day", "week"],
            EnglishTimeUnitWords.All.Select(word => word.Word));
}
