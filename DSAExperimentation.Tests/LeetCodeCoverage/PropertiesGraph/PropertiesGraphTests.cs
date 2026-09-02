using DSAExperimentation.LeetCode.PropertiesGraph;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PropertiesGraph;

// Harness only. Both strategies are PropertiesGraphSolution's - this file just
// pins them to LeetCode's published examples, including the below-threshold pair
// that must NOT connect despite sharing a value.
public sealed class PropertiesGraphTests
{
    public static TheoryData<int[][], int, int> Examples =>
        new()
        {
            { [[1, 2], [1, 1], [3, 4], [4, 5], [5, 6], [7, 7]], 1, 3 },
            { [[1, 2, 3], [2, 3, 4], [4, 3, 5]], 2, 1 },
            { [[1, 1], [1, 1]], 2, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfComponentsByBruteForce_LeetCodeExamples_ReturnsComponentCount(
        int[][] properties, int k, int expected) =>
        Assert.Equal(expected, PropertiesGraphSolution.NumberOfComponentsByBruteForce(properties, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfComponentsByDisjointSet_LeetCodeExamples_ReturnsComponentCount(
        int[][] properties, int k, int expected) =>
        Assert.Equal(expected, PropertiesGraphSolution.NumberOfComponentsByDisjointSet(properties, k));
}
