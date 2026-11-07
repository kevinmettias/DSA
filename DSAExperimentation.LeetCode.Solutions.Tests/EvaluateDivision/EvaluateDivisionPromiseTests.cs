using DSAExperimentation.LeetCode.Solutions.EvaluateDivision;

namespace DSAExperimentation.LeetCode.Solutions.Tests.EvaluateDivision;

public sealed partial class EvaluateDivisionPromiseTests
{
    private static readonly string[][] NoQueries = [];

    public static TheoryData<string[][], double[]> ConsistentEquations =>
        new()
        {
            { [["a", "b"], ["b", "c"]], [2.0, 3.0] },
            { [["a", "b"], ["b", "c"], ["a", "c"]], [2.0, 3.0, 6.0] },
            { [["a", "b"], ["c", "d"]], [0.5, 4.0] },
            { [["x", "x"]], [1.0] },
        };

    public static TheoryData<string[][], double[]> ContradictoryEquations =>
        new()
        {
            { [["a", "b"], ["b", "c"], ["c", "d"], ["d", "a"]], [2.0, 3.0, 0.5, 0.333] },
            { [["v1", "v2"], ["v2", "v3"], ["v3", "v4"], ["v4", "v1"]], [3.0, 4.0, 5.0, 6.0] },
            { [["a", "b"], ["a", "b"]], [2.0, 3.0] },
        };

    [Theory]
    [MemberData(nameof(ConsistentEquations))]
    public void IsKeptBy_EveryPathOneRatio_IsTrue(string[][] equations, double[] values) =>
        Assert.True(EvaluateDivisionPromise.IsKeptBy(equations, values, NoQueries));

    [Theory]
    [MemberData(nameof(ContradictoryEquations))]
    public void IsKeptBy_TwoPathsTwoRatios_IsFalse(string[][] equations, double[] values) =>
        Assert.False(EvaluateDivisionPromise.IsKeptBy(equations, values, NoQueries));
}
