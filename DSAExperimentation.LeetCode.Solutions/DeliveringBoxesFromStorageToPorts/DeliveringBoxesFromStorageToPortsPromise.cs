namespace DSAExperimentation.LeetCode.Solutions.DeliveringBoxesFromStorageToPorts;

// LC 1687's promise about its input: every box goes to a port from 1 to portsCount and weighs from 1 to
// maxWeight, so one trip can always carry any single box. LeetCode's judge never poses an input that breaks
// it. A box heavier than the ship can carry never leaves storage, and both strategies then answer
// nonsense, as the dataset's own recorded output is. Generated cases do not always keep the promise, so a
// harness that runs them asks this first.
internal static class DeliveringBoxesFromStorageToPortsPromise
{
    private const int Port = 0;
    private const int Weight = 1;
    private const int Lowest = 1;

    public static bool IsKeptBy(int[][] boxes, int portsCount, int maxBoxes, int maxWeight) =>
        maxBoxes >= Lowest && boxes.All(box => IsWithin(box[Port], portsCount) && IsWithin(box[Weight], maxWeight));

    private static bool IsWithin(int value, int highest) => value >= Lowest && value <= highest;
}
