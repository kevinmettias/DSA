namespace DSAExperimentation.LeetCode.Harness.Tests.Probes;

// An answer-check-shaped class for CatalogAnswerCheckTests and CatalogJudgeTests: it accepts any answer of
// the recorded answer's parity that is no larger than the input array is long, so a test can tell that it
// read the case's input, the answer and the recorded answer, each into its own type.
internal static class SameParityProbeAnswerCheck
{
    public static bool IsAccepted(int[] values, int answer, int recorded) => answer % 2 == recorded % 2 && answer <= values.Length;
}
