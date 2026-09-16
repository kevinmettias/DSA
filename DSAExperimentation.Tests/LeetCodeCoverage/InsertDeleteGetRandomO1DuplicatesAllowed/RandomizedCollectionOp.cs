using DSAExperimentation.LeetCode.InsertDeleteGetRandomO1DuplicatesAllowed;

namespace DSAExperimentation.Tests.LeetCodeCoverage.InsertDeleteGetRandomO1DuplicatesAllowed;

// One call in a RandomizedCollection script: which method to invoke and with what
// argument. Pure dispatch, built via the named factories below so a script (like
// InsertDeleteGetRandomO1DuplicatesAllowedTests.Examples) reads like the call
// sequence it replays. Its own file because
// InsertDeleteGetRandomO1DuplicatesAllowedTests' public Theory signatures name it,
// so it cannot be a nested private helper - and a second file-level type beside the
// test class would leave the file's name identifying neither.
public readonly record struct RandomizedCollectionOp(RandomizedCollectionOp.OpKind kind, int value)
{
    public static RandomizedCollectionOp Insert(int value) => new(OpKind.Insert, value);

    public static RandomizedCollectionOp Remove(int value) => new(OpKind.Remove, value);

    public static RandomizedCollectionOp GetRandom() => new(OpKind.GetRandom, 0);

    public static RandomizedCollectionOp Count() => new(OpKind.Count, 0);

    // Boxed uniformly so a script runner can assert against one expected value per
    // operation regardless of which method it dispatches to. Internal, not public:
    // IRandomizedCollection is internal to
    // InsertDeleteGetRandomO1DuplicatesAllowedSolution, and only this same assembly's
    // InsertDeleteGetRandomO1DuplicatesAllowedTests.RunScript ever calls Apply.
    internal object? Apply(InsertDeleteGetRandomO1DuplicatesAllowedSolution.IRandomizedCollection collection) =>
        kind switch
        {
            OpKind.Insert => collection.Insert(value),
            OpKind.Remove => collection.Remove(value),
            OpKind.GetRandom => collection.GetRandom(),
            _ => collection.Count,
        };

    public enum OpKind
    {
        Insert,
        Remove,
        GetRandom,
        Count,
    }
}
