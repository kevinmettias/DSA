namespace DSAExperimentation.LeetCode.Solutions.AsteroidCollision;

// LC 735's promise about its input: no asteroid has size zero, so each one moves right or left.
// LeetCode's judge never poses an input that breaks it. An asteroid of size zero moves neither way,
// and the strategies treat it as one that moves right. Generated cases do not always keep the
// promise, so a harness that runs them asks this first.
internal static class AsteroidCollisionPromise
{
    public static bool IsKeptBy(int[] asteroids) => !asteroids.Contains(0);
}
