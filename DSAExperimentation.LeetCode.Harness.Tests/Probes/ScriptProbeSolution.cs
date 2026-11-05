namespace DSAExperimentation.LeetCode.Harness.Tests.Probes;

// A design solution's shape for the call-script tests, outside the solutions project and never enrolled: it
// answers LRU Cache's scripts with a map that never evicts, whose put and remove the map answers itself and
// whose get the Calls type states, since the map answers only TryGet. Its answers are a map's, not a cache's.
[LeetCodeQuestion("lru-cache", LeetCodeAnswerOrder.AsWritten, Calls = typeof(ScriptProbeCalls))]
internal static class ScriptProbeSolution
{
    public static ScriptProbeMap CreateByDictionary(int capacity) => new(capacity);
}
