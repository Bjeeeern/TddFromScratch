namespace TestSuites;

public class DeepCloneTests
{
    public void CanDeepCloneScene()
    {
        var model = new TestModel
        {
            StringProperty = "test value",
            PrimitiveProperty = 1,
            ValueTypeProperty = Vector2.UnitX,
            ComplexProperty = new()
            {
                ValueTypeProperty = Vector2.UnitY,
            },
        };

        var clone = model.DeepClone();

        Assert.Equivalent(model, clone);
        Assert.NotSame(model.ComplexProperty, clone.ComplexProperty);
    }

    internal class TestModel
    {
        internal string? StringProperty { get; set; }
        internal int PrimitiveProperty { get; set; }
        internal Vector2 ValueTypeProperty { get; set; }

        internal ComplexPropertyModel? ComplexProperty { get; set; }

        internal class ComplexPropertyModel
        {
            internal Vector2 ValueTypeProperty { get; set; }
        }
    }
}