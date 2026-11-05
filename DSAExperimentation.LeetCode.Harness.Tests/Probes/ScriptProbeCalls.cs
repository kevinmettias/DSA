namespace DSAExperimentation.LeetCode.Harness.Tests.Probes;

// The call ScriptProbeMap does not answer by a method of its name: LeetCode's get, which answers -1 for a
// missing key.
internal static class ScriptProbeCalls
{
    private const int Missing = -1;

    public static int Get(ScriptProbeMap map, int key)
    {
        if (!map.TryGet(key, out var value))
        {
            return Missing;
        }

        return value;
    }
}
