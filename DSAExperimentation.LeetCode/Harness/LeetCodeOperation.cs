namespace DSAExperimentation.LeetCode.Harness;

// One call in a design problem's operation sequence. LeetCode states these
// problems as two parallel arrays - ["LRUCache","put","get"] and
// [[2],[1,1],[1]] - producing a result per call, with null for the ones that
// return nothing; this is one entry of that, and a registration's TInput is
// simply a list of them.
//
// Integer arguments only, which covers the design problems in this repo (Min
// Stack, LRU Cache, All O`one's counts). A design problem whose operations take
// strings or anything else does NOT stretch this type - TInput is free, so it
// declares its own script shape and its registration replays that instead. The
// shared type is here because most design problems are int-shaped, not because
// every one must be.
internal readonly record struct LeetCodeOperation(string Name, IReadOnlyList<int> Arguments)
{
    public static LeetCodeOperation Of(string name, params int[] arguments) => new(name, arguments);

    public override string ToString() => $"{Name}({string.Join(',', Arguments)})";
}
