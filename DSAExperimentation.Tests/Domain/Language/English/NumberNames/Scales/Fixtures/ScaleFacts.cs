using DSAExperimentation.Domain.Language.English.NumberNames.Scales;

namespace DSAExperimentation.Tests.Domain.Language.English.NumberNames.Scales.Fixtures;

// What every scale must agree on: its k-th group word names 10 to the first group's digits plus k
// later groups' digits, so the words and the group sizes cannot drift apart.
internal static class ScaleFacts
{
    public static IEnumerable<(long Expected, long Actual)> GroupWordValues<TScale>()
        where TScale : struct, INumberScale =>
        TScale.GroupWords.Select((word, index) =>
            (PowerOfTen(TScale.FirstGroupDigits + (index * TScale.LaterGroupDigits)), word.Value));

    private static long PowerOfTen(int digits) =>
        Enumerable.Repeat(10L, digits).Aggregate(1L, (power, ten) => power * ten);
}
