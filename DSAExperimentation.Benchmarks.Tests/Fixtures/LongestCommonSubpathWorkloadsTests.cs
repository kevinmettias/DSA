using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for LongestCommonSubpathWorkloads (ARCHITECTURE 17.7). The reading depends on the
// paths running over a tiny set of cities, so long shared runs occur, and on staying inside LC 1923's
// contract: no path lists the same city twice in a row.
public sealed partial class LongestCommonSubpathWorkloadsTests
{
    private const int PathCount = 3;
    private const int PathLength = 500; // the benchmark's largest size
    private const int Seed = 1923; // LC problem number
    private const int FirstCity = 1;
    private const int CityCount = 5;
    private const int SecondPosition = 1;

    [Fact]
    public void BuildPaths_PathCountAndLength_ReturnsThatManyPathsOfThatLength()
    {
        var paths = Build();

        Assert.Equal(PathCount, paths.Length);
        Assert.All(paths, path => Assert.Equal(PathLength, path.Length));
    }

    [Fact]
    public void BuildPaths_EveryCity_ComesFromTheSmallCitySet() =>
        Assert.All(Build(), path => Assert.All(path, city => Assert.InRange(city, FirstCity, CityCount)));

    [Fact]
    public void BuildPaths_EveryPath_NeverListsACityTwiceInARow() =>
        Assert.All(
            Build(),
            path => Assert.All(
                Enumerable.Range(SecondPosition, PathLength - SecondPosition),
                position => Assert.NotEqual(path[position - 1], path[position])));

    [Fact]
    public void BuildPaths_SameSeed_ReturnsTheSamePaths() =>
        Assert.Equal(Build(), Build());

    private static int[][] Build() => LongestCommonSubpathWorkloads.BuildPaths(PathCount, PathLength, Seed);
}
