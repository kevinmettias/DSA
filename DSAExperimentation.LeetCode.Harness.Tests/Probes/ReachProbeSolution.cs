namespace DSAExperimentation.LeetCode.Harness.Tests.Probes;

// A solution-shaped class for CatalogColumnsTests, outside the solutions project and never enrolled: it
// answers Climbing Stairs, and its reach keeps only the shorter flights, so its cases are that question's
// within the reach.
[LeetCodeQuestion("climbing-stairs", LeetCodeAnswerOrder.AsWritten, Reach = typeof(ReachProbeReach))]
internal static class ReachProbeSolution
{
    public static int WaysByCounting(int stairs) => stairs;
}
