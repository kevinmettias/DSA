namespace DSAExperimentation.LeetCode.Harness.Tests.Probes;

// A solution-shaped class for CatalogColumnsTests, outside the solutions project and never enrolled: its
// strategy takes Rectangle Area's eight inputs as two records of four, so its cases are that question's.
[LeetCodeQuestion("rectangle-area", LeetCodeAnswerOrder.AsWritten)]
internal static class SpreadProbeSolution
{
    public static long AreaByCorners(ProbeCorners first, ProbeCorners second) => ((long)first.X2 - first.X1) * (first.Y2 - first.Y1);
}
