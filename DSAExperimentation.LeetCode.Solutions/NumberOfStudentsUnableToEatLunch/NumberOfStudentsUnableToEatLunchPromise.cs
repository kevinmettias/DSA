namespace DSAExperimentation.LeetCode.Solutions.NumberOfStudentsUnableToEatLunch;

// LC 1700's promise about its input: there are as many sandwiches as students. LeetCode's judge
// never poses an input that breaks it. A sandwich no student is there for is read past the end of
// the queue. Generated cases do not always keep the promise, so a harness that runs them asks this
// first.
internal static class NumberOfStudentsUnableToEatLunchPromise
{
    public static bool IsKeptBy(int[] students, int[] sandwiches) => students.Length == sandwiches.Length;
}
