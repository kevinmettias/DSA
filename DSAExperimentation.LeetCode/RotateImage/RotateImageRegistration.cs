using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.LeetCode.RotateImage;

// Shape under test: IN-PLACE MUTATION with a void return. This is the case that
// dictates a piece of the harness design, so it is worth being explicit about.
//
// A case's input is handed to EVERY strategy in turn, and each benchmark
// iteration reuses its workload. If TInput were the int[][] itself, the first
// strategy would rotate the shared matrix and the second would be handed an
// already-rotated one - passing or failing for reasons having nothing to do with
// the code under test. So a mutating problem's TInput is a FACTORY, and each run
// builds its own matrix. Any problem that writes through its argument registers
// this way.
internal sealed class RotateImageRegistration : ILeetCodeProblemRegistration
{
    // Both mechanisms are stateless, so one instance each serves every run and no
    // benchmark iteration allocates one just to name a strategy.
    private static readonly IRotation ArrayReverseRotation = new ArrayReverseMechanism();
    private static readonly IRotation StackReverseRotation = new StackReverseMechanism();

    public LeetCodeProblem Describe()
        => LeetCodeProblem.For<Func<int[][]>, int[][]>("rotate-image")
            .Strategy("ArrayReverse", input => Rotated(ArrayReverseRotation, input()))
            .Strategy("StackReverse", input => Rotated(StackReverseRotation, input()))
            .MatchingAnswersWith(LeetCodeAnswers.SequenceOfSequencesEqual<int>)
            .Case(
                "example-1",
                () => [[1, 2, 3], [4, 5, 6], [7, 8, 9]],
                [[7, 4, 1], [8, 5, 2], [9, 6, 3]])
            .Case(
                "example-2",
                () => [[5, 1, 9, 11], [2, 4, 8, 10], [13, 3, 6, 7], [15, 14, 12, 16]],
                [[15, 13, 2, 5], [14, 3, 4, 1], [12, 6, 8, 9], [16, 7, 10, 11]])
            .Case("single-cell", () => [[1]], [[1]])
            // The two sizes the retired per-problem benchmark swept with [Params].
            // Kept apart rather than collapsed into one middling size: the stack
            // arm's allocation per row is what separates the strategies, and that
            // cost only overtakes the in-place reversal as the square grows.
            .Workload("square-50", () => BuildSquare(50))
            .Workload("square-300", () => BuildSquare(300))
            .Build();

    // How a matrix gets rotated in place - the one thing the two strategies do
    // differently. The matrix is built and handed in by the caller rather than by this
    // interface, because a mutating problem's input has to be a factory (see above).
    private interface IRotation
    {
        void ApplyTo(int[][] matrix);
    }

    private static int[][] Rotated(IRotation rotation, int[][] matrix)
    {
        rotation.ApplyTo(matrix);

        return matrix;
    }

    private static int[][] BuildSquare(int size)
        => [.. Enumerable.Range(0, size).Select(row => Enumerable.Range(row * size, size).ToArray())];

    // Each mechanism is a direct call to the solution method it names: the rotation is
    // the code under test, so nothing here may stand between a strategy and it.
    private sealed class ArrayReverseMechanism : IRotation
    {
        public void ApplyTo(int[][] matrix) => RotateImageSolution.RotateByArrayReverse(matrix);
    }

    private sealed class StackReverseMechanism : IRotation
    {
        public void ApplyTo(int[][] matrix) => RotateImageSolution.RotateByStackReverse(matrix);
    }
}
