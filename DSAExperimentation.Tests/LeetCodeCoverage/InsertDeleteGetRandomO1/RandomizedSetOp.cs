using DSAExperimentation.LeetCode.InsertDeleteGetRandomO1;

namespace DSAExperimentation.Tests.LeetCodeCoverage.InsertDeleteGetRandomO1;

// One call in a RandomizedSet script: which method to invoke and with what argument.
// Pure dispatch, built via the named factories below so a script (like
// InsertDeleteGetRandomO1Tests.Examples) reads like the LeetCode call sequence it
// replays. Its own file because InsertDeleteGetRandomO1Tests' public Theory
// signatures name it, so it cannot be a nested private helper - and a second
// file-level type beside the test class would leave the file's name identifying
// neither.
public readonly record struct RandomizedSetOp(RandomizedSetOp.OpKind kind, int value)
{
    public static RandomizedSetOp Insert(int value) => new(OpKind.Insert, value);

    public static RandomizedSetOp Remove(int value) => new(OpKind.Remove, value);

    public static RandomizedSetOp GetRandom() => new(OpKind.GetRandom, 0);

    public static RandomizedSetOp Count() => new(OpKind.Count, 0);

    // Boxed uniformly so a script runner can assert against one expected value per
    // operation regardless of which method it dispatches to. Internal, not public:
    // IRandomizedSet is internal to InsertDeleteGetRandomO1Solution, and only this
    // same assembly's InsertDeleteGetRandomO1Tests.RunScript ever calls Apply.
    internal object? Apply(InsertDeleteGetRandomO1Solution.IRandomizedSet set) => kind switch
    {
        OpKind.Insert => set.Insert(value),
        OpKind.Remove => set.Remove(value),
        OpKind.GetRandom => set.GetRandom(),
        _ => set.Count,
    };

    public enum OpKind
    {
        Insert,
        Remove,
        GetRandom,
        Count,
    }
}
