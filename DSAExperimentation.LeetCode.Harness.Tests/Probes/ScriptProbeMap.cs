namespace DSAExperimentation.LeetCode.Harness.Tests.Probes;

// The object ScriptProbeSolution's factory makes: a map that keeps every key it is given.
internal sealed class ScriptProbeMap(int capacity)
{
    private readonly Dictionary<int, int> _values = new(capacity);

    public void Put(int key, int value) => _values[key] = value;

    public bool TryGet(int key, out int value) => _values.TryGetValue(key, out value);
}
