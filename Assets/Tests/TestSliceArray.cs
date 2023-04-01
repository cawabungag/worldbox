using NUnit.Framework;

public class TestSliceArray
{
    [Test]
    public void TestSliceArraySimplePasses()
	{
		var testCase = new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16};
		var rectangular = testCase.ToRectangular(4);
		var slice = rectangular.Slice(0, 0, 1, 1);
		Assert.AreEqual(new[] {1}, slice);
	}
}
