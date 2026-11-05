namespace DSAExperimentation.LeetCode.Harness.Tests.Probes;

// A solution-shaped class for LeetCodeShapeTests, outside the solutions project and never enrolled: each
// strategy has two overloads a case could feed, as 17.4's hoisted overloads may - rows beside tuples, a
// declared input beside one more, an array beside a list that match alike, and a tuple spread over three
// inputs beside a table built from them.
internal static class ShapedProbeSolution
{
    public static int CountByLoop(int[][] intervals) => intervals.Length;

    public static int CountByLoop((int Start, int End)[] intervals) => intervals.Length;

    public static int SumByLoop(int[] values) => values.Sum();

    public static int SumByLoop(int[] values, int half) => values.Sum() - half;

    public static int PickByLoop(int[] values) => values[0];

    public static int PickByLoop(IList<int> values) => values[0];

    public static int CostByLoop(string source, (char[] Original, char[] Changed, int[] Cost) rules) => source.Length + rules.Cost.Sum();

    public static int CostByLoop(string source, long[,] distances) => source.Length + distances.Length;
}
