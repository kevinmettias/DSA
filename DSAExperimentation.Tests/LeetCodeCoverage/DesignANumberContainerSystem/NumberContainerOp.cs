using DSAExperimentation.LeetCode.DesignANumberContainerSystem;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignANumberContainerSystem;

// One call in a NumberContainers script: which method to invoke and with what
// arguments. Pure dispatch, built via the named factories below so a script (like
// Examples in DesignANumberContainerSystemTests) reads like the LeetCode call
// sequence it replays.
public readonly record struct NumberContainerOp(bool isFind, int index, int number)
{
    public static NumberContainerOp Change(int index, int number) => new(isFind: false, index, number);

    public static NumberContainerOp Find(int number) => new(isFind: true, index: 0, number);

    // null for the void Change call, the reported index for Find - so a script
    // runner can assert against one expected value per operation uniformly.
    // Internal, not public: INumberContainerStrategy is internal to
    // DesignANumberContainerSystemSolution, and only this same assembly's
    // RunScript ever calls Apply.
    internal int? Apply(DesignANumberContainerSystemSolution.INumberContainerStrategy strategy)
    {
        if (isFind)
        {
            return strategy.Find(number);
        }

        strategy.Change(index, number);
        return null;
    }
}
