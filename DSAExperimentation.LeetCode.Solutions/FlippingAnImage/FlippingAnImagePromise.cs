namespace DSAExperimentation.LeetCode.Solutions.FlippingAnImage;

// LC 832's promise about its input: the image is n by n. LeetCode's judge never poses an input that
// breaks it. The strategies flip each row across the image's height, which is its width only when
// it is square. Generated cases do not always keep the promise, so a harness that runs them asks
// this first.
internal static class FlippingAnImagePromise
{
    public static bool IsKeptBy(int[][] image) => image.All(row => row.Length == image.Length);
}
