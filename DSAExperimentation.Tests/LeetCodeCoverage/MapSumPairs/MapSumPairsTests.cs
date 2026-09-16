using IMapSumStrategy = DSAExperimentation.LeetCode.MapSumPairs.MapSumPairsSolution.IMapSumStrategy;
using MapSumByDictionaryScan = DSAExperimentation.LeetCode.MapSumPairs.MapSumPairsSolution.MapSumByDictionaryScan;
using MapSumByTrieFold = DSAExperimentation.LeetCode.MapSumPairs.MapSumPairsSolution.MapSumByTrieFold;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MapSumPairs;

// Harness only. Both strategies are MapSumPairsSolution's - this file inserts
// every key/value in keys/values, then replays sum queries against each
// IMapSumStrategy implementation and checks the totals LeetCode itself
// publishes, so a failure still names the strategy that broke.
public sealed class MapSumPairsTests
{
    public static TheoryData<MapSumExample> Examples =>
        new()
        {
            // insert("apple", 3); sum("ap") == 3
            { new MapSumExample(Keys: ["apple"], Values: [3], Prefixes: ["ap"], Expected: [3]) },
            // insert("apple", 3); insert("app", 2); sum("ap") == 5
            { new MapSumExample(Keys: ["apple", "app"], Values: [3, 2], Prefixes: ["ap"], Expected: [5]) },
            // insert("apple", 3); insert("apple", 10) overrides; sum("apple") == 10
            { new MapSumExample(Keys: ["apple", "apple"], Values: [3, 10], Prefixes: ["apple"], Expected: [10]) },
            // insert("apple", 3); sum("banana") == 0 - unknown prefix
            { new MapSumExample(Keys: ["apple"], Values: [3], Prefixes: ["banana"], Expected: [0]) },
            // apple->3, app->2 and banana->4 inserted, then sum("") == 9 - the
            // empty prefix sums every inserted value.
            {
                new MapSumExample(
                    Keys: ["apple", "app", "banana"],
                    Values: [3, 2, 4],
                    Prefixes: [""],
                    Expected: [9])
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MapSumByDictionaryScan_LeetCodeExamples_ReturnsExpectedSums(MapSumExample example) =>
        AssertSums(new MapSumByDictionaryScan(), example);

    [Theory]
    [MemberData(nameof(Examples))]
    public void MapSumByTrieFold_LeetCodeExamples_ReturnsExpectedSums(MapSumExample example) =>
        AssertSums(new MapSumByTrieFold(), example);

    private static void AssertSums(IMapSumStrategy mapSum, MapSumExample example)
    {
        for (var i = 0; i < example.Keys.Length; i++)
        {
            mapSum.Insert(example.Keys[i], example.Values[i]);
        }

        for (var i = 0; i < example.Prefixes.Length; i++)
        {
            Assert.Equal(example.Expected[i], mapSum.Sum(example.Prefixes[i]));
        }
    }

    // One LeetCode example: the inserts, the prefixes to query afterwards, and the
    // total each query must return. They travel together at every call site - an
    // assertion helper that got them apart could line a prefix up with the wrong
    // total and still compile - so they are one thing with a name.
    public readonly record struct MapSumExample(string[] Keys, int[] Values, string[] Prefixes, int[] Expected);
}
