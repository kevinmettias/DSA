using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FlippingAnImage;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FlippingAnImageSolution's, the same methods
// FlippingAnImageTests proves correct - an in-place two-pointer reverse+invert
// (the textbook approach) vs. this repo's own LIFO Stack<int> reversing each row
// while inverting each value as it comes back off the stack. Each iteration clones
// the pristine image before flipping, since the solution rewrites in place and
// [GlobalSetup] runs once per benchmark, not once per invocation.
[MemoryDiagnoser]
public class FlippingAnImageBenchmarks
{
    private const int RandomSeed = 4;
    private const int PixelValueExclusiveBound = 2; private int[][] _image = [];

    // pixels are binary: 0 or 1

    [Params(50, 300)]
    public int Side { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _image = new int[Side][];

        for (var r = 0; r < Side; r++)
        {
            _image[r] = new int[Side];

            for (var c = 0; c < Side; c++)
            {
                _image[r][c] = random.Next(0, PixelValueExclusiveBound);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int[][] TwoPointerReverseAndInvert() =>
        FlippingAnImageSolution.FlipAndInvertImageByTwoPointerReverse(Clone(_image));

    [Benchmark]
    public int[][] StackReverseAndInvert() =>
        FlippingAnImageSolution.FlipAndInvertImageByStackReverse(Clone(_image));

    private static int[][] Clone(int[][] image)
    {
        var copy = new int[image.Length][];

        for (var r = 0; r < image.Length; r++)
        {
            copy[r] = (int[])image[r].Clone();
        }

        return copy;
    }
}
